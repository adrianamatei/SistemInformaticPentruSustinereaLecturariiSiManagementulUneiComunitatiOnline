using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using AplicatieLicenta.Pages.API;

namespace AplicatieLicenta.Pages.Users
{
    public class StartUserModel : PageModel
    {
        private readonly AppDbContext _context;

        public StartUserModel(AppDbContext context)
        {
            _context = context;
        }

        public string CategorieVarsta { get; set; }
        public List<CluburiLectura> CluburiRelevante { get; set; } = new();
        public int UserId { get; set; }
        public string UserEmail { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Login");
            }

            UserId = userId.Value;

            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
                return RedirectToPage("/Login");

            CategorieVarsta = user.CategorieVarsta;
            UserEmail = user.Email;

            CluburiRelevante = await _context.CluburiLectura
                .Include(c => c.MembriClub)
                .Where(c => c.CategorieVarsta == CategorieVarsta)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostJoinAsync(int clubId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToPage("/Login");

            bool esteMembru = await _context.MembriClub
                .AnyAsync(m => m.IdUtilizator == userId.Value && m.IdClub == clubId);

            if (!esteMembru)
            {
                _context.MembriClub.Add(new MembriClub
                {
                    IdUtilizator = userId.Value,
                    IdClub = clubId,
                    DataInscrierii = DateTime.Now,
                    Status = "In asteptare"
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<JsonResult> OnGetIncarcaMesajeAsync(int idClub)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return new JsonResult(new { error = "Utilizator neautentificat" });

            var club = await _context.CluburiLectura.FindAsync(idClub);
            if (club != null)
            {
                _context.UsersActivity.Add(new UsersActivity
                {
                    UserId = userId.Value,
                    Action = $"A vizualizat conversatia din clubul de lectura \"{club.Nume}\"",
                    Data = $"Utilizatorul {UserEmail} a accesat chatul",
                    Timestamp = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }

            var mesaje = await _context.MesajClub
                .Include(m => m.Utilizator)
                .Where(m => m.IdClub == idClub)
                .OrderBy(m => m.DataTrimiterii)
                .Select(m => new
                {
                    email = m.Utilizator.Email,
                    continut = m.Continut,
                    dataTrimiterii = m.DataTrimiterii.ToString("HH:mm")
                })
                .ToListAsync();

            return new JsonResult(mesaje);
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostVocalAsync(IFormFile vocal, int idClub, int userId, string userEmail)
        {
            if (vocal == null || vocal.Length == 0)
                return BadRequest("Fisier vocal invalid.");

            var extensie = Path.GetExtension(vocal.FileName);
            var fileName = $"{Guid.NewGuid()}{extensie}";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vocale", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await vocal.CopyToAsync(stream);
            }

            _context.UsersActivity.Add(new UsersActivity
            {
                UserId = userId,
                Action = $"A trimis un mesaj vocal in clubul cu ID {idClub}",
                Data = $"Fisier: {fileName}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Content(fileName);
        }
    }
}
