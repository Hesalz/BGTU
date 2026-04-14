using Dall003;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var picturesPath = Path.Combine(Directory.GetCurrentDirectory(), "Photo");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(picturesPath),
    RequestPath = "/Photo"
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(picturesPath),
    RequestPath = "/Photo"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(picturesPath),
    RequestPath = "/Celebrities/download",
    OnPrepareResponse = ctx =>
    {
        var fileName = Path.GetFileName(ctx.File.PhysicalPath);
        ctx.Context.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
    }
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(picturesPath),
    RequestPath = "/Celebrities/download"
});

Repository.JSONFileName = "Celebrities.json";
using (IRepository repository = new Repository("Celebrities"))
{
    app.MapGet("/Celebrities", () => repository.getAllCelebrities());
    app.MapGet("/Celebrities/{id:int}", (int id) => repository.getCelebrityById(id));
    app.MapGet("/Celebrities/BySurname/{surname}", (string surname) => repository.getCelebritiesBySurname(surname));
    app.MapGet("/Celebrities/PhotoPathById/{id:int}", (int id) => repository.getPhotoPathId(id));
    app.MapGet("/", () => "Hello World!");

    app.Run();
}
