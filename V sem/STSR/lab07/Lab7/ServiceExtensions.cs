using DAL_Celebrity_MSSQL.Interfaces;
using DAL_Celebrity_MSSQL.Repositories;
using Lab7.Configuration;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata.Ecma335;

namespace Lab7
{
    public static class ServiceExtensions
    {
        public static WebApplicationBuilder AddCelebritiesConfiguration(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile("Celebrities.config.json", optional: false, reloadOnChange: true);
            builder.Services.Configure<CelebritiesConfig>(builder.Configuration.GetSection("Celebrities"));
            return builder;
        }
        public static WebApplicationBuilder AddCelebritiesServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IRepository, Repository>((IServiceProvider p) =>
            {
                CelebritiesConfig config = p.GetRequiredService<IOptions<CelebritiesConfig>>().Value;
                return new Repository(config.ConnectionString);
            });
            return builder;
        }
    }
}
