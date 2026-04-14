using DAL005;
using Microsoft.AspNetCore.Diagnostics;
using System.IO;

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

        app.MapGet("/Celebrities", (IRepository repository) => repository.getAllCelebrities());

        app.MapGet("/Celebrities/{id:int}", (int id, IRepository repository) =>
        {
            Celebrity? celebrity = repository.getCelebrityById(id);
            if (celebrity == null)
                throw new FoundByIdException($"Celebrity Id = {id}");
            return celebrity;
        });

        app.MapPost("/Celebrities", (Celebrity celebrity, IRepository repository, HttpContext ctx) =>
        {
            int? id = repository.addCelebrity(celebrity);
            if (id == null)
                throw new AddCelebrityException("POST /Celebrities error, id == null");
            if (repository.SaveChanges() <= 0)
                throw new SaveException("/Celebrities error, SaveChanges() <= 0");

            return Results.Ok(new Celebrity((int)id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath));
        })
        // Фильтр 1: Проверка наличия обязательных полей Firstname и Surname
        .AddEndpointFilter(async (context, next) =>
        {
            var celebrity = context.GetArgument<Celebrity>(0);

            if (celebrity == null)
            {
                return Results.Problem(
                    title: "Internal Server Error",
                    detail: "Celebrity object is null",
                    statusCode: 500
                );
            }

            if (string.IsNullOrWhiteSpace(celebrity.Surname) || celebrity.Surname.Length < 2)
            {
                return Results.Problem(
                    title: "Conflict",
                    detail: "Surname is required and must be at least 2 characters long",
                    statusCode: 409
                );
            }

            return await next(context);
        })
        // Фильтр 2: проверка уникальности surname
        .AddEndpointFilter(async (context, next) =>
        {
            var celebrity = context.GetArgument<Celebrity>(0);

            if (celebrity == null)
            {
                return Results.Problem(
                    title: "Internal Server Error",
                    detail: "Celebrity object is null",
                    statusCode: 500
                );
            }

            var repository = context.HttpContext.RequestServices.GetService<IRepository>();
            if (repository == null)
            {
                return Results.Problem(
                    title: "Internal Server Error",
                    detail: "Repository is not available",
                    statusCode: 500
                );
            }

            var duplicates = repository.getCelebritiesBySurname(celebrity.Surname);
            if (duplicates.Length > 0)
            {
                return Results.Problem(
                    title: "Conflict",
                    detail: $"Surname '{celebrity.Surname}' already exists",
                    statusCode: 409
                );
            }

            return await next(context);
        })
        // Фильтр 3: проверка файла
        .AddEndpointFilter(async (context, next) =>
        {
            var celebrity = context.GetArgument<Celebrity>(0);

            // Проверка на null
            if (celebrity == null)
            {
                return Results.Problem(
                    title: "Internal Server Error",
                    detail: "Celebrity object is null",
                    statusCode: 500
                );
            }

            var repository = context.HttpContext.RequestServices.GetService<IRepository>();
            if (repository == null)
            {
                return Results.Problem(
                    title: "Internal Server Error",
                    detail: "Repository is not available",
                    statusCode: 500
                );
            }

            if (!string.IsNullOrWhiteSpace(celebrity.PhotoPath))
            {
                string fileName = Path.GetFileName(celebrity.PhotoPath);
                string filePath = Path.Combine(repository.BasePath, fileName);

                if (!File.Exists(filePath))
                {
                    context.HttpContext.Response.Headers["X-celebrity"] = $"Not found = {fileName}";

                    return Results.Problem(
                        title: "File Not Found",
                        detail: $"Photo file not found: {fileName}",
                        statusCode: 404
                    );
                }
            }

            return await next(context);
        });

        app.MapDelete("/Celebrities/{id:int}", (int id, IRepository repository) =>
        {
            if (!repository.delCelebrityById(id))
                throw new DeleteCelebrityException($"Delete by Id:DELETE /Celebrities error, Id = {id}");
            if (repository.SaveChanges() <= 0)
                throw new SaveException("/Celebrities error, SaveChanges() <= 0");
            return Results.Ok($"Celebrity with Id = {id} deleted");
        });

        app.MapPut("/Celebrities/{id:int}", (int id, Celebrity celebrity, IRepository repository) =>
        {
            int? Id = repository.updCelebrityById(id, celebrity);
            if (Id == null)
                throw new UpdateCelebrityException("Put /Celebrities error, id == null");
            if (repository.SaveChanges() <= 0)
                throw new SaveException("/Celebrities error, SaveChanges() <= 0");
            return Results.Ok(new Celebrity((int)Id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath));
        });

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
                    case BadHttpRequestException:
                        rc = Results.BadRequest(ex.Message);
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