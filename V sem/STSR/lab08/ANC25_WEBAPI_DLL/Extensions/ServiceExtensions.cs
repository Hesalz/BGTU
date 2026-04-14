using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DAL_Celebrity;
using DAL_Celebrity_MSSQL;
using Microsoft.AspNetCore.Builder;
using ANC25_WEBAPI_DLL;

namespace ANC25_WEBAPI_DLL {
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCelebritiesServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CelebritiesConfig>(config.GetSection(CelebritiesConfig.SectionName));

            services.AddScoped<IRepository>(provider =>
                Repository.Create(provider.GetRequiredService<IOptions<CelebritiesConfig>>().Value.ConnectionString));

            services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptions<CelebritiesConfig>>().Value;
                return CountryCodesService.LoadFromFile(config.ISO3166alpha2Path);
            });

            services.AddSingleton(new CelebrityTitles
            {
                Title = "Celebrities Management",
                Head = "Celebrities Database",
                Copyright = $"© {DateTime.Now.Year}"
            });

            return services;
        }

        public static IApplicationBuilder UseCelebritiesErrorHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
