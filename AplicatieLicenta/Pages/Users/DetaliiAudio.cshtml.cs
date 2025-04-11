using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace AplicatieLicenta.Pages.Users
{
    public class DetaliiAudioModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetaliiAudioModel(AppDbContext context)
        {
            _context = context;
        }

        public Carti Carte { get; set; }
        public List<Recenzii> Recenzii { get; set; }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        [BindProperty]
        public string Comentariu { get; set; }

        [BindProperty]
        public int Rating { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                HttpContext.Session.SetString("ReturnUrl", $"/Users/DetaliiAudio?id={id}");
                return RedirectToPage("/Users/Login");
            }

            await LoadCarteSiRecenzii(id, userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var email = HttpContext.Session.GetString("UserEmail");

            if (!userId.HasValue || string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Users/Login");
            }

            Carte = await _context.Carti.FirstOrDefaultAsync(c => c.IdCarte == id && c.TipCarte == "Audio");
            if (Carte == null)
            {
                return NotFound(); 
            }

            var recenzie = new Recenzii
            {
                IdCarte = id,
                IdUtilizator = userId.Value,
                EmailUtilizator = email,
                Comentariu = Comentariu,
                Rating = Rating,
                DataPublicarii = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Recenzii.Add(recenzie);

            _context.UsersActivity.Add(new UsersActivity
            {
                UserId = userId.Value,
                Action = "Recenzie carte",
                Data = $"Utilizatorul a scris o recenzie la cartea: {Carte.Titlu}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
            await LoadCarteSiRecenzii(id, userId.Value);

            return Page();
        }


        private async Task LoadCarteSiRecenzii(int carteId, int userId)
        {
            Carte = await _context.Carti
                .Include(c => c.CategoriiVarsta)
                .Include(c => c.Genuri)
                .FirstOrDefaultAsync(c => c.IdCarte == carteId && c.TipCarte == "Audio");

            if (Carte == null)
                return;

            Recenzii = await _context.Recenzii
                .Where(r => r.IdCarte == carteId)
                .OrderByDescending(r => r.DataPublicarii)
                .Take(3)
                .ToListAsync();

            _context.UsersActivity.Add(new UsersActivity
            {
                UserId = userId,
                Action = "Ascultare carte audio",
                Data = $"Utilizatorul a ascultat cartea: {Carte.Titlu}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }
    }
}
