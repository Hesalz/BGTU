using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "Neumann.html" }
});

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Picture")),
    RequestPath = "/Picture"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwRoot")),
    RequestPath = "/static"
});

app.Run();