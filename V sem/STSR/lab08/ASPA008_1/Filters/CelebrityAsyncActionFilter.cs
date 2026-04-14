using DAL_Celebrity;
using DAL_Celebrity_MSSQL;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using System.Linq;
using System;

namespace ASPA008_1.Filters
{
    public class InfoAsyncActionFilter : Attribute, IAsyncActionFilter
    {
        public const string Wikipedia = "WIKI";
        public const string Facebook = "FACE";

        private readonly string _infoType;

        public InfoAsyncActionFilter(string infoType = "")
        {
            _infoType = infoType.ToUpper();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var repo = context.HttpContext.RequestServices.GetService<IRepository>();
            if (repo == null)
            {
                await next();
                return;
            }

            if (!context.ActionArguments.TryGetValue("id", out var idObj) || !(idObj is int id) || id <= 0)
            {
                await next();
                return;
            }

            var celebrity = repo.GetCelebrityById(id);
            if (celebrity == null)
            {
                await next();
                return;
            }

            if (_infoType.Contains(Wikipedia))
            {
                var wikiReferences = await WikiInfoCelebrity.GetReferences(celebrity.FullName);
                context.HttpContext.Items[Wikipedia] = wikiReferences;
                
                System.Diagnostics.Debug.WriteLine($"Wikipedia references found: {wikiReferences?.Count ?? 0} for {celebrity.FullName}");
            }

            if (_infoType.Contains(Facebook))
            {
                context.HttpContext.Items[Facebook] = GetFromFacebook(celebrity.FullName);
            }

            await next();
        }

        private static string GetFromFacebook(string fullName)
        {
            return "Info from Face";
        }
    }
    public class WikiInfoCelebrity
    {
        private readonly HttpClient _client;
        private readonly Dictionary<string, string> _wikiReferences;
        private readonly string _wikiURI;

        private WikiInfoCelebrity(string fullName)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("User-Agent", "CelebritiesApp/1.0 (https://example.com/contact)");
            _client.Timeout = TimeSpan.FromSeconds(10);
            _wikiReferences = new Dictionary<string, string>();
            _wikiURI = $"https://en.wikipedia.org/w/api.php?action=opensearch&search={Uri.EscapeDataString(fullName)}&limit=5&format=json";
        }

        public static async Task<Dictionary<string, string>> GetReferences(string fullName)
        {
            var info = new WikiInfoCelebrity(fullName);
            
            try
            {
                HttpResponseMessage message = await info._client.GetAsync(info._wikiURI);

                if (message.IsSuccessStatusCode)
                {
                    var jsonString = await message.Content.ReadAsStringAsync();
                    
                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        // Wikipedia OpenSearch API возвращает массив: [searchTerm, [titles...], [descriptions...], [urls...]]
                        using (JsonDocument doc = JsonDocument.Parse(jsonString))
                        {
                            var root = doc.RootElement;
                            if (root.ValueKind == System.Text.Json.JsonValueKind.Array && root.GetArrayLength() >= 4)
                            {
                                var titlesArray = root[1];
                                var urlsArray = root[3];

                                if (titlesArray.ValueKind == System.Text.Json.JsonValueKind.Array && 
                                    urlsArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                                {
                                    var titles = titlesArray.EnumerateArray().Select(e => e.GetString()).ToList();
                                    var urls = urlsArray.EnumerateArray().Select(e => e.GetString()).ToList();

                                    if (titles != null && urls != null && titles.Count == urls.Count)
                                    {
                                        for (int i = 0; i < titles.Count; i++)
                                        {
                                            if (!string.IsNullOrEmpty(titles[i]) && !string.IsNullOrEmpty(urls[i]))
                                            {
                                                info._wikiReferences[titles[i]] = urls[i];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем выполнение
                System.Diagnostics.Debug.WriteLine($"Error fetching Wikipedia references: {ex.Message}");
            }
            finally
            {
                info._client?.Dispose();
            }

            return info._wikiReferences;
        }
    }
}
