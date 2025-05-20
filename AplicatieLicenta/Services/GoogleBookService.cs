using System.Net.Http.Json;
using AplicatieLicenta.Models;
using Microsoft.Extensions.Options;
namespace AplicatieLicenta.Services
{
    public class GoogleBookService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GoogleBookService(HttpClient httpClient, IOptions<GoogleBooksSettings> settings)
        {
            _httpClient = httpClient;
            _apiKey = settings.Value.ApiKey;
        }

        public async Task<string> CautaDescriereCarteAsync(string titlu)
        {
            var url = $"https://www.googleapis.com/books/v1/volumes?q=intitle:{Uri.EscapeDataString(titlu)}&key={_apiKey}";
            var response = await _httpClient.GetFromJsonAsync<GoogleBooksResponse>(url);
            var carte = response?.Items?.FirstOrDefault();
            return carte?.VolumeInfo?.Description ?? "Nu am găsit o descriere pentru această carte.";
        }
    }
}
