using System.Text.Json.Serialization;

namespace ANC25_WEBAPI_DLL {
    public class Country
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("countryLabel")]
        public string Name { get; set; }
    }
}
