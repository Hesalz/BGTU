using System.Text.Json;

namespace ANC25_WEBAPI_DLL.Services
{
    public class CelebritiesError
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CelebritiesError> _logger;

        public CelebritiesError(RequestDelegate next, ILogger<CelebritiesError> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in Celebrities module");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Celebrities Module Error",
                    Error = new
                    {
                        Type = ex.GetType().Name,
                        ex.Message,
                        StackTrace = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                            ? ex.StackTrace
                            : null,
                        Source = ex.Source,
                        InnerException = ex.InnerException?.Message
                    },
                    Request = new
                    {
                        context.Request.Method,
                        context.Request.Path,
                        context.Request.QueryString,
                        context.Request.Headers
                    },
                    Timestamp = DateTime.UtcNow
                };

                var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }

    public static class CelebritiesErrorExtensions
    {
        public static IApplicationBuilder UseCelebritiesErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CelebritiesError>();
        }
    }
}
