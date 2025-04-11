using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace AplicatieLicenta.Pages.Users
{
    public class DetaliiPdfModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetaliiPdfModel(AppDbContext context)
        {
            _context = context;
        }

        public Carti Carte { get; set; }
        public List<Recenzii> Recenzii { get; set; }

        [BindProperty] public string Comentariu { get; set; }
        [BindProperty] public int Rating { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                HttpContext.Session.SetString("ReturnUrl", $"/Users/DetaliiPdf?id={id}");
                return RedirectToPage("/Users/Login");
            }

            await LoadCarteSiRecenzii(id, userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Users/Login");
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
                return RedirectToPage("/Users/Login");

            
            Carte = await _context.Carti.FirstOrDefaultAsync(c => c.IdCarte == id);
            if (Carte == null)
            {
                return NotFound(); 
            }

            var recenzie = new Recenzii
            {
                IdCarte = id,
                IdUtilizator = userId.Value,
                EmailUtilizator = user.Email,
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
                .FirstOrDefaultAsync(c => c.IdCarte == carteId && c.TipCarte == "PDF");

            Recenzii = await _context.Recenzii
                .Where(r => r.IdCarte == carteId)
                .OrderByDescending(r => r.DataPublicarii)
                .Take(3)
                .ToListAsync();

            _context.UsersActivity.Add(new UsersActivity
            {
                UserId = userId,
                Action = "Vizualizare carte PDF",
                Data = $"Utilizatorul a deschis cartea: {Carte?.Titlu}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }
    }
}
