
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

string mode = Environment.GetEnvironmentVariable("BALANCER_MODE")
              ?? "RoundRobin";

if (mode.Equals("Weighted", StringComparison.OrdinalIgnoreCase))
{
    var app = builder.Build();

    string[] sequence =
    {
        "X", "X", "X", "X", "X",
        "Y", "Y", "Y",
        "Z", "Z"
    };

    int currentIndex = -1;

    var servers = new Dictionary<string, string>
    {
        ["X"] = "http://localhost:5101",
        ["Y"] = "http://localhost:5102",
        ["Z"] = "http://localhost:5103"
    };

    using var client = new HttpClient();

    app.MapMethods("/lb",
        new[] { "GET", "POST", "PUT", "DELETE" },
        async (HttpContext context) =>
        {
            int index = Interlocked.Increment(ref currentIndex);
            string nick = sequence[index % sequence.Length];

            var target = new Uri(
                servers[nick] + context.Request.Path +
                context.Request.QueryString);

            using var request = new HttpRequestMessage(
                new HttpMethod(context.Request.Method), target);

            if (context.Request.ContentLength > 0 ||
                context.Request.Headers.ContainsKey("Transfer-Encoding"))
            {
                request.Content = new StreamContent(context.Request.Body);
            }

            foreach (var header in context.Request.Headers)
            {
                if (!request.Headers.TryAddWithoutValidation(
                    header.Key, header.Value.ToArray()))
                {
                    request.Content?.Headers.TryAddWithoutValidation(
                        header.Key, header.Value.ToArray());
                }
            }

            using var response = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                context.RequestAborted);

            context.Response.StatusCode = (int)response.StatusCode;

            foreach (var header in response.Headers)
                context.Response.Headers[header.Key] = header.Value.ToArray();

            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                    context.Response.Headers[header.Key] = header.Value.ToArray();

                context.Response.Headers.Remove("transfer-encoding");

                await response.Content.CopyToAsync(
                    context.Response.Body, context.RequestAborted);
            }
        });

    app.Run("http://localhost:5000");
}
else
{
    string configFile = mode.Equals(
        "Sticky", StringComparison.OrdinalIgnoreCase)
        ? "ocelot.sticky.json"
        : "ocelot.roundrobin.json";

    builder.Configuration.AddJsonFile(
    Path.Combine(AppContext.BaseDirectory, configFile),
    optional: false,
    reloadOnChange: true);

    builder.Services.AddOcelot(builder.Configuration);

    var app = builder.Build();

    await app.UseOcelot();

    app.Run();
}
