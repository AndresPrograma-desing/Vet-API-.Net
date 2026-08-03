using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Middleware
{
    //Describe: Middleware global para capturar y registrar excepciones no controladas y respuestas de error de la API en la base de datos.
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISystemLogService logService)
        {
            // 1. Guardar el stream de respuesta original
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
                await HandleExceptionAsync(context, ex);
                
                // Copiar la respuesta de error al stream original
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                return;
            }

            // Si el código de estado es de error, leer el cuerpo del buffer
            if (context.Response.StatusCode >= 400)
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                string responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                string accion = context.Response.StatusCode == 500 
                    ? "ERROR_INTERNO_CONTROLADO" 
                    : "SOLICITUD_INCORRECTA (400)";

                await RegistrarLogErrorAsync(context, logService, $"{accion}: {responseText}", null);
            }

            // Copiar la respuesta original de vuelta al flujo HTTP
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
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
                // No permitir que falle el middleware si falla el registro de logs
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        { 
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new { message = exception.Message };
            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}