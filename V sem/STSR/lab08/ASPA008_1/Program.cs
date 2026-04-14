using ASPA008_1.API;
using Microsoft.Extensions.FileProviders;
using ANC25_WEBAPI_DLL;
using ANC25_WEBAPI_DLL.Services;
using static ASPA008_1.Controllers.CelebritiesController;
using System.Text.Json;
using DAL_Celebrity_MSSQL;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddCelebritiesConfiguration();
        
        var connectionString = builder.Configuration.GetSection("Celebrities")["ConnectionString"]
            ?? ANC25_WEBAPI_DLL.AppConfig.ConnectionString
            ?? throw new InvalidOperationException("Connection string is not configured");
        
        Init.Execute(connectionString, delete: false, create: true);
        
        builder.AddCelebritiesServices();
        builder.Services.AddControllersWithViews();
        builder.Services.AddSingleton<CountryCodes>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var env = provider.GetRequiredService<IWebHostEnvironment>();

            var path = Path.Combine(env.ContentRootPath,
                config["Celebrities:ISO3166alpha2Path"]);

            return CountryCodes.LoadFromFile(path);
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Debug);


        var app = builder.Build();

        app.UseHttpsRedirection();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "Photos")),
            RequestPath = "/Photos"
        });

        app.UseRouting();
        app.UseMiddleware<CelebritiesError>();
        app.UseAuthorization();

        app.MapCelebrities();
        app.MapLifeevents();
        app.MapPhotoCelebrities();

        app.MapCelebritiesEndpoints();
        app.MapLifeEventsEndpoints();

        app.MapControllerRoute(
            name: "new_celebrity",
            pattern: "/0",
            defaults: new { Controller = "Celebrities", Action = "NewHumanForm" });

        app.MapControllerRoute(
            name: "celebrityDetails",
            pattern: "Celebrities/{id:int}",
            defaults: new { controller = "Celebrities", action = "Celebrities" });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Celebrities}/{action=Index}/{id?}");

        app.Run();
    }
}
