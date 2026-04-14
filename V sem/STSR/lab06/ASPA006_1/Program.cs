using ASPA006_1;
using Azure.Identity;
using DAL_Celebrity_MSSQL;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata.Ecma335;

internal class Program
{
    private static void Main(string[] args)
    {
        Init.Execute();
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.AddJsonFile("Celebrities.config.json", optional: false, reloadOnChange: true);
        builder.Services.Configure<CelebritiesConfig>(builder.Configuration.GetSection("Celebrities"));
        builder.Services.AddScoped<IRepository, Repository>((IServiceProvider p) =>
        {
            CelebritiesConfig config = p.GetRequiredService<IOptions<CelebritiesConfig>>().Value;
            return new Repository(config.ConnectionString);
        });

        var app = builder.Build();

        app.UseExceptionHandler("/Errors");

        app.UseStaticFiles();

        app.MapGet("/", () => Results.Redirect("/Index.html"));

        // --------- Знаменитости (Celebrities) -----------------------------------------------------
        var celebrities = app.MapGroup("/api/Celebrities");
        celebrities.MapGet("/", (IRepository repo) => repo.GetAllCelebrities());
        celebrities.MapGet("/{id:int:min(1)}", (IRepository repo, int id) => repo.GetCelebrityById(id));
        celebrities.MapGet("/Lifeevents/{id:int:min(1)}", (IRepository repo, int id) => repo.GetCelebrityByLifeeventId(id));
        celebrities.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) => {
            var c = repo.GetCelebrityById(id);
            if (c != null)
            {
                repo.DelCelebrity(id);
                return Results.Json(c);
            }
            throw new FileNotFoundException($"404002:Celebrity Id = {id}");
        });
        celebrities.MapPost("/", (IRepository repo, Celebrity celebrity) => {
            if (repo.AddCelebrity(celebrity))
            {
                return Results.Json(celebrity);
            }
            throw new Exception($"ERROR: AddCelebrity");
        });
        celebrities.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Celebrity celebrity) => {
            var c = repo.GetCelebrityById(id);
            if (c != null)
            {
                if (repo.UpdCelebrity(id, celebrity) != null)
                {
                    return Results.Json(repo.GetCelebrityById(id));
                }
                throw new Exception($"ERROR: UpdCelebrity");
            }
            throw new FileNotFoundException($"404003:Celebrity Id = {id}");
        });
        celebrities.MapGet("/photo/{fname}", async (HttpContext context, string fname) =>
        {
            var iconfig = builder.Configuration.GetSection("Celebrities").Get<CelebritiesConfig>();
            var path = Path.Combine(iconfig.PhotoFolder, fname);

            if (!File.Exists(path))
            {
                return Results.NotFound();
            }
            return Results.File(path, "image/jpeg");
        });

        celebrities.MapGet("/photopath/{fname}", async (HttpContext context, string fname) =>
        {
            var iconfig = builder.Configuration.GetSection("Celebrities").Get<CelebritiesConfig>();
            var path = Path.Combine(iconfig.PhotoFolder, fname);

            if (!File.Exists(path))
            {
                return Results.NotFound();
            }
            var url = $"{context.Request.Scheme}://{context.Request.Host}/api/Celebrities/photo/{fname}";
            return Results.Json(url);
        });

        // --------- События (Lifeevents) -----------------------------------------------------
        var lifeevents = app.MapGroup("/api/Lifeevents");
        lifeevents.MapGet("/", (IRepository repo) => repo.GetAllLifeevents());
        lifeevents.MapGet("/{id:int:min(1)}", (IRepository repo, int id) => repo.GetLifeeventById(id));
        lifeevents.MapGet("/Celebrities/{id:int:min(1)}", (IRepository repo, int id) => repo.GetLifeeventsByCelebrityId(id));
        lifeevents.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) => {
            var l = repo.GetLifeeventById(id);
            if (l != null)
            {
                repo.DelLifeevent(id);
                return Results.Json(l);
            }
            throw new FileNotFoundException($"404004:Lifeevent Id = {id}");
        });
        lifeevents.MapPost("/", (IRepository repo, Lifeevent lifeevent) => {
            if (repo.AddLifeevent(lifeevent))
            {
                return Results.Json(lifeevent);
            }
            throw new Exception($"ERROR: AddLifeevent");
        });
        lifeevents.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Lifeevent lifeevent) => {
            var l = repo.GetLifeeventById(id);
            if (l != null)
            {
                if (repo.UpdLifeevent(id, lifeevent))
                {
                    return Results.Json(repo.GetLifeeventById(id));
                }
                throw new Exception($"ERROR: UpdLifeevent");
            }
            throw new FileNotFoundException($"404004:Lifeevent Id = {id}");
        });

        app.MapFallback((HttpContext ctx) =>
        {
            return Results.NotFound(new { message = $"path {ctx.Request.Path.Value} not supported" });
        });

        app.Map("/Errors", (HttpContext ctx) =>
        {
            Exception? ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
            IResult rc = Results.Problem(detail: "Panic", instance: app.Environment.EnvironmentName, title: "ASPA006", statusCode: 500);
            if (ex != null)
            {
                if (ex is FileNotFoundException) rc = Results.Problem(title: "Not Found", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 404);
            }
            return rc;
        });

        app.Run("http://localhost:5204");
    }
}