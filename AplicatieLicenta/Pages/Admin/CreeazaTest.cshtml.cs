using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Xml.Serialization;

namespace AplicatieLicenta.Pages.Admin
{
    public class CreeazaTestModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreeazaTestModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Quiz Quiz { get; set; } = new();
        [BindProperty] public IntrebareQuiz IntrebareNoua { get; set; }
        [BindProperty] public List<VariantaRaspuns> VarianteRaspunsNou { get; set; } = new();

        public SelectList CartiDisponibile { get; set; }

        public List<IntrebareQuiz> IntrebariTemp
        {
            get
            {
                if (TempData.TryGetValue("Intrebari", out var json))
                    return JsonSerializer.Deserialize<List<IntrebareQuiz>>(json.ToString());
                return new List<IntrebareQuiz>();
            }
        }

        private void IncarcaCarti()
        {
            CartiDisponibile = new SelectList(_context.Carti.ToList(), "IdCarte", "Titlu");
        }

        public void OnGet()
        {
            IncarcaCarti();

            if (TempData.TryGetValue("QuizTitlu", out var titlu))
                Quiz.Titlu = titlu?.ToString();

            if (TempData.TryGetValue("QuizCarteId", out var carteIdStr) && int.TryParse(carteIdStr?.ToString(), out int id))
                Quiz.CarteId = id;

            TempData.Keep("Intrebari");
            TempData.Keep("QuizTitlu");
            TempData.Keep("QuizCarteId");
        }

        public IActionResult OnPostAdaugaIntrebare()
        {
            IncarcaCarti();
            var intrebari = IntrebariTemp;

            if (string.IsNullOrWhiteSpace(IntrebareNoua.Enunt))
            {
                ModelState.AddModelError("", "Enun?ul întrebãrii nu poate fi gol.");
                return Page();
            }

            IntrebareNoua.Variante = VarianteRaspunsNou.Where(v => !string.IsNullOrWhiteSpace(v.Text)).ToList();

            if (!IntrebareNoua.Variante.Any())
            {
                ModelState.AddModelError("", "Trebuie sã adaugi cel pu?in un rãspuns.");
                return Page();
            }

            intrebari.Add(IntrebareNoua);

            TempData["Intrebari"] = JsonSerializer.Serialize(intrebari);
            TempData["QuizTitlu"] = Quiz?.Titlu;
            TempData["QuizCarteId"] = Quiz?.CarteId;

            TempData.Keep("Intrebari");
            TempData.Keep("QuizTitlu");
            TempData.Keep("QuizCarteId");

            return RedirectToPage();
        }

        public IActionResult OnPostSalveazaTest()
        {
            IncarcaCarti();
            var intrebari = IntrebariTemp;

            var titlu = Request.Form["quizTitluHidden"];
            var carteIdStr = Request.Form["quizCarteIdHidden"];

            if (string.IsNullOrWhiteSpace(titlu) || string.IsNullOrWhiteSpace(carteIdStr) || !int.TryParse(carteIdStr, out int carteId))
            {
                ModelState.AddModelError("", "Datele testului nu au fost transmise corect.");
                return Page();
            }

            if (intrebari == null || !intrebari.Any())
            {
                ModelState.AddModelError("", "Nu ai adãugat nicio întrebare.");
                return Page();
            }

            Quiz = new Quiz
            {
                Titlu = titlu,
                CarteId = carteId,
                Intrebari = intrebari
            };

            _context.Quizuri.Add(Quiz);
            _context.SaveChanges();

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "teste");
            Directory.CreateDirectory(folderPath);

            string fileName = $"quiz_{Quiz.Id}.xml";
            string filePath = Path.Combine(folderPath, fileName);

            var quizXml = new QuizXml
            {
                Titlu = Quiz.Titlu,
                CarteTitlu = _context.Carti.FirstOrDefault(c => c.IdCarte == Quiz.CarteId)?.Titlu,
                Intrebari = Quiz.Intrebari.Select(i => new IntrebareXml
                {
                    Enunt = i.Enunt,
                    Categorie = i.Categorie,
                    Variante = i.Variante?.Select(v => new VariantaXml
                    {
                        Text = v.Text,
                        EsteCorect = v.EsteCorect
                    }).ToList() ?? new List<VariantaXml>()
                }).ToList()
            };

            var serializer = new XmlSerializer(typeof(QuizXml));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(stream, quizXml);
            }

            TempData.Clear();
            return RedirectToPage("/Admin/VizualizareTeste");
        }

        public class QuizXml
        {
            public string Titlu { get; set; }
            public string CarteTitlu { get; set; }
            public List<IntrebareXml> Intrebari { get; set; }
        }

        public class IntrebareXml
        {
            public string Enunt { get; set; }
            public string Categorie { get; set; }
            public List<VariantaXml> Variante { get; set; }
        }

        public class VariantaXml
        {
            public string Text { get; set; }
            public bool EsteCorect { get; set; }
        }
    }
}
