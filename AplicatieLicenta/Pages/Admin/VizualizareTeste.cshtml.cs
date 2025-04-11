using Microsoft.AspNetCore.Mvc.RazorPages;
using AplicatieLicenta.Models;
using System.Xml.Linq;

namespace AplicatieLicenta.Pages.Admin
{
    public class VizualizareTesteModel : PageModel
    {
        private readonly AplicatieLicenta.Data.AppDbContext _context;

        public VizualizareTesteModel(AplicatieLicenta.Data.AppDbContext context)
        {
            _context = context;
        }

        public List<TestViewModel> Teste { get; set; } = new();

        public void OnGet()
        {
            string folder = "wwwroot/teste";
            if (!Directory.Exists(folder)) return;

            var fisiere = Directory.GetFiles(folder, "*.xml");

            foreach (var path in fisiere)
            {
                try
                {
                    var xml = XElement.Load(path);
                    string titlu = xml.Element("Titlu")?.Value ?? "Fãrã titlu";
                    int carteId = int.Parse(xml.Element("CarteId")?.Value ?? "0");
                    string carteTitlu = _context.Carti.FirstOrDefault(c => c.IdCarte == carteId)?.Titlu ?? "Necunoscutã";

                    Teste.Add(new TestViewModel
                    {
                        Titlu = titlu,
                        CarteTitlu = carteTitlu,
                        FileName = Path.GetFileName(path)
                    });
                }
                catch
                {
                    // ignori fi?iere corupte
                }
            }
        }

        public class TestViewModel
        {
            public string Titlu { get; set; }
            public string CarteTitlu { get; set; }
            public string FileName { get; set; } // pentru editare
        }
    }
}
