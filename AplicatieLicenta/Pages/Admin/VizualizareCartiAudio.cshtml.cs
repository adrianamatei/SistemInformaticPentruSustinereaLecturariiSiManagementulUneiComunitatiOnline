using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using AplicatieLicenta.Data;
using System.Linq;
using AplicatieLicenta.Models;


namespace AplicatieLicenta.Pages.Admin
{
    public class VizualizareCartiAudioModel : PageModel
    {
        private readonly AppDbContext _context;
        public IList<Carti> Carti { get; set; }

        public VizualizareCartiAudioModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Carti = await _context.Carti
     .Where(c => c.TipCarte == "Audio")
     .Include(c => c.CategoriiVarsta)
     .Include(c => c.Genuri)
     .ToListAsync();


        }
    }
}
