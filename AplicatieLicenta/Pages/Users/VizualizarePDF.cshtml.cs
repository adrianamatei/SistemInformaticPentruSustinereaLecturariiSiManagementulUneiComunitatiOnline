using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;

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

        [BindProperty(SupportsGet = true)]
        public string Autor { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Titlu { get; set; }

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

           
            var cartiList = await query.ToListAsync();

           
            if (!string.IsNullOrEmpty(Autor))
            {
                var autorFaraDiacritice = RemoveDiacritics(Autor.ToLower());
                cartiList = cartiList.Where(c =>
                    RemoveDiacritics(c.Autor.ToLower()).Contains(autorFaraDiacritice)).ToList();
            }

            if (!string.IsNullOrEmpty(Titlu))
            {
                var titluFaraDiacritice = RemoveDiacritics(Titlu.ToLower());
                cartiList = cartiList.Where(c =>
                    RemoveDiacritics(c.Titlu.ToLower()).Contains(titluFaraDiacritice)).ToList();
            }

            if (!string.IsNullOrEmpty(Litera))
            {
                cartiList = cartiList.Where(c => c.Titlu.StartsWith(Litera)).ToList();
            }

            
            cartiList = SortBy switch
            {
                "Autor" => cartiList.OrderBy(c => c.Autor).ToList(),
                "Gen" => cartiList.OrderBy(c => c.Genuri.Select(g => g.Denumire).FirstOrDefault()).ToList(),
                "CategorieVarsta" => cartiList.OrderBy(c => c.CategoriiVarsta.Select(cv => cv.Denumire).FirstOrDefault()).ToList(),
                _ => cartiList.OrderBy(c => c.Titlu).ToList()
            };

            Carti = cartiList;

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
       



        private string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
