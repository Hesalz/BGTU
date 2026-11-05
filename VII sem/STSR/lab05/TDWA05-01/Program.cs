using System.Text.Json;

static string GetArgument(string[] arguments, string name)
{
    for (var i = 0; i < arguments.Length; i++)
    {
        if (arguments[i].Equals($"--{name}", StringComparison.OrdinalIgnoreCase))
        {
            if (i + 1 >= arguments.Length)
                throw new ArgumentException($"Не указано значение параметра --{name}.");
            return arguments[i + 1];
        }
    }

    throw new ArgumentException($"Обязательный параметр --{name} не задан.");
}

string nick = GetArgument(args, "Nick");
if (!int.TryParse(GetArgument(args, "Port"), out int port) || port is < 1 or > 65535)
    throw new ArgumentException("Параметр --Port должен быть числом от 1 до 65535.");
if (!int.TryParse(GetArgument(args, "Delay"), out int delay) || delay < 0)
    throw new ArgumentException("Параметр --Delay должен быть целым неотрицательным числом.");

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var app = builder.Build();

app.MapMethods("/lb", new[] { "GET", "POST", "PUT", "DELETE" },
    async (HttpContext context) =>
    {
        double factor = context.Request.Method switch
        {
            "GET" => 1.0 / 3.0,
            "POST" => 2.0 / 3.0,
            "PUT" => 1.0,
            "DELETE" => 1.0 / 4.0,
            _ => 0
        };

        int actualDelay = (int)Math.Round(delay * factor, MidpointRounding.AwayFromZero);

        const string cookieName = "TDWA05-Session";

        if (!context.Request.Cookies.ContainsKey(cookieName))
        {
            context.Response.Cookies.Append(
                cookieName,
                Guid.NewGuid().ToString("N"),
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    MaxAge = TimeSpan.FromMinutes(30)
                });
        }

        await Task.Delay(actualDelay, context.RequestAborted);

        return Results.Json(new
        {
            Nick = nick,
            Method = context.Request.Method,
            Port = port,
            BaseDelay = delay,
            ActualDelay = actualDelay
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    });

app.MapGet("/", () => Results.Json(new
{
    Nick = nick,
    Port = port,
    Delay = delay,
    Status = "TDWA05-01 is running"
}));

Console.WriteLine($"TDWA05-01 запущен: Nick={nick}, Port={port}, Delay={delay} мс");
app.Run();
