using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AplicatieLicenta.Services;
using AplicatieLicenta.Models;
using AplicatieLicenta.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AplicatieLicenta.Pages
{
    public class ChatBotModel : PageModel
    {
        private readonly GoogleBookService _googleBookService;
        private readonly AppDbContext _context;
        private readonly GeminiService _geminiService;

        public ChatBotModel(GoogleBookService googleBookService, AppDbContext context, GeminiService geminiService)
        {
            _googleBookService = googleBookService;
            _context = context;
            _geminiService = geminiService;
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

                var mapariGenuri = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "aventura", "Aventura" },
                    { "actiune", "Aventura" },
                    { "fantasy", "Fantezie" },
                    { "fantezie", "Fantezie" },
                    { "povesti", "Basm" },
                    { "basm", "Basm" },
                    { "fictiune", "Fictiune" }
                };

                string genGasit = mapariGenuri.Keys.FirstOrDefault(k => UserMessage.ToLower().Contains(k));
                string genReal = genGasit != null ? mapariGenuri[genGasit] : null;

                if (!string.IsNullOrEmpty(genReal))
                {
                    int? userId = HttpContext.Session.GetInt32("UserId");

                    if (userId.HasValue)
                    {
                        var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUtilizator == userId.Value);

                        if (user != null && !string.IsNullOrEmpty(user.CategorieVarsta))
                        {
                            string categorie = user.CategorieVarsta.ToLower();

                            var carti = await _context.Carti
                                .Include(c => c.Genuri)
                                .Include(c => c.CategoriiVarsta)
                                .Where(c => c.Genuri.Any(g => g.Denumire.ToLower() == genReal.ToLower()))
                                .Where(c => c.CategoriiVarsta.Any(cv => cv.Denumire.ToLower() == categorie))
                                .Take(5)
                                .ToListAsync();

                            if (carti.Any())
                            {
                                raspuns = $"Iti recomand aceste carti din genul {genReal} potrivite pentru varsta ta ({categorie}):\n" +
                                          string.Join("\n", carti.Select(c => $"- {c.Titlu} de {c.Autor}"));
                                goto finalizeaza;
                            }
                        }
                    }

                    var cartiGen = await _context.Carti
                        .Include(c => c.Genuri)
                        .Where(c => c.Genuri.Any(g => g.Denumire.ToLower() == genReal.ToLower()))
                        .Take(5)
                        .ToListAsync();

                    if (cartiGen.Any())
                    {
                        raspuns = $"Iti recomand aceste carti din genul {genReal}, disponibile pe site:\n" +
                                  string.Join("\n", cartiGen.Select(c => $"- {c.Titlu} de {c.Autor}"));
                        goto finalizeaza;
                    }
                }

                if (inModRecomandare == "true")
                {
                    if (string.IsNullOrWhiteSpace(tempVarsta))
                    {
                        HttpContext.Session.SetString("TempVarsta", UserMessage);
                        raspuns = "Si ce gen de carti iti plac? (ex: aventura, mister, fantasy)";
                        goto finalizeaza;
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
                            raspuns = $"Iti recomand urmatoarele carti din genul {tempGen} pentru {tempVarsta} ani:\n" +
                                      string.Join("\n", cartiRecomandate.Select(c => $"- {c.Titlu} de {c.Autor}"));
                        }
                        else
                        {
                            raspuns = $"Momentan nu am gasit carti din genul {tempGen} potrivite pentru {tempVarsta} ani.";
                        }

                        HttpContext.Session.Remove("InModRecomandare");
                        HttpContext.Session.Remove("TempVarsta");
                        HttpContext.Session.Remove("TempGen");
                        goto finalizeaza;
                    }
                }
                else if (UserMessage.ToLower().Contains("recomanzi"))
                {
                    int? userId = HttpContext.Session.GetInt32("UserId");

                    if (userId.HasValue)
                    {
                        var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUtilizator == userId.Value);

                        if (user == null || string.IsNullOrWhiteSpace(user.CategorieVarsta))
                        {
                            raspuns = "Nu am putut determina categoria ta de varsta. Te rog actualizeaza-ti profilul.";
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
                                raspuns = $"Iti recomand aceste carti pentru categoria ta de varsta ({denumireCategorie}):\n" +
                                          string.Join("\n", cartiRecomandate.Select(c => $"- {c.Titlu} de {c.Autor}"));
                            }
                            else
                            {
                                raspuns = $"Nu am gasit momentan carti pentru categoria ta de varsta ({denumireCategorie}).";
                            }
                        }
                        goto finalizeaza;
                    }
                    else
                    {
                        HttpContext.Session.SetString("InModRecomandare", "true");
                        HttpContext.Session.Remove("TempVarsta");
                        HttpContext.Session.Remove("TempGen");
                        raspuns = "Ca sa iti pot face o recomandare, cati ani ai?";
                        goto finalizeaza;
                    }
                }
                else if (UserMessage.ToLower().StartsWith("despre ce e"))
                {
                    string titlu = UserMessage.Replace("despre ce e", "", StringComparison.OrdinalIgnoreCase).Trim();
                    raspuns = await _googleBookService.CautaDescriereCarteAsync(titlu);
                    goto finalizeaza;
                }
                else
                {
                    raspuns = await _geminiService.TrimitePromptAsync(UserMessage);

                    if (string.IsNullOrWhiteSpace(raspuns) || raspuns.Contains("nu am reusit") || raspuns.Length < 15)
                    {
                        string fallbackDescriere = await _googleBookService.CautaDescriereCarteAsync(UserMessage);

                        if (!string.IsNullOrWhiteSpace(fallbackDescriere))
                        {
                            string promptTraducere = $"Te rog, traduci in romana urmatorul text:\n{fallbackDescriere}";
                            raspuns = await _geminiService.TrimitePromptAsync(promptTraducere);
                        }
                        else
                        {
                            raspuns = "Imi pare rau, nu am reusit sa gasesc un raspuns relevant.";
                        }
                    }
                }

            finalizeaza:
                if (!string.IsNullOrEmpty(raspuns))
                {
                    raspuns = raspuns.Replace("**", "").Replace("*", "•");
                    raspuns = Regex.Replace(raspuns, @"\s*•\s*•", "•");
                    raspuns = Regex.Replace(raspuns, @"(?<=[.!?]) +", "$0\n");
                    raspuns = Regex.Replace(raspuns, " {2,}", " ");
                }

                ConversationHistory.Add(new ChatMessage("bot", raspuns));

                int? userIdSave = HttpContext.Session.GetInt32("UserId");
                if (userIdSave.HasValue)
                {
                    _context.UsersActivity.Add(new UsersActivity
                    {
                        UserId = userIdSave.Value,
                        Action = "Intrebare in chatbot",
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
