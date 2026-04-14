using DAL005;
using Microsoft.AspNetCore.Diagnostics;
using System.IO;
using Validation;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IRepository>(provider =>
        {
            Repository.JSONFileName = "Celebrities.json";
            return new Repository("Celebrities");
        });

        var app = builder.Build();

        app.UseExceptionHandler("/Celebrities/Error");

        // Создаём общую группу маршрутов
        RouteGroupBuilder api = app.MapGroup("/Celebrities");

        // POST -------------------------------------------------
        api.MapPost("/", (Celebrity celebrity, IRepository repository) =>
        {
            int? id = repository.addCelebrity(celebrity);
            if (id == null)
                throw new AddCelebrityException("POST /Celebrities error, id == null");
            if (repository.SaveChanges() <= 0)
                throw new SaveException("/Celebrities error, SaveChanges() <= 0");
            return new Celebrity((int)id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
        })
        .AddEndpointFilter<SurnameFilter>()        // объединены 2 проверки: на пустое и дублирующееся имя
        .AddEndpointFilter<PhotoExistFilter>();    // проверка наличия файла фотографии

        // GET -------------------------------------------------
        api.MapGet("/", (IRepository repository) => repository.getAllCelebrities());

        api.MapGet("/{id:int}", (int id, IRepository repository) =>
        {
            Celebrity? celebrity = repository.getCelebrityById(id);
            if (celebrity == null)
                throw new FoundByIdException($"Celebrity Id = {id}");
            return celebrity;
        });

        // PUT -------------------------------------------------
        api.MapPut("/{id:int}", (int id, Celebrity celebrity, IRepository repository) =>
        {
            int? Id = repository.updCelebrityById(id, celebrity);
            if (Id == null)
                throw new UpdateCelebrityException("PUT /Celebrities error, id == null");
            if (repository.SaveChanges() <= 0)
                throw new SaveException("/Celebrities error, SaveChanges() <= 0");
            return new Celebrity((int)Id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
        })
        .AddEndpointFilter<PutFilter>();

        // DELETE ----------------------------------------------
        api.MapDelete("/{id:int}", (int id, IRepository repository) =>
        {
            if (!repository.delCelebrityById(id))
                throw new DeleteCelebrityException($"DELETE /Celebrities error, Id = {id}");
            if (repository.SaveChanges() <= 0)
                throw new SaveException("/Celebrities error, SaveChanges() <= 0");
            return $"Celebrity with Id = {id} deleted";
        })
        .AddEndpointFilter<DeleteFilter>();

        // Глобальная обработка ошибок -------------------------
        app.Map("/Celebrities/Error", (HttpContext ctx) =>
        {
            Exception? ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
            IResult rc = Results.Problem(
                detail: "Internal server error",
                instance: app.Environment.EnvironmentName,
                title: "ASPA005",
                statusCode: 500);

            if (ex != null)
            {
                switch (ex)
                {
                    case UpdateCelebrityException:
                    case DeleteCelebrityException:
                    case FoundByIdException:
                        rc = Results.NotFound(ex.Message);
                        break;
                    case FileNotFoundException:
                        rc = Results.Problem(
                            title: "ASPA005/FileNotFound",
                            detail: ex.Message,
                            instance: app.Environment.EnvironmentName,
                            statusCode: 404);
                        break;
                    case SaveException:
                    case AddCelebrityException:
                        rc = Results.Problem(
                            title: $"ASPA005/{ex.GetType().Name}",
                            detail: ex.Message,
                            instance: app.Environment.EnvironmentName,
                            statusCode: 500);
                        break;
                    case ArgumentNullException:
                        rc = Results.Problem(
                            title: "ASPA005/Validation",
                            detail: ex.Message,
                            instance: app.Environment.EnvironmentName,
                            statusCode: 400);
                        break;
                }
            }
            return rc;
        });

        app.MapFallback((HttpContext ctx) =>
        {
            return Results.NotFound(new { error = $"path {ctx.Request.Path} not supported" });
        });

        app.Run();
    }
}
