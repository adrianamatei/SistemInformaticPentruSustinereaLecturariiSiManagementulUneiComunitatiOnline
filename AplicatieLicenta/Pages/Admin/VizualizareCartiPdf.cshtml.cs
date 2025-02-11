using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;


namespace AplicatieLicenta.Pages.Admin
{
    public class VizualizareCartiPdfModel : PageModel
    {
        private readonly AppDbContext _context;
        public IList<Carti> Carti { get; set; }

        public VizualizareCartiPdfModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Carti=await _context.Carti.ToListAsync();
        }
    }
}
