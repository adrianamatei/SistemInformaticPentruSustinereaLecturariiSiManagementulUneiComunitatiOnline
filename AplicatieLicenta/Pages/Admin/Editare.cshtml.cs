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
        [BindProperty]
        public List<string> CategorieVarstaValori { get; set; } = new List<string>();
        [BindProperty]
        public List<string> GenValori { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carti =  await _context.Carti
                .Include(c => c.CategoriiVarsta)
                .Include(c => c.Genuri)
                .FirstOrDefaultAsync(m => m.IdCarte == id);
            if (carti == null)
            {
                return NotFound();
            }
            Carti= carti;
            CategorieVarstaValori = Carti.CategoriiVarsta.Select(cv => cv.Denumire).ToList();
            GenValori= Carti.Genuri.Select(g => g.Denumire).ToList();

            return Page();
        }

       
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var cartiDb = await _context.Carti
               .Include(c => c.CategoriiVarsta)
               .Include(c => c.Genuri)
               .FirstOrDefaultAsync(c => c.IdCarte == Carti.IdCarte);


            if (cartiDb == null) 
                return NotFound();

            cartiDb.Titlu = Carti.Titlu;
            cartiDb.Autor = Carti.Autor;
            cartiDb.ImagineCoperta = Carti.ImagineCoperta;
            cartiDb.UrlFisier = Carti.UrlFisier;
            cartiDb.TipCarte = Carti.TipCarte;
            cartiDb.DurataAscultare = Carti.DurataAscultare;

            cartiDb.CategoriiVarsta = await _context.CategoriiVarsta
                .Where(cv => CategorieVarstaValori.Contains(cv.Denumire))
                .ToListAsync();

            cartiDb.Genuri = await _context.Genuri
                .Where(g => GenValori.Contains(g.Denumire))
                .ToListAsync();

            await _context.SaveChangesAsync();
            if (cartiDb.TipCarte == "Audio")
            {
                return RedirectToPage("/Admin/VizualizareCartiAudio");
            }
            else 
            {
                return RedirectToPage("/Admin/VizualizareCartiPdf");
            }

        }


    }
}
