using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AplicatieLicenta.Services;
using AplicatieLicenta.Models;
using AplicatieLicenta.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AplicatieLicenta.Pages
{
    public class ChatBotModel : PageModel
    {
        private readonly GoogleBookService _googleBookService;
        private readonly AppDbContext _context;

        public ChatBotModel(GoogleBookService googleBookService, AppDbContext context)
        {
            _googleBookService = googleBookService;
            _context = context;
        }

        [BindProperty]
        public string UserMessage { get; set; }

        public List<ChatMessage> ConversationHistory { get; set; } = new();

        public void OnGet()
        {
            var storedConversation = HttpContext.Session.GetString("StoredConversation");
            if (!string.IsNullOrWhiteSpace(storedConversation))
            {
                ConversationHistory = JsonSerializer.Deserialize<List<ChatMessage>>(storedConversation);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var storedConversation = HttpContext.Session.GetString("StoredConversation");
            if (!string.IsNullOrWhiteSpace(storedConversation))
                ConversationHistory = JsonSerializer.Deserialize<List<ChatMessage>>(storedConversation);

            string raspuns = string.Empty;
            UserMessage = UserMessage?.Trim();

            if (!string.IsNullOrWhiteSpace(UserMessage))
            {
                ConversationHistory.Add(new ChatMessage("user", UserMessage));

                string inModRecomandare = HttpContext.Session.GetString("InModRecomandare");
                string tempVarsta = HttpContext.Session.GetString("TempVarsta");
                string tempGen = HttpContext.Session.GetString("TempGen");

                // Recomandare pe baza inputului utilizatorului (vârstã + gen)
                if (inModRecomandare == "true")
                {
                    if (string.IsNullOrWhiteSpace(tempVarsta))
                    {
                        HttpContext.Session.SetString("TempVarsta", UserMessage);
                        raspuns = "Si ce gen de cãrti îti plac? (ex: aventurã, mister, fantasy)";
                    }
                    else if (string.IsNullOrWhiteSpace(tempGen))
                    {
                        HttpContext.Session.SetString("TempGen", UserMessage);
                        tempGen = UserMessage;
                        int.TryParse(tempVarsta, out int varstaInt);
                        string genCautat = tempGen.Trim().ToLower();

                        var toateCartile = await _context.Carti
                            .Include(c => c.Genuri)
                            .Include(c => c.CategoriiVarsta)
                            .ToListAsync();

                        var cartiRecomandate = toateCartile
                            .Where(c => c.Genuri.Any(g => g.Denumire.ToLower().Contains(genCautat)))
                            .Where(c => c.CategoriiVarsta.Any(cv =>
                            {
                                var den = cv.Denumire.ToLower();
                                if (den.Contains("sub")) return varstaInt < 8;
                                if (den.Contains("peste")) return varstaInt > 14;

                                var parts = den.Split(new[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                return parts.Length == 2 &&
                                       int.TryParse(parts[0], out int min) &&
                                       int.TryParse(parts[1], out int max) &&
                                       varstaInt >= min && varstaInt <= max;
                            }))
                            .Take(5)
                            .ToList();

                        if (cartiRecomandate.Any())
                        {
                            raspuns = $"Îti recomand urmãtoarele cãrti din genul {tempGen} pentru {tempVarsta} ani:\n" +
                                      string.Join("\n", cartiRecomandate.Select(c => $"- {c.Titlu} de {c.Autor}"));
                        }
                        else
                        {
                            raspuns = $"Momentan nu am gãsit cãrti din genul {tempGen} potrivite pentru {tempVarsta} ani.";
                        }

                        HttpContext.Session.Remove("InModRecomandare");
                        HttpContext.Session.Remove("TempVarsta");
                        HttpContext.Session.Remove("TempGen");
                    }
                }
                // Recomandare automatã pentru utilizator autentificat
                else if (UserMessage.ToLower().Contains("recomanzi"))
                {
                    int? userId = HttpContext.Session.GetInt32("UserId");

                    if (userId.HasValue)
                    {
                        var user = await _context.Users
                            .FirstOrDefaultAsync(u => u.IdUtilizator == userId.Value);

                        if (user == null || string.IsNullOrWhiteSpace(user.CategorieVarsta))
                        {
                            raspuns = "Nu am putut determina categoria ta de varsta. Te rog actualizeazã-ti profilul.";
                        }
                        else
                        {
                            string denumireCategorie = user.CategorieVarsta.ToLower();

                            var cartiRecomandate = await _context.Carti
                                .Include(c => c.CategoriiVarsta)
                                .Where(c => c.CategoriiVarsta.Any(cv =>
                                    cv.Denumire != null && cv.Denumire.ToLower() == denumireCategorie))
                                .Take(5)
                                .ToListAsync();

                            if (cartiRecomandate.Any())
                            {
                                raspuns = $"Îti recomand aceste cãrti pentru categoria ta de vârstã ({denumireCategorie}):\n" +
                                          string.Join("\n", cartiRecomandate.Select(c => $"- {c.Titlu} de {c.Autor}"));
                            }
                            else
                            {
                                raspuns = $"Nu am gãsit momentan cãrti pentru categoria ta de vârstã ({denumireCategorie}).";
                            }
                        }
                    }
                    else
                    {
                        HttpContext.Session.SetString("InModRecomandare", "true");
                        HttpContext.Session.Remove("TempVarsta");
                        HttpContext.Session.Remove("TempGen");
                        raspuns = "Ca sã îti pot face o recomandare, câti ani ai?";
                    }
                }
                // Descriere carte prin Google Books
                else if (UserMessage.ToLower().StartsWith("despre ce e"))
                {
                    string titlu = UserMessage.Replace("despre ce e", "", StringComparison.OrdinalIgnoreCase).Trim();
                    raspuns = await _googleBookService.CautaDescriereCarteAsync(titlu);
                }
                // Mesaj fallback
                else
                {
                    raspuns = "Îmi poti adresa întrebãri despre o carte anume sau îti pot recomanda ceva potrivit pentru tine.";
                }

                // Salvare mesaj bot în conversa?ie
                ConversationHistory.Add(new ChatMessage("bot", raspuns));

                // Salvare în tabelul UsersActivity
                int? userIdSave = HttpContext.Session.GetInt32("UserId");
                if (userIdSave.HasValue)
                {
                    _context.UsersActivity.Add(new UsersActivity
                    {
                        UserId = userIdSave.Value,
                        Action = "Întrebare în chatbot",
                        Data = UserMessage,
                        Timestamp = DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                }
            }

            UserMessage = "";
            HttpContext.Session.SetString("StoredConversation", JsonSerializer.Serialize(ConversationHistory));
            return Page();
        }

        public class ChatMessage
        {
            public string Role { get; set; }
            public string Message { get; set; }

            public ChatMessage() { }

            public ChatMessage(string role, string message)
            {
                Role = role;
                Message = message;
            }
        }
    }
}
