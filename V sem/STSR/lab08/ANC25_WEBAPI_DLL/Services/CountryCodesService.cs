using System.Collections;
using System.Text.Json;
namespace ANC25_WEBAPI_DLL {
    public class CountryCodesService : IEnumerable<Country>
    {
        private List<Country> _countries;

        private CountryCodesService(List<Country> countries)
            => _countries = countries ?? new List<Country>();

        public static CountryCodesService LoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var countries = JsonSerializer.Deserialize<List<Country>>(json);
            return new CountryCodesService(countries);
        }

        public IEnumerator<Country> GetEnumerator() => _countries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
