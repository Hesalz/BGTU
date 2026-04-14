using System.Net;

namespace Lab7.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (NullReferenceException ex)
            {
                LogError(context, ex, "Not Found");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await HandleExceptionAsync(context, ex.Message);
            }
            catch (ArgumentException ex)
            {
                LogError(context, ex, "Bad Request");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await HandleExceptionAsync(context, ex.Message);
            }
            catch (NotSupportedException ex)
            {
                LogError(context, ex, "Method Not Allowed");
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                await HandleExceptionAsync(context, ex.Message);
            }
            catch (Exception ex)
            {
                LogError(context, ex, "Internal Server Error", isCritical: true);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await HandleExceptionAsync(context, "An unexpected error occurred");
            }
        }

        private void LogError(HttpContext context, Exception ex, string errorType, bool isCritical = false)
        {
            var logMessage = $"""
                Error Type: {errorType}
                Path: {context.Request.Path}
                Method: {context.Request.Method}
                Message: {ex.Message}
                Stack Trace: {ex.StackTrace}
                """;

            if (isCritical)
            {
                _logger.LogCritical(logMessage);
            }
            else
            {
                _logger.LogError(logMessage);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, string message)
        {
            return context.Response.WriteAsJsonAsync(new
            {
                ErrorMessage = message,
                StatusCode = context.Response.StatusCode,
                TimeStamp = DateTime.UtcNow
            });
        }
    }
}