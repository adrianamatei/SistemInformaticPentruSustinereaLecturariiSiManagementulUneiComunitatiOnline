using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace AplicatieLicenta.Pages.Admin
{
    public class AdaugareCartiPdfModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdaugareCartiPdfModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult OnGet()
        {
            return Page();
        }
        [BindProperty] public string Titlu { get; set; }
        [BindProperty] public string Autor { get; set; }
        [BindProperty] public IFormFile ImagineCoperta { get; set; }
        [BindProperty] public IFormFile UrlFisier { get; set; }
        [BindProperty] public string CategorieVarsta { get; set; }
        [BindProperty] public string TipCarte { get; set; } = "PDF";

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Titlu) || string.IsNullOrEmpty(Autor) || UrlFisier == null || ImagineCoperta == null || string.IsNullOrEmpty(CategorieVarsta))
                {
                    ModelState.AddModelError(string.Empty, "Toate câmpurile sunt obligatorii !");
                }

                if (string.IsNullOrEmpty(Titlu))
                {
                    ModelState.AddModelError(string.Empty, "Titlul este obligatoriu !");
                }
                if (string.IsNullOrEmpty(Autor))
                {
                    ModelState.AddModelError(string.Empty, "Autorul este obligatoriu !");
                }
                if (UrlFisier == null)
                {
                    ModelState.AddModelError(string.Empty, "Fisierul PDF este obligatoriu !");
                }
                if (ImagineCoperta == null)
                {
                    ModelState.AddModelError(string.Empty, "Imaginea copertei este obligatorie !");
                }
                if (string.IsNullOrEmpty(CategorieVarsta))
                {
                    ModelState.AddModelError(string.Empty, "Categoria de varsta este obligatorie !");
                }
                if (TipCarte != "PDF")
                {
                    ModelState.AddModelError(string.Empty, "Doar fisiere PDF sunt acceptate !");
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }
                var existingBook = await _context.Carti.FirstOrDefaultAsync(c => c.Titlu == Titlu);
                if (existingBook != null)
                {
                    ModelState.AddModelError(string.Empty, "O carte cu acest titlu exista deja in baza de date !");
                    return Page();
                }
                string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string imagineFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImagineCoperta.FileName);
                string imaginePath = Path.Combine(uploadPath, imagineFileName);
                using (var fileStream = new FileStream(imaginePath, FileMode.Create))
                {
                    await ImagineCoperta.CopyToAsync(fileStream);
                }
                string imagineUrl = $"/uploads/{imagineFileName}";

                string pdfFileName = Guid.NewGuid().ToString() + Path.GetExtension(UrlFisier.FileName);
                string pdfPath = Path.Combine(uploadPath, pdfFileName);
                using (var fileStream = new FileStream(pdfPath, FileMode.Create))
                {
                    await UrlFisier.CopyToAsync(fileStream);
                }
                string pdfUrl = $"/uploads/{pdfFileName}";

                var carte = new Carti
                {
                    Titlu = Titlu,
                    Autor = Autor,
                    ImagineCoperta = imagineUrl,
                    UrlFisier = pdfUrl,
                    TipCarte = "PDF",
                    CategorieVarsta = CategorieVarsta
                };

                _context.Carti.Add(carte);
                await _context.SaveChangesAsync();

                return RedirectToPage("/Admin/VizualizareCartiPdf");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "A aparut o eroare la procesarea cererii !");
                return Page();
            }
        }

    }
}
