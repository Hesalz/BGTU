using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace ANC25_WEBAPI_DLL {
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
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

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = exception switch
            {
                DeleteError => new ErrorResponse(404, "Not Found", exception.Message),
                NullError => new ErrorResponse(404, "Not Found", exception.Message),
                FileError => new ErrorResponse(404, "File Error", exception.Message),
                MinusIdError => new ErrorResponse(400, "Bad Request", exception.Message),
                _ => new ErrorResponse(500, "Internal Error", "Unexpected error")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
