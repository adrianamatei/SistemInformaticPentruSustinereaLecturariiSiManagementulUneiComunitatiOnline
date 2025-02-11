using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;

namespace AplicatieLicenta.Pages.Admin
{
    public class StergereCartiModel : PageModel
    {
        private readonly AplicatieLicenta.Data.AppDbContext _context;

        public StergereCartiModel(AplicatieLicenta.Data.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Carti Carti { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carti = await _context.Carti.FirstOrDefaultAsync(m => m.IdCarte == id);

            if (carti == null)
            {
                return NotFound();
            }
            else
            {
                Carti = carti;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carti = await _context.Carti.FindAsync(id);
            if (carti != null)
            {
                Carti = carti;
                _context.Carti.Remove(Carti);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Admin/VizualizareCartiPdf");
        }
    }
}
