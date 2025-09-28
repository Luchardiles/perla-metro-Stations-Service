using System.Net;
using System.Text.Json;

/// <summary>   
/// Middleware para el manejo centralizado de excepciones
namespace perla_metro_Stations_Service.src.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Ocurrió una excepción no manejada: {Message}", exception.Message);

            context.Response.ContentType = "application/json";

            var response = new
            {
                message = "Ocurrió un error interno en el servidor",
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.Value
            };

            switch (exception)
            {
                case InvalidOperationException:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    response = new
                    {
                        message = exception.Message,
                        timestamp = DateTime.UtcNow,
                        path = context.Request.Path.Value
                    };
                    break;

                case ArgumentException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        message = exception.Message,
                        timestamp = DateTime.UtcNow,
                        path = context.Request.Path.Value
                    };
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = new
                    {
                        message = "Recurso no encontrado",
                        timestamp = DateTime.UtcNow,
                        path = context.Request.Path.Value
                    };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}