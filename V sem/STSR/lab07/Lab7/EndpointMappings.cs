using DAL_Celebrity_MSSQL.Interfaces;
using DAL_Celebrity_MSSQL.Models;
using Lab7.Configuration;
using Microsoft.Extensions.Options;

namespace Lab7
{
    public static class EndpointMappings
    {
        public static void MapCelebrities(this WebApplication app)
        {
            var celebrities = app.MapGroup("/api/Celebrities");

            celebrities.MapGet("/", (IRepository repo) => repo.GetAllCelebrities());

            celebrities.MapGet("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var celebrity = repo.GetCelebrityById(id);
                return celebrity is null
                    ? throw new NullReferenceException($"Celebrity с id {id} не найден")
                    : Results.Ok(celebrity);
            });

            celebrities.MapGet("/Lifeevents/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var celebrity = repo.GetCelebrityByLifeeventId(id);
                return celebrity is null
                    ? throw new NullReferenceException($"Celebrity с id {id} не найден")
                    : Results.Ok(celebrity);
            });

            celebrities.MapPost("/", (IRepository repo, Celebrity celebrity) =>
            {
                return !repo.AddCelebrity(celebrity)
                    ? throw new ArgumentException("Ошибка при добавлении celebrity")
                    : Results.Ok("Celebrity добавлен!");
            });

            celebrities.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                return !repo.DelCelebrity(id)
                    ? throw new NullReferenceException($"Celebrity по id {id} не найден")
                    : Results.Ok("Celebrity удален");
            });

            celebrities.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Celebrity celebrity) =>
            {
                return !repo.UpdCelebrity(id, celebrity)
                    ? throw new NullReferenceException($"Ошибка при обновлении Celebrity с id {id}")
                    : Results.Ok("Celebrity успешно обновлен");
            });
        }

        public static void MapPhotoCelebrities(this WebApplication app)
        {
            var celebrities = app.MapGroup("/api/Celebrities");

            celebrities.MapGet("/photo/{fname}", async (IOptions<CelebritiesConfig> iconfig, HttpContext context, string fname) =>
            {
                string photosFolder = iconfig.Value.PhotosFolder;
                string photoFullPath = Path.Combine(photosFolder, fname);

                if (!File.Exists(photoFullPath))
                    throw new ArgumentException($"{photoFullPath} does not exist.");

                await context.Response.SendFileAsync(photoFullPath);
            });
        }

        public static void MapLifeevents(this WebApplication app)
        {
            var lifeevents = app.MapGroup("/api/lifeevents");

            lifeevents.MapGet("/", (IRepository repo) => repo.GetAllLifeevents());

            lifeevents.MapGet("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var lifeevent = repo.GetLifeeventById(id);
                return lifeevent is null
                    ? throw new NullReferenceException($"Lifeevent с id {id} не найден")
                    : Results.Ok(lifeevent);
            });

            lifeevents.MapGet("/Celebrities/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var lifeeventsList = repo.GetLifeeventsByCelebrityId(id);
                return lifeeventsList.Count == 0
                    ? throw new ArgumentException($"Ни одного события для Celebrity id = {id} не найдено")
                    : Results.Ok(lifeeventsList);
            });

            lifeevents.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                return !repo.DelLifeevent(id)
                    ? throw new NullReferenceException($"Не удалось удалить lifeevent с id {id}")
                    : Results.Ok("Lifeevent удален успешно");
            });

            lifeevents.MapPost("/", (IRepository repo, Lifeevent lifeevent) =>
            {
                return !repo.AddLifeevent(lifeevent)
                    ? throw new ArgumentException("Ошибка при добавлении lifeevent")
                    : Results.Ok("Lifeevent добавлен!");
            });

            lifeevents.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Lifeevent lifeevent) =>
            {
                if (repo.GetCelebrityById(lifeevent.CelebrityId) is null)
                    throw new ArgumentException("Передан несуществующий CelebrityId");

                return !repo.UpdLifeevent(id, lifeevent)
                    ? throw new NullReferenceException($"Ошибка при обновлении Lifeevent с id {id}")
                    : Results.Ok("Lifeevent успешно обновлен");
            });
        }
    }

}
