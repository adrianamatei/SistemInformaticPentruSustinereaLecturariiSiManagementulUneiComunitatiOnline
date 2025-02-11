using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;

namespace AplicatieLicenta.Pages.Admin
{
    public class EditareModel : PageModel
    {
        private readonly AplicatieLicenta.Data.AppDbContext _context;

        public EditareModel(AplicatieLicenta.Data.AppDbContext context)
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

            var carti =  await _context.Carti.FirstOrDefaultAsync(m => m.IdCarte == id);
            if (carti == null)
            {
                return NotFound();
            }
            Carti = carti;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Carti).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartiExists(Carti.IdCarte))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Admin/VizualizareCartiPdf");
        }

        private bool CartiExists(int id)
        {
            return _context.Carti.Any(e => e.IdCarte == id);
        }
    }
}
