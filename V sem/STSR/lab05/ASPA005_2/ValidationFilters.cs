using DAL005;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Validation
{
    // --- Общий фильтр для проверки фамилии ---
    public class SurnameFilter : IEndpointFilter
    {
        public static IRepository? repository;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var celebrity = context.GetArgument<Celebrity>(0);
            var repo = repository ?? context.HttpContext.RequestServices.GetRequiredService<IRepository>();

            if (string.IsNullOrWhiteSpace(celebrity.Surname) || celebrity.Surname.Length < 2)
            {
                context.HttpContext.Response.StatusCode = 409;
                return Results.Json(new { error = "POST /Celebrities error, Surname is wrong" });
            }

            var duplicates = repo.getCelebritiesBySurname(celebrity.Surname);
            if (duplicates.Length > 0)
            {
                context.HttpContext.Response.StatusCode = 409;
                return Results.Json(new { error = "POST /Celebrities error, Surname is doubled" });
            }

            return await next(context);
        }
    }

    // --- Проверка наличия файла фото ---
    public class PhotoExistFilter : IEndpointFilter
    {
        public static IRepository? repository;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var celebrity = context.GetArgument<Celebrity>(0);
            var repo = repository ?? context.HttpContext.RequestServices.GetRequiredService<IRepository>();

            if (!string.IsNullOrWhiteSpace(celebrity.PhotoPath))
            {
                string fileName = Path.GetFileName(celebrity.PhotoPath);
                string fullPath = Path.Combine(repo.BasePath, fileName);
                if (!File.Exists(fullPath))
                {
                    context.HttpContext.Response.Headers.Append("X-Celebrity", $"NotFound-{fileName}");
                }
            }

            return await next(context);
        }
    }

    // --- Фильтр для PUT ---
    public class PutFilter : IEndpointFilter
    {
        public static IRepository? repository;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var celebrity = context.GetArgument<Celebrity>(1);
            if (string.IsNullOrWhiteSpace(celebrity.Surname))
            {
                context.HttpContext.Response.StatusCode = 409;
                return Results.Json(new { error = "PUT /Celebrities error, Surname is wrong" });
            }
            return await next(context);
        }
    }

    // --- Фильтр для DELETE ---
    public class DeleteFilter : IEndpointFilter
    {
        public static IRepository? repository;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            int id = context.GetArgument<int>(0);
            var repo = repository ?? context.HttpContext.RequestServices.GetRequiredService<IRepository>();
            var celebrity = repo.getCelebrityById(id);
            if (celebrity == null)
            {
                context.HttpContext.Response.StatusCode = 404;
                return Results.Json(new { error = $"DELETE /Celebrities error, Id = {id} not found" });
            }

            return await next(context);
        }
    }
}
