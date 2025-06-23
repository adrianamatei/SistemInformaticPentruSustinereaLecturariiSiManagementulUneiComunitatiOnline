using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AplicatieLicenta.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "AIzaSyAbjQIRur51xsXFANt3lpFRQuUthdc22MI"; // Regenerată și protejată

        public GeminiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> TrimitePromptAsync(string mesaj)
        {
            var request = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = mesaj }
                        }
                    }
                }
            };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            string model = "models/gemini-1.5-pro";
            string url = $"https://generativelanguage.googleapis.com/v1/{model}:generateContent?key={_apiKey}";

            try
            {
                var response = await _httpClient.PostAsync(url, httpContent);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    if (jsonDoc.RootElement.TryGetProperty("candidates", out var candidates) &&
                        candidates.GetArrayLength() > 0 &&
                        candidates[0].TryGetProperty("content", out var content) &&
                        content.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0 &&
                        parts[0].TryGetProperty("text", out var textElement))
                    {
                        return textElement.GetString();
                    }

                    return "Am primit un raspuns de la Gemini, dar nu am putut extrage textul generat!";
                }

                return $"Eroare Gemini ({response.StatusCode}):\n{responseContent}";
            }
            catch (HttpRequestException httpEx)
            {
                return $"Eroare de conexiune la Gemini: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                return $"Eroare interna: {ex.Message}";
            }
        }
    }
}
