using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using AplicatieLicenta.Data;

namespace AplicatieLicenta.Pages.Admin
{
    public class CreeazaTestModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Titlul testului este obligatoriu")]
        public string TitluTest { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Selecteazã o carte !")]
        public int? CarteId { get; set; }

        [BindProperty]
        public IntrebareForm IntrebareCurenta { get; set; } = new();

        public List<SelectListItem> Carti { get; set; } = new();

        [BindProperty(SupportsGet = false)]
        public List<IntrebareForm> Intrebari { get; set; } = new();

        private readonly AppDbContext _context;

        public CreeazaTestModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Carti = _context.Carti
                .Select(c => new SelectListItem
                {
                    Value = c.IdCarte.ToString(),
                    Text = $"{c.Titlu} ({c.TipCarte})"
                })
                .ToList();
        }

        public IActionResult OnPostAdaugaIntrebare()
        {
            if (!ModelState.IsValid)
                return Page();

            Intrebari.Add(new IntrebareForm
            {
                Enunt = IntrebareCurenta.Enunt,
                Categorie = IntrebareCurenta.Categorie,
                Variante = new List<VariantaForm>
                {
                    new VariantaForm { Text = IntrebareCurenta.Varianta1, Corecta = IntrebareCurenta.Corecta == 1 },
                    new VariantaForm { Text = IntrebareCurenta.Varianta2, Corecta = IntrebareCurenta.Corecta == 2 },
                    new VariantaForm { Text = IntrebareCurenta.Varianta3, Corecta = IntrebareCurenta.Corecta == 3 },
                }
            });

            IntrebareCurenta = new IntrebareForm();
            ModelState.Clear();
            OnGet();
            return Page();
        }

        public IActionResult OnPostSalveazaTest()
        {
            // ?? Eliminãm erorile doar pentru IntrebareCurenta (care e goalã ?i irelevantã acum)
            ModelState.Remove("IntrebareCurenta.Enunt");
            ModelState.Remove("IntrebareCurenta.Categorie");
            ModelState.Remove("IntrebareCurenta.Varianta1");
            ModelState.Remove("IntrebareCurenta.Varianta2");
            ModelState.Remove("IntrebareCurenta.Varianta3");

            // Verificãm validitatea doar pentru titlu, carte ?i lista de întrebãri
            if (!ModelState.IsValid || Intrebari.Count == 0)
                return Page();

            var xml = new XElement("Test",
                new XElement("Titlu", TitluTest),
                new XElement("CarteId", CarteId),
                new XElement("Intrebari",
                    Intrebari.Select(i =>
                        new XElement("Intrebare",
                            new XElement("Enunt", i.Enunt),
                            new XElement("Categorie", i.Categorie),
                            new XElement("Variante",
                                i.Variante.Select(v =>
                                    new XElement("Varianta",
                                        new XAttribute("corecta", v.Corecta.ToString().ToLower()),
                                        v.Text
                                    )
                                )
                            )
                        )
                    )
                )
            );

            var fileName = $"wwwroot/teste/quiz_carte_{CarteId}.xml";
            Directory.CreateDirectory("wwwroot/teste");
            xml.Save(fileName);

            TitluTest = string.Empty;
            CarteId = null;
            Intrebari = new List<IntrebareForm>();

            TempData["Message"] = "Testul a fost salvat cu succes!";
            return RedirectToPage();
        }


        public class IntrebareForm
        {
            [Required(ErrorMessage = "Scrie un enunt pentru intrebare !")]
            public string Enunt { get; set; }

            [Required(ErrorMessage = "Alege o categorie !")]
            public string Categorie { get; set; }

            [Required(ErrorMessage = "Completeazã varianta 1 !")]
            public string Varianta1 { get; set; }

            [Required(ErrorMessage = "Completeazã varianta 2 !")]
            public string Varianta2 { get; set; }

            [Required(ErrorMessage = "Completeazã varianta 3 !")]
            public string Varianta3 { get; set; }

            public int Corecta { get; set; }

            public List<VariantaForm> Variante { get; set; } = new();
        }

        public class VariantaForm
        {
            [Required(ErrorMessage = "Completeazã textul variantei.")]
            public string Text { get; set; }
            public bool Corecta { get; set; }
        }
    }
}