using System.Net.Http.Json;
using System.Text.Json;

Test test = new Test();
Console.WriteLine("--- /A ------------------------------------------------------------------------------------");
await test.ExecuteGET<int?>("http://localhost:5050/A/3", (int? x, int? y, int status) => (x == 3 && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteGET<int?>("http://localhost:5050/A/-3", (int? x, int? y, int status) => (x == -3 && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteGET<int?>("http://localhost:5050/A/118", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecutePOST<int?>("http://localhost:5050/A/5", (int? x, int? y, int status) => (x == 5 && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecutePOST<int?>("http://localhost:5050/A/-5", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecutePOST<int?>("http://localhost:5050/A/118", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecutePUT<int?>("http://localhost:5050/A/2/3", (int? x, int? y, int status) => (x == 2 && y == 3 && status == 200) ? Test.OK : Test.NOK);

await test.ExecutePUT<int?>("http://localhost:5050/A/0/3", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecutePUT<int?>("http://localhost:5050/A/25/-3", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecutePUT<int?>("http://localhost:5050/A/0/-3", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecuteDELETE<int?>("http://localhost:5050/A/1-99", (int? x, int? y, int status) => (x == 1 && y == 99 && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteDELETE<int?>("http://localhost:5050/A/99-1", (int? x, int? y, int status) => (x == 99 && y == 1 && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteDELETE<int?>("http://localhost:5050/A/-1-25", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecuteDELETE<int?>("http://localhost:5050/A/-1--25", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecuteDELETE<int?>("http://localhost:5050/A/25-101", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

Console.WriteLine("--- /B ---------------------------------------------------------------------------------------");
await test.ExecuteGET<float?>("http://localhost:5050/B/2.5", (float? x, float? y, int status) => (x == 2.5f && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteGET<float?>("http://localhost:5050/B/2", (float? x, float? y, int status) => (x == 2f && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteGET<float?>("http://localhost:5050/B/2X", (float? x, float? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecutePOST<float?>("http://localhost:5050/B/2.5/3.2", (float? x, float? y, int status) => (x == 2.5f && y == 3.2f && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteDELETE<float?>("http://localhost:5050/B/2.5-3.2", (float? x, float? y, int status) => (x == 2.5f && y == 3.2f && status == 200) ? Test.OK : Test.NOK);

Console.WriteLine("--- /C ------------------------------------------------------------------------------------");
await test.ExecuteGET<bool?>("http://localhost:5050/C/2.5", (bool? x, bool? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecuteGET<bool?>("http://localhost:5050/C/true", (bool? x, bool? y, int status) => (x == true && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecutePOST<bool?>("http://localhost:5050/C/true,false", (bool? x, bool? y, int status) => (x == true && y == false && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteDELETE<bool?>("http://localhost:5050/C/true,false", (bool? x, bool? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

Console.WriteLine("--- /D ------------------------------------------------------------------------------------");
await test.ExecuteGET<DateTime?>("http://localhost:5050/D/2025-02-25", (DateTime? x, DateTime? y, int status) => (x == new DateTime(2025, 02, 25) && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteGET<DateTime?>("http://localhost:5050/D/2025-02-29", (DateTime? x, DateTime? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecuteGET<DateTime?>("http://localhost:5050/D/2024-02-29", (DateTime? x, DateTime? y, int status) => (x == new DateTime(2024, 02, 29) && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteGET<DateTime?>("http://localhost:5050/D/2025-02-25T19:25", (DateTime? x, DateTime? y, int status) => (x == new DateTime(2025, 02, 25, 19, 25, 0) && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecutePOST<DateTime?>("http://localhost:5050/D/2025-02-25%7C2025-03-25", (DateTime? x, DateTime? y, int status) => (x == new DateTime(2025, 02, 25) && y == new DateTime(2025, 03, 25) && status == 200) ? Test.OK : Test.NOK);

await test.ExecutePUT<DateTime?>("http://localhost:5050/D/2025-02-25T19:25%7C2025-03-25T20:30", (DateTime? x, DateTime? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

Console.WriteLine("--- /E ------------------------------------------------------------------------------------");
await test.ExecuteGET<string?>("http://localhost:5050/E/12-bis", (string? x, string? y, int status) => (x == "bis" && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteGET<string?>("http://localhost:5050/E/11-bis", (string? x, string? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecuteGET<string?>("http://localhost:5050/E/12-777", (string? x, string? y, int status) => (x == "777" && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteGET<string?>("http://localhost:5050/E/12-", (string? x, string? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecutePUT<string?>("http://localhost:5050/E/abcd", (string? x, string? y, int status) => (x == "abcd" && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecutePUT<string?>("http://localhost:5050/E/abcd123", (string? x, string? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecutePUT<string?>("http://localhost:5050/E/a", (string? x, string? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecutePUT<string?>("http://localhost:5050/E/123456", (string? x, string? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecutePUT<string?>("http://localhost:5050/E/aabbccddeeffgghh", (string? x, string? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

Console.WriteLine("--- /F ---------------------------------------------------------------------------------");
await test.ExecuteGET<string?>("http://localhost:5050/F/smw@belstu.by", (string? x, string? y, int status) => (x == "smw@belstu.by" && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteGET<string?>("http://localhost:5050/F/xxx@yyyy.by", (string? x, string? y, int status) => (x == "xxx@yyyy.by" && y == null && status == 200) ? Test.OK : Test.NOK);

await test.ExecuteGET<string?>("http://localhost:5050/F/xxx@yyyy.ru", (string? x, string? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecuteGET<string?>("http://localhost:5050/F/xxxyyyy.by", (string? x, string? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

await test.ExecuteGET<string?>("http://localhost:5050/F/xxx@yyyy", (string? x, string? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

class Test
{
    class Answer<T>
    {
        public T? x { get; set; } = default;
        public T? y { get; set; } = default;
        public string? message { get; set; } = null;
    }

    public const string OK = "OK";
    public const string NOK = "NOK";

    private readonly HttpClient _client = new HttpClient();

    public async Task ExecuteGET<T>(string path, Func<T?, T?, int, string> result)
    {
        await ResultPrint<T>("GET", path, await _client.GetAsync(path), result);
    }

    public async Task ExecutePOST<T>(string path, Func<T?, T?, int, string> result)
    {
        await ResultPrint<T>("POST", path, await _client.PostAsync(path, null), result);
    }

    public async Task ExecutePUT<T>(string path, Func<T?, T?, int, string> result)
    {
        await ResultPrint<T>("PUT", path, await _client.PutAsync(path, null), result);
    }

    public async Task ExecuteDELETE<T>(string path, Func<T?, T?, int, string> result)
    {
        await ResultPrint<T>("DELETE", path, await _client.DeleteAsync(path), result);
    }

    private async Task ResultPrint<T>(string method, string path, HttpResponseMessage response, Func<T?, T?, int, string> result)
    {
        int status = (int)response.StatusCode;
        try
        {
            var answer = await response.Content.ReadFromJsonAsync<Answer<T>>() ?? new Answer<T>();
            string r = result(answer.x, answer.y, status);
            Console.WriteLine($" {r}: {method} {path}, status = {status}, x = {answer.x}, y = {answer.y}, m = {answer.message}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($" {Test.NOK}: {method} {path} failed to connect: {ex.Message}");
        }
        catch (JsonException ex)
        {
            string r = result(default, default, status);
            Console.WriteLine($" {r}: {method} {path}, status = {status}, x = {null}, y = {null}, m = {ex.Message}");
        }
    }
}