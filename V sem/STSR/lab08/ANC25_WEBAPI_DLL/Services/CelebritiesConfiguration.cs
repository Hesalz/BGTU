namespace ANC25_WEBAPI_DLL
{
    public static class CelebritiesConfigurationService
    {
        public static WebApplicationBuilder AddCelebritiesConfiguration(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile(
                path: "Celebrities.config.json",
                optional: false,
                reloadOnChange: true);

            var configSection = builder.Configuration.GetSection(CelebritiesConfig.SectionName);
            if (!configSection.Exists())
            {
                throw new InvalidOperationException(
                    $"Section '{CelebritiesConfig.SectionName}' not found in configuration.");
            }

            builder.Services.Configure<CelebritiesConfig>(configSection);

            var connectionString = configSection["ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string is not configured in Celebrities.config.json");
            }

            AppConfig.ConnectionString = connectionString;

            return builder;
        }
    }
}
