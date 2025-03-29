using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AplicatieLicenta.Data; 
using Microsoft.EntityFrameworkCore;

namespace AplicatieLicenta.Pages
{
    public class ChatBotModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

       
        public List<(string Role, string Message)> ConversationHistory { get; set; } = new();

        public ChatBotModel(AppDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        [BindProperty]
        public string UserMessage { get; set; }

        public string BotResponse { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var hfToken = "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";
            var endpoint = "https://api-inference.huggingface.co/models/mistralai/Mistral-7B-Instruct-v0.1";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hfToken);

            
            var activitati = await _context.UsersActivity
                .OrderByDescending(a => a.Data)
                .Take(3)
                .Select(a => $"{a.UserId} - {a.Action}")
                .ToListAsync();

            
            string contextText = "";

            if (activitati.Any())
                contextText += "\n\nActivitate recenta :\n" + string.Join("\n", activitati);

         
            string prompt = !string.IsNullOrWhiteSpace(contextText)
                ? $"Esti un asistent care ofera sfaturi pentru combaterea analfabetismului functional. " +
                  $"Folosind urmatoarele informatii din sistem:\n\n{contextText}\n\nIntrebare: {UserMessage}\n\nRãspuns:"
                : $"Esti un asistent care ajuta cu sfaturi despre analfabetism functional. Intrebare: {UserMessage}";

            var payload = new { inputs = prompt };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Hugging Face response: " + responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    BotResponse = $"Eroare Hugging Face ({response.StatusCode}): {responseContent}";
                    return Page();
                }

                using JsonDocument doc = JsonDocument.Parse(responseContent);

                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var fullText = doc.RootElement[0].GetProperty("generated_text").ToString();

                    
                    var cleanText = fullText.Replace(prompt, "").Trim();

                    
                    BotResponse = cleanText;
                }
                else
                {
                    BotResponse = "Nu am primit un raspuns valid.";
                }

            }
            catch (Exception ex)
            {
                BotResponse = "Eroare la conectarea cu Hugging Face: " + ex.Message;
            }

            ConversationHistory.Add(("user", UserMessage));
            ConversationHistory.Add(("bot", BotResponse));

           
            UserMessage = "";

            return Page();
        }
    }
}
