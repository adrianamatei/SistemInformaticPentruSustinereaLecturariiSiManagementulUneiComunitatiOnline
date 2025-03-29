using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace AplicatieLicenta.Pages.Users
{
    public class VizualizarePDFModel : PageModel
    {
        private readonly AppDbContext _context;
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "Titlu";

        [BindProperty(SupportsGet = true)]
        public string Litera { get; set; }
        [BindProperty(SupportsGet = true)]
        public string CategorieSelectata { get; set; }


        public VizualizarePDFModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Carti> Carti { get; set; }

       

        public async Task<IActionResult> OnGetAsync()
        {
            var query = _context.Carti.Where(c => c.TipCarte == "PDF");
            if (!string.IsNullOrEmpty(CategorieSelectata))
            {
                query = query.Where(c => c.CategoriiVarsta.Any(cv => cv.Denumire == CategorieSelectata));
            }


            if (!string.IsNullOrEmpty(Litera))
            {
                query = query.Where(c => c.Titlu.StartsWith(Litera));
            }

            query = SortBy switch
            {
                "Autor" => query.OrderBy(c => c.Autor),
                _ => query.OrderBy(c => c.Titlu)
            };

            Carti = query.ToList();

            return Page();
        }
        public IActionResult OnGetCiteste(int id)
        {
            var esteLogat = HttpContext.Session.GetInt32("UserId").HasValue;

            if (!esteLogat)
            {

                HttpContext.Session.SetString("ReturnUrl", $"/Users/DetaliiPdf?id={id}");
                return RedirectToPage("/Login");
            }


            return RedirectToPage("/Users/DetaliiPdf", new { id });
        }

    }
}
