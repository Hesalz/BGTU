using Microsoft.AspNetCore.Builder;
using DAL_Celebrity;
using DAL_Celebrity_MSSQL;
using ANC25_WEBAPI_DLL;
namespace ASPA008_1.API
{
    public static class CelebritiesEndpoints
    {
        public static void MapCelebrities(this WebApplication app) {
            app.MapGet("api/Celebrities", () =>
            {
                    try
                    {
                        using (var db = new Repository(AppConfig.ConnectionString))
                        {
                            var celebrities = db.GetAllCelebrities();
                            return Results.Ok(celebrities);
                        }
                    }
                    catch (Exception ex) {
                        return Results.Problem(
                        title: "Database Error",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                    } 
            });
        }
    }
}
