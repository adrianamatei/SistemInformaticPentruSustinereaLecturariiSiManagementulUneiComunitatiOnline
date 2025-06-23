using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace AplicatieLicenta.Pages.Users
{
    public class TesteCarteModel : PageModel
    {
        private readonly AppDbContext _context;

        public TesteCarteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Dictionary<int, int> RaspunsuriUtilizator { get; set; } = new();

        public List<IntrebareQuiz> Intrebari { get; set; } = new();
        public int CarteId { get; set; }

        public async Task<IActionResult> OnGetAsync(int idCarte)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            var quiz = await _context.Quizuri
                .Include(q => q.Intrebari)
                    .ThenInclude(i => i.Variante)
                .FirstOrDefaultAsync(q => q.CarteId == idCarte);

            if (quiz != null && quiz.Intrebari != null && quiz.Intrebari.Count > 0)
            {
                var rezultat = await _context.RezultateQuiz
                    .FirstOrDefaultAsync(r => r.UserId == userId.Value && r.QuizId == quiz.Id);

                if (rezultat != null && rezultat.Scor >= 80)
                {
                    TempData["Mesaj"] = "Ai promovat deja acest test si nu il mai poti relua !";
                    return RedirectToPage("/Users/VizualizareTeste");
                }

                Intrebari = quiz.Intrebari.ToList();
                CarteId = idCarte;
                return Page();
            }
            else
            {
                string titlu = await GetTitluCurat(idCarte);
                string fisierXml = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "teste", $"Test_{titlu}.xml");

                if (!System.IO.File.Exists(fisierXml))
                    return NotFound("Testul nu a fost gasit in baza de date sau ca fisier XML !");

                var testXml = XmlTestParser.Parse(fisierXml);
                int quizId = await SalveazaTestDinXmlInBazaDeDate(testXml);

                var quizNou = await _context.Quizuri
                    .Include(q => q.Intrebari)
                        .ThenInclude(i => i.Variante)
                    .FirstOrDefaultAsync(q => q.Id == quizId);

                if (quizNou == null)
                    return NotFound("Testul nu a putut fi salvat corect !");

                Intrebari = quizNou.Intrebari.ToList();
                CarteId = idCarte;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int idCarte)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            var quiz = await _context.Quizuri
                .Include(q => q.Intrebari)
                    .ThenInclude(i => i.Variante)
                .FirstOrDefaultAsync(q => q.CarteId == idCarte);

            if (quiz == null)
            {
                return NotFound("Testul nu a fost gasit in baza de date !");
            }

            var intrebari = quiz.Intrebari.ToList();
            int raspunsuriCorecte = 0;

            for (int i = 0; i < intrebari.Count; i++)
            {
                if (Request.Form.TryGetValue($"RaspunsuriUtilizator[{i}]", out var val)
                    && int.TryParse(val, out int indexVar))
                {
                    var varianteLista = intrebari[i].Variante.ToList(); 

                    if (indexVar >= 0 && indexVar < varianteLista.Count && varianteLista[indexVar].EsteCorect)
                    {
                        raspunsuriCorecte++;
                    }
                }
            }

            int scor = (int)Math.Round((raspunsuriCorecte * 100.0) / intrebari.Count);

            _context.RezultateQuiz.Add(new RezultatQuiz
            {
                UserId = userId.Value,
                QuizId = quiz.Id,
                Scor = scor,
                Data = DateTime.Now
            });
          
            var carte = await _context.Carti.FindAsync(idCarte);
            string titluCarte = carte?.Titlu ?? "necunoscut";

            _context.UsersActivity.Add(new UsersActivity
            {
                UserId = userId.Value,
                Action = $"A finalizat testul pentru cartea '{titluCarte}'",
                Data = scor >= 80 ? "Test promovat" : "Test picat",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
            await ActualizeazaScorUtilizator(userId.Value);

            if (scor >= 80)
                TempData["Success"] = "Felicitari!  Ai promovat testul!";
            else
                TempData["Error"] = "Ai obtinut sub 80% , trebuie sa reiei testul !";

            return RedirectToPage("/Users/StartUser");
        }

        private async Task<string> GetTitluCurat(int idCarte)
        {
            var carte = await _context.Carti.FindAsync(idCarte);
            if (carte == null) return "Carte";

            var titlu = carte.Titlu.Replace(" ", "_");
            return new string(titlu.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());
        }

        private async Task<int> SalveazaTestDinXmlInBazaDeDate(TestModel testXml)
        {
            var quiz = new Quiz
            {
                Titlu = testXml.Titlu,
                CarteId = testXml.CarteId,
                Intrebari = new List<IntrebareQuiz>()
            };

            foreach (var intrebareXml in testXml.Intrebari)
            {
                var intrebare = new IntrebareQuiz
                {
                    Enunt = intrebareXml.Text,
                    Categorie = intrebareXml.Categorie,
                    Variante = intrebareXml.Variante?.Select(v => new VariantaRaspuns
                    {
                        Text = v.Text,
                        EsteCorect = v.Corecta
                    }).ToList() ?? new List<VariantaRaspuns>()
                };

                quiz.Intrebari.Add(intrebare);
            }

            _context.Quizuri.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz.Id;
        }

        private async Task ActualizeazaScorUtilizator(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUtilizator == userId);
            if (user == null) 
                return;

            var testePromovate = await _context.RezultateQuiz
                .Where(r => r.UserId == userId && r.Scor >= 80)
                .ToListAsync();

            int scorTotal = 0;
            foreach (var rezultat in testePromovate)
            {
                scorTotal = scorTotal+ CalculeazaPunctePeTest(rezultat.Scor);
            }

            user.ScorTotal = scorTotal;
            await _context.SaveChangesAsync();
        }

        private int CalculeazaPunctePeTest(int scor)
        {
            if (scor >= 100)
                return 20;
            if (scor >= 90) 
                return 15;
            return 10;
        }
    }

    public static class XmlTestParser
    {
        public static TestModel Parse(string filePath)
        {
            var doc = XDocument.Load(filePath);

            var test = new TestModel
            {
                Titlu = doc.Root.Element("Titlu")?.Value,
                CarteId = int.Parse(doc.Root.Element("CarteId")?.Value ?? "0"),
                Intrebari = doc.Root.Element("Intrebari")?.Elements("Intrebare").Select(i => new IntrebareModel
                {
                    Text = i.Element("Text")?.Value,
                    Categorie = i.Element("Categorie")?.Value,
                    Variante = i.Element("Variante")?.Elements("Varianta").Select(v => new VariantaModel
                    {
                        Text = v.Value,
                        Corecta = (v.Attribute("corecta")?.Value == "true")
                    }).ToList() ?? new List<VariantaModel>()
                }).ToList() ?? new List<IntrebareModel>()
            };

            return test;
        }
    }

    public class TestModel
    {
        public string Titlu { get; set; }
        public int CarteId { get; set; }
        public List<IntrebareModel> Intrebari { get; set; } = new();
    }

    public class IntrebareModel
    {
        public string Text { get; set; }
        public List<VariantaModel> Variante { get; set; } = new();
        public string Categorie { get; set; }
    }

    public class VariantaModel
    {
        public string Text { get; set; }
        public bool Corecta { get; set; }
    }
}
