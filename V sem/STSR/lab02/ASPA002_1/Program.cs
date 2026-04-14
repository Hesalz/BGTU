using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWelcomePage("/Aspnetcore");
app.MapGet("/Aspnetcore", () => "");

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();