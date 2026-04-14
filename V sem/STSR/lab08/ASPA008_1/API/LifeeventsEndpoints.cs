using DAL_Celebrity_MSSQL;
using ANC25_WEBAPI_DLL;

namespace ASPA008_1.API
{
    public static class LifeeventsEndpoints
    {
        public static void MapLifeevents(this WebApplication app)
        {
            app.MapGet("api/Lifeevents", () =>
            {
                try
                {
                    using (var db = new Repository(AppConfig.ConnectionString))
                    {
                        var lifeevents = db.GetAllLifeEvents();
                        return Results.Ok(lifeevents);
                    }
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                    title: "Database Error",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
                }

            });
        }
    }
}
