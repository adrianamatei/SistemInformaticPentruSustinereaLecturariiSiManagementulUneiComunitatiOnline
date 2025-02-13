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
    public class AdaugareCartiAudioModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdaugareCartiAudioModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public string titlu { get; set; }
        [BindProperty] public string autor { get; set; }
        [BindProperty] public IFormFile Imagine_Coperta { get; set; }
        [BindProperty] public IFormFile url_fisier { get; set; }
        [BindProperty] public string categorie_varsta { get; set; }
        [BindProperty] public string tip_carte { get; set; } = "Audio";
        [BindProperty] public int ore { get; set; }
        [BindProperty] public int minute { get; set; }
        [BindProperty] public int secunde { get; set; }

        // Proprietate unicã pentru toate mesajele de eroare
        public string messageError { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"Titlu primit: {titlu}");
            Console.WriteLine($"Autor primit: {autor}");
            Console.WriteLine($"Ore: {ore}, Minute: {minute}, Secunde: {secunde}");
            Console.WriteLine($"Categorie Varsta: {categorie_varsta}");
            Console.WriteLine($"Imagine Coperta: {Imagine_Coperta?.FileName}");
            Console.WriteLine($"Fisier Audio: {url_fisier?.FileName}");

            if(string.IsNullOrWhiteSpace(titlu) && string.IsNullOrWhiteSpace(autor) && Imagine_Coperta == null && url_fisier == null )
            {
                messageError = "Toate campurile sunt obligatorii !";
                return Page();
            }
            if (string.IsNullOrWhiteSpace(titlu)) 
            { 
                messageError = "Titlul este obligatoriu !";
                return Page();
            }
            if (string.IsNullOrWhiteSpace(autor))
            {
                messageError = "Autorul este obligatoriu !";
                return Page();
            }
            if (Imagine_Coperta == null) 
            {
                messageError = "Imaginea de coperta este obligatorie !"; 
                return Page();
            }
            if (url_fisier == null) 
            {
                messageError = "Fisierul audio este obligatoriu !";
                return Page(); 
            }
            if (string.IsNullOrWhiteSpace(categorie_varsta))
            { 
                messageError = "Categoria de varsta este obligatorie !";
                return Page();
            }
            if (ore < 0 || minute < 0 || minute >= 60 || secunde < 0 || secunde >= 60)
            {
                messageError = "Introduceti o durata valida !";
                return Page();
            }

            string extensieFisier = Path.GetExtension(url_fisier.FileName).ToLower();
            if (extensieFisier != ".mp3" && extensieFisier != ".wav")
            {
                messageError = "Doar fisiere MP3 si WAV sunt acceptate !";
                return Page();
            }

            var existingBook = await _context.Carti.FirstOrDefaultAsync(c => c.Titlu == titlu);
            if (existingBook != null)
            {
                messageError = "O carte cu acest titlu exista deja in baza de date !";
                return Page();
            }

            try
            {
                
                string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

            
                string imagineFileName = $"{Path.GetFileNameWithoutExtension(Imagine_Coperta.FileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(Imagine_Coperta.FileName)}";
                string imaginePath = Path.Combine(uploadPath, imagineFileName);
                using (var fileStream = new FileStream(imaginePath, FileMode.Create))
                {
                    await Imagine_Coperta.CopyToAsync(fileStream);
                }
                string imagineUrl = $"/uploads/{imagineFileName}";

              
                string audioFileName = $"{Path.GetFileNameWithoutExtension(url_fisier.FileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(url_fisier.FileName)}";
                string audioPath = Path.Combine(uploadPath, audioFileName);
                using (var fileStream = new FileStream(audioPath, FileMode.Create))
                {
                    await url_fisier.CopyToAsync(fileStream);
                }
                string audioUrl = $"/uploads/{audioFileName}";

                TimeSpan durataAscultare = new TimeSpan(ore, minute, secunde);

           
                var carte = new Carti
                {
                    Titlu = titlu,
                    Autor = autor,
                    ImagineCoperta = imagineUrl,
                    UrlFisier = audioUrl,
                    TipCarte = "Audio",
                    CategorieVarsta = categorie_varsta,
                    DurataAscultare = durataAscultare
                };

                _context.Carti.Add(carte);
                await _context.SaveChangesAsync();

                return RedirectToPage("/Admin/VizualizareCartiAudio");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare server: {ex.Message}");
                messageError = "A aparut o eroare la procesarea cererii !";
                return Page();
            }
        }
    }
}
