using Microsoft.AspNetCore.HttpLogging;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args); // Создаём билдера приложения с загрузкой конфигурации

        builder.Services.AddHttpLogging(options =>        // Регистрируем сервис логирования HTTP-запросов
        {
            options.LoggingFields = HttpLoggingFields.All; // Логировать всё: заголовки, тело запроса/ответа и др.
        });

        var app = builder.Build(); // Строим веб-приложение с учётом всех сервисов и настроек

        app.UseHttpLogging();      // Включаем middleware для логирования HTTP

        app.MapGet("/", () => "First ASPA)");              // GET / — возвращаем простую строку
        app.MapGet("/test", (HttpContext ctx) =>          // GET /test — возвращаем время и число заголовков
            $"Test endpoint called at {DateTime.Now}. Headers: {ctx.Request.Headers.Count}");

        app.Run(); // Запускаем приложение, оно начинает слушать HTTP-запросы
    }
}
