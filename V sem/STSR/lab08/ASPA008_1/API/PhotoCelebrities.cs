using DAL_Celebrity_MSSQL;
using ANC25_WEBAPI_DLL;
namespace ASPA008_1.API
{
    public static class PhotoCelebrities
    {
        public static void MapPhotoCelebrities(this WebApplication app)
        {
            app.MapGet("api/PhotoCelebrities", () =>
            {
                try
                {
                    using (var db = new Repository(AppConfig.ConnectionString))
                    {
                        List<string> paths = new List<string>();
                        var celebrities = db.GetAllCelebrities();
                        foreach (var c in celebrities) {
                            if (c.ReqPhotoPath != null) {
                                paths.Add(c.ReqPhotoPath);
                            }
                        }
                        return Results.Ok(paths);
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
