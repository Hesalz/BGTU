using Microsoft.AspNetCore.Http;

static string GetArgument(string[] args, string name, string? defaultValue = null)
{
    for (var i = 0; i < args.Length; i++)
    {
        if (args[i].Equals($"--{name}", StringComparison.OrdinalIgnoreCase))
        {
            if (i + 1 >= args.Length)
                throw new ArgumentException($"Не указано значение параметра --{name}.");
            return args[i + 1];
        }
    }

    if (defaultValue is not null)
        return defaultValue;

    throw new ArgumentException($"Обязательный параметр --{name} не задан.");
}

string nick = GetArgument(args, "Nick");
if (!int.TryParse(GetArgument(args, "Port"), out int port) || port is < 1 or > 65535)
    throw new ArgumentException("Параметр --Port должен быть числом от 1 до 65535.");

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

app.MapMethods("/A", new[] { "GET", "POST", "PUT", "DELETE" },
    (HttpContext context) =>
    {
        return Results.Json(new
        {
            Nick = nick,
            Method = context.Request.Method
        }, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        });
    });

app.MapGet("/", () => Results.Json(new
{
    Nick = nick,
    Port = port,
    Status = "TDWA04-01 is running"
}));

Console.WriteLine($"TDWA04-01 запущен: Nick={nick}, Port={port}");
app.Run();
