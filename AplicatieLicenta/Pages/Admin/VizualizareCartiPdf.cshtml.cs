using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AplicatieLicenta.Pages.Admin
{
    public class VizualizareCartiPdfModel : PageModel
    {
        private readonly AppDbContext _context;

        public VizualizareCartiPdfModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Carti> Carti { get; set; }

        public async Task OnGetAsync()
        {
            Carti = await _context.Carti
                .Where(c => c.TipCarte == "PDF")
                .Include(c => c.Genuri) 
                .Include(c => c.CategoriiVarsta) 
                .ToListAsync();
        }
    }
}
