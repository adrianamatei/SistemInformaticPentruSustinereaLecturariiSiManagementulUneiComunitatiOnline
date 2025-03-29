using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;

namespace AplicatieLicenta.Pages.Admin
{
    public class StergereCartiModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public StergereCartiModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Carti Carti { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carti = await _context.Carti
                .Include(c => c.CategoriiVarsta) 
                .Include(c => c.Genuri)         
                .FirstOrDefaultAsync(m => m.IdCarte == id);

            if (carti == null)
            {
                return NotFound();
            }

            Carti = carti;
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
             
                if (!string.IsNullOrEmpty(carti.ImagineCoperta))
                {
                    var imaginePath = Path.Combine(_environment.WebRootPath, carti.ImagineCoperta.TrimStart('/'));
                    if (System.IO.File.Exists(imaginePath))
                        System.IO.File.Delete(imaginePath);
                }

                if (!string.IsNullOrEmpty(carti.UrlFisier))
                {
                    var fisierPath = Path.Combine(_environment.WebRootPath, carti.UrlFisier.TrimStart('/'));
                    if (System.IO.File.Exists(fisierPath))
                        System.IO.File.Delete(fisierPath);
                }

                Carti = carti;
                _context.Carti.Remove(Carti);
                await _context.SaveChangesAsync();

               
                if (Carti.TipCarte == "Audio")
                    return RedirectToPage("/Admin/VizualizareCartiAudio");
                else
                    return RedirectToPage("/Admin/VizualizareCartiPdf");
            }

            return NotFound();
        }
    }
}
