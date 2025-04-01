using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AplicatieLicenta.Pages.Users
{
    public class VizualizarePDFModel : PageModel
    {
        private readonly AppDbContext _context;

        public VizualizarePDFModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "Titlu";

        [BindProperty(SupportsGet = true)]
        public string Litera { get; set; }

        [BindProperty(SupportsGet = true)]
        public string CategorieSelectata { get; set; }

        [BindProperty(SupportsGet = true)]
        public string GenSelectat { get; set; }

        public List<Carti> Carti { get; set; } = new();

        public List<string> GenuriDisponibile { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var query = _context.Carti
                .Where(c => c.TipCarte == "PDF")
                .Include(c => c.Genuri)
                .Include(c => c.CategoriiVarsta)
                .AsQueryable();

            if (!string.IsNullOrEmpty(CategorieSelectata))
            {
                query = query.Where(c => c.CategoriiVarsta.Any(cv => cv.Denumire == CategorieSelectata));
            }

            if (!string.IsNullOrEmpty(GenSelectat))
            {
                query = query.Where(c => c.Genuri.Any(g => g.Denumire == GenSelectat));
            }

            if (!string.IsNullOrEmpty(Litera))
            {
                query = query.Where(c => c.Titlu.StartsWith(Litera));
            }

            query = SortBy switch
            {
                "Autor" => query.OrderBy(c => c.Autor),
                "Gen" => query.OrderBy(c => c.Genuri.Select(g => g.Denumire).FirstOrDefault()),
                "CategorieVarsta" => query.OrderBy(c => c.CategoriiVarsta.Select(cv => cv.Denumire).FirstOrDefault()),
                _ => query.OrderBy(c => c.Titlu)
            };

            Carti = await query.ToListAsync();

            // Pentru dropdown genuri
            GenuriDisponibile = await _context.Genuri.Select(g => g.Denumire).ToListAsync();

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
