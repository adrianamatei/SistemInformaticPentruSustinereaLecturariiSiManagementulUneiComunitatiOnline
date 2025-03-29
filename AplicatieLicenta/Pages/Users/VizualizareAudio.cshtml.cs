using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AplicatieLicenta.Pages.Users
{
    public class VizualizareAudioModel : PageModel
    {
        private readonly AppDbContext _context;

        public VizualizareAudioModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Carti> Carti { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "Titlu";

        [BindProperty(SupportsGet = true)]
        public string Litera { get; set; }
        [BindProperty(SupportsGet = true)]
        public string CategorieSelectata { get; set; }

        public List<string> CategoriiDisponibile { get; set; } = new();


        public async Task<IActionResult> OnGetAsync()
        {
            var query = _context.Carti
                .Where(c => c.TipCarte == "Audio");

            if (!string.IsNullOrEmpty(Litera))
            {
                query = query.Where(c => c.Titlu.StartsWith(Litera));
            }
            if (!string.IsNullOrEmpty(CategorieSelectata))
            {
                query = query.Where(c => c.CategoriiVarsta.Any(cv => cv.Denumire == CategorieSelectata));
            }


            query = SortBy switch
            {
                "Autor" => query.OrderBy(c => c.Autor),
                "CategorieVarsta" => query.OrderBy(c => c.CategoriiVarsta.FirstOrDefault().Denumire),
                _ => query.OrderBy(c => c.Titlu)
            };

            Carti = query.ToList();
            return Page();
        }

        public IActionResult OnGetAsculta(int id)
        {
            var esteLogat = HttpContext.Session.GetInt32("UserId").HasValue;

            if (!esteLogat)
            {
               
                HttpContext.Session.SetString("ReturnUrl", $"/Users/DetaliiAudio?id={id}");
                return RedirectToPage("/Login");
            }


            return RedirectToPage("/Users/DetaliiAudio", new { id });
        }
    }
}
