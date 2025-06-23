using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
namespace AplicatieLicenta.Services
{
    public class GoogleSearchServices
    {
        private readonly string apiKey = "AIzaSyDY4x1G1HsCjLNmujm6rND6OBnAUovfGDk";
        private readonly string cx = "c10d1ad09f4f2428a";
        private readonly HttpClient httpClient;

        public GoogleSearchServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<string>> SearchLinksAsync(string query)
        {
            var url = $"https://www.googleapis.com/customsearch/v1?key={apiKey}&cx={cx}&q={Uri.EscapeDataString(query)}";

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<string>();

            var content = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content);

            var links = jsonDoc.RootElement
                .GetProperty("items")
                .EnumerateArray()
                .Select(item => item.GetProperty("link").GetString())
                .ToList();

            return links;
        }
    }
}
