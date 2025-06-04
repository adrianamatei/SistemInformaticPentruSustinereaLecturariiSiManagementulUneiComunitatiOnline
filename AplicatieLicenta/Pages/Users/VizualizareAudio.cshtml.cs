using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        [BindProperty(SupportsGet = true)]
        public string GenSelectat { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Autor { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Titlu { get; set; }

        public List<string> GenuriDisponibile { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var query = _context.Carti
                .Where(c => c.TipCarte == "Audio")
                .Include(c => c.Genuri)
                .Include(c => c.CategoriiVarsta)
                .AsQueryable();

            if (!string.IsNullOrEmpty(Litera))
            {
                query = query.Where(c => c.Titlu.StartsWith(Litera));
            }

            if (!string.IsNullOrEmpty(CategorieSelectata))
            {
                query = query.Where(c => c.CategoriiVarsta.Any(cv => cv.Denumire == CategorieSelectata));
            }

            if (!string.IsNullOrEmpty(GenSelectat))
            {
                query = query.Where(c => c.Genuri.Any(g => g.Denumire == GenSelectat));
            }

            if (!string.IsNullOrEmpty(Autor))
            {
                query = query.Where(c => c.Autor.ToLower().Contains(Autor.ToLower()));
            }

            if (!string.IsNullOrEmpty(Titlu))
            {
                query = query.Where(c => c.Titlu.ToLower().Contains(Titlu.ToLower()));
            }

            query = SortBy switch
            {
                "Autor" => query.OrderBy(c => c.Autor),
                "CategorieVarsta" => query.OrderBy(c => c.CategoriiVarsta.Select(cv => cv.Denumire).FirstOrDefault()),
                "Gen" => query.OrderBy(c => c.Genuri.Select(g => g.Denumire).FirstOrDefault()),
                _ => query.OrderBy(c => c.Titlu)
            };

            Carti = await query.ToListAsync();

            GenuriDisponibile = await _context.Genuri.Select(g => g.Denumire).ToListAsync();

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
