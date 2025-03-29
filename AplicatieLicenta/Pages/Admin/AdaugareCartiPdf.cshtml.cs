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
        [BindProperty] public List<string> CategorieVarsta { get; set; } = new List<string>();
        [BindProperty]
        public List<string> Gen { get; set; } = new List<string>();
        [BindProperty] public string TipCarte { get; set; } = "PDF";
        public string messageError { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Titlu) && string.IsNullOrEmpty(Autor) && UrlFisier == null && ImagineCoperta == null )
                {
                    messageError = "Toate campurile sunt obligatorii !";
                    return Page();
                }

                if (string.IsNullOrEmpty(Titlu))
                {
                    messageError = "Titlul este obligatoriu !";
                    return Page();
                }
                if (string.IsNullOrEmpty(Autor))
                {
                    messageError = "Autorul este obligatoriu !";
                    return Page();
                }
                if (UrlFisier == null)
                {
                    messageError= "Fisierul PDF este obligatoriu !";
                    return Page();
                }
                if (ImagineCoperta == null)
                {
                    messageError= "Imaginea copertei este obligatorie !";
                    return Page();
                }
            
                if (TipCarte != "PDF")
                {
                    messageError= "Doar fisiere PDF sunt acceptate !";
                    return Page();
                }


                var existingBook = await _context.Carti.FirstOrDefaultAsync(c => c.Titlu == Titlu && c.Autor == Autor && c.TipCarte == "PDF");

                if (existingBook != null)
                {
                    messageError = "O carte cu acest titlu exista deja in baza de date !";
                    return Page();
                }
                string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string imagineFileName =  Path.GetFileName(ImagineCoperta.FileName);
                string imaginePath = Path.Combine(uploadPath, imagineFileName);
                using (var fileStream = new FileStream(imaginePath, FileMode.Create))
                {
                    await ImagineCoperta.CopyToAsync(fileStream);
                }
                string imagineUrl = $"/uploads/{imagineFileName}";

                string pdfFileName =  Path.GetFileName(UrlFisier.FileName);
                string pdfPath = Path.Combine(uploadPath, pdfFileName);
                using (var fileStream = new FileStream(pdfPath, FileMode.Create))
                {
                    await UrlFisier.CopyToAsync(fileStream);
                }
                string pdfUrl = $"/uploads/{pdfFileName}";

                var categoriiSelectate = await _context.CategoriiVarsta
           .Where(cv => CategorieVarsta.Contains(cv.Denumire))
           .ToListAsync();

                var genuriSelectate = await _context.Genuri
                    .Where(g => Gen.Contains(g.Denumire))
                    .ToListAsync();

               
                var carte = new Carti
                {
                    Titlu = Titlu,
                    Autor = Autor,
                    ImagineCoperta = $"/uploads/{imagineFileName}",
                    UrlFisier = $"/uploads/{pdfFileName}",
                    TipCarte = "PDF",
                    CategoriiVarsta = categoriiSelectate,
                    Genuri = genuriSelectate
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
