using DAL_Celebrity_MSSQL;
using DAL_Celebrity_MSSQL.Data;
using Lab7;
using Lab7.Configuration;
using Lab7.Middlewares;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.AddCelebritiesConfiguration();
builder.AddCelebritiesServices();
builder.Services.AddRazorPages(o =>
{
    o.Conventions.AddPageRoute("/Celebrities", "/");
    o.Conventions.AddPageRoute("/NewCelebrity", "/0");
    o.Conventions.AddPageRoute("/Celebrity", "/Celebrities/{id:int:min(1)}");
    o.Conventions.AddPageRoute("/Celebrity", "/{id:int:min(1)}");

});

var app = builder.Build();


var celebritiesOptions = builder.Configuration
    .GetSection("Celebrities")
    .Get<CelebritiesConfig>();

try
{
    var init = new Init(celebritiesOptions.ConnectionString);
    Init.Execute(delete: false, create: true);
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при инициализации базы данных: {ex.Message}");
}

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        celebritiesOptions.PhotosFolder),
    RequestPath = "/Photos"
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();

app.MapCelebrities();
app.MapLifeevents();
app.MapPhotoCelebrities();

app.Run();
