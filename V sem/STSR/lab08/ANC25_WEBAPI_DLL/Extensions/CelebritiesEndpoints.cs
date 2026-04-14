using DAL_Celebrity_MSSQL;
using DAL_Celebrity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System.IO;
namespace ANC25_WEBAPI_DLL{
    public static class CelebritiesEndpoints
    {
        public static IEndpointRouteBuilder MapCelebritiesEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/celebrities");

            group.MapGet("/", (IRepository repo) =>
            {
                var celebrities = repo.GetAllCelebrities();
                return celebrities ?? throw new ArgumentNullException("No celebrities found");
            });

            //group.MapGet("/{id:int:min(1)}", (IRepository repo, int id) =>
            //{
            //    var celebrity = repo.GetCelebrityById(id);
            //    return celebrity ?? throw new ArgumentException($"Celebrity {id} not found");
            //});

            group.MapGet("/LifeEvents/{eventId:int:min(1)}", (IRepository repo, int eventId) =>
            {
                var celebrity = repo.GetCelebrityByLifeEventId(eventId);
                return celebrity ?? throw new ArgumentException($"Celebrity for event {eventId} not found");
            });

            group.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var celebrity = repo.GetCelebrityById(id);
                if (celebrity == null)
                {
                    throw new ArgumentException($"Celebrity Id = {id} not found");
                }
                repo.DeleteCelebrity(id);
                return Results.Ok($"Celebrity {id} deleted");
            });

            group.MapPost("/", (IRepository repo, Celebrity celebrity) =>
            {
                repo.AddCelebrity(celebrity);
                var newId = repo.GetCelebrityByName(celebrity.FullName);
                if (newId == -1)
                {
                    throw new InvalidOperationException("Failed to add celebrity");
                }
                return Results.Created($"/celebrities/{newId}", celebrity);
            });

            group.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Celebrity celebrity) =>
            {
                var existingCelebrity = repo.GetCelebrityById(id);
                if (existingCelebrity == null)
                {
                    throw new ArgumentException($"Celebrity {id} not found");
                }
                repo.UpdateCelebrity(id, celebrity);
                return Results.Ok(celebrity);
            });

            group.MapGet("/photo/{fileName}", async (IOptions<CelebritiesConfig> config, IWebHostEnvironment env, string fileName) =>
            {
                var photosPath = Path.Combine(env.ContentRootPath, "Photos");
                var filePath = Path.Combine(photosPath, fileName);

                if (!File.Exists(filePath))
                {
                    return Results.NotFound($"File {fileName} not found");
                }

                var contentType = "image/jpeg";
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".bmp" => "image/bmp",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                return Results.File(filePath, contentType);
            });

            return builder;
        }
    }
}
