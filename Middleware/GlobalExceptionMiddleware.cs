using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Middleware
{
    // Middleware global para capturar, registrar en BD y sanitizar errores 500 hacia el cliente.
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISystemLogService logService)
        {
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            { 
                await _next(context);
            }
            catch (Exception ex)
            {
                await RegistrarLogErrorAsync(context, logService, $"ERROR_NO_CONTROLADO: {ex.Message}", ex.StackTrace);
                
                await EscribirRespuestaGenerica500Async(context, responseBody);

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                return;
            }

            if (context.Response.StatusCode >= 400)
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                string responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();

                if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
                {
                    await RegistrarLogErrorAsync(context, logService, $"ERROR_INTERNO_CONTROLADO: {responseText}", null);

                    // Solo se reemplaza por el mensaje genérico cuando el controller no dejó
                    // un cuerpo de error propio (500 vacío o no controlado). Si ya viene un
                    // mensaje (ej. try/catch del controller), se respeta tal cual.
                    if (string.IsNullOrWhiteSpace(responseText))
                    {
                        await EscribirRespuestaGenerica500Async(context, responseBody);
                    }
                }
                else
                {
                    await RegistrarLogErrorAsync(context, logService, $"SOLICITUD_INCORRECTA ({context.Response.StatusCode}): {responseText}", null);

                    // El 400 automático de [ApiController] ante un ModelState inválido (ej. un
                    // [Range]/[Required] del DTO) devuelve un ValidationProblemDetails crudo
                    // ({ title, errors: { Campo: [...] } }) que el front no sabe interpretar y
                    // terminaba mostrando tal cual. Se sanitiza a un mensaje genérico, igual que
                    // ya se hace acá abajo para el 500; los 400 armados a mano en los controllers
                    // (BadRequest(new { error = "..." })) ya traen su propio mensaje y no matchean
                    // este shape, así que se respetan sin tocar.
                    if (context.Response.StatusCode == StatusCodes.Status400BadRequest && EsValidationProblemDetails(responseText))
                    {
                        await EscribirRespuestaValidacionAsync(context, responseBody);
                    }
                }
            }

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static async Task EscribirRespuestaGenerica500Async(HttpContext context, MemoryStream responseBody)
        {
            responseBody.SetLength(0); 

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var jsonResponse = JsonSerializer.Serialize(new 
            { 
                message = "Error al procesar la solictud enviada." 
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        // Detecta el shape por defecto de ValidationProblemDetails ({ title, status, errors })
        // que [ApiController] genera automáticamente ante un ModelState inválido, sin parsear
        // JSON completo: alcanza con los nombres de campo que ese formato siempre trae.
        private static bool EsValidationProblemDetails(string responseText)
        {
            return responseText.Contains("\"errors\"", StringComparison.OrdinalIgnoreCase)
                && responseText.Contains("\"title\"", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task EscribirRespuestaValidacionAsync(HttpContext context, MemoryStream responseBody)
        {
            responseBody.SetLength(0);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var jsonResponse = JsonSerializer.Serialize(new
            {
                error = ResponseErrors.ValidationFailed
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        private static async Task RegistrarLogErrorAsync(HttpContext context, ISystemLogService logService, string accion, string? stackTrace)
        {
            try
            {
                var idClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? userId = int.TryParse(idClaim, out var parsedId) ? parsedId : null;

                string? ipAddress = context.Connection.RemoteIpAddress?.ToString();
                string? userAgent = context.Request.Headers["User-Agent"].ToString();

                await logService.RegisterLogAsync(
                    accion: accion,
                    tablaAfectada: context.Request.Path,
                    datosNuevos: stackTrace,
                    usuarioId: userId,
                    ipAddress: ipAddress,
                    userAgent: userAgent
                );
            }
            catch
            {
                //RETURN 0;
            }
        }
    }
}