namespace ANC25_WEBAPI_DLL {
    public class CelebritiesConfig
    {
        public const string SectionName = "Celebrities";

        public string PhotosFolderRequestPath { get; set; }
        public string PhotosFolder { get; set; }
        public string ConnectionString { get; set; }
        public string ISO3166alpha2Path { get; set; }
    }
    public static class AppConfig
    {
        public static string ConnectionString { get; set; }
    }
}
