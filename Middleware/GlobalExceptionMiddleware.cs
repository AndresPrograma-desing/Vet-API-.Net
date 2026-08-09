using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
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

                    await EscribirRespuestaGenerica500Async(context, responseBody);
                }
                else
                {
                    await RegistrarLogErrorAsync(context, logService, $"SOLICITUD_INCORRECTA ({context.Response.StatusCode}): {responseText}", null);
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