using AplicatieLicenta.Data;
using AplicatieLicenta.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace AplicatieLicenta.Pages.Users
{
    public class DescarcaDiplomaModel : PageModel
    {
        private readonly AppDbContext _context;

        public DescarcaDiplomaModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToPage("/Login");

            int id = userId.Value;
            var top3 = await _context.Users
                .Select(u => new
                {
                    u.IdUtilizator,
                    u.Email,
                    Scor = _context.RezultateQuiz
                        .Where(r => r.UserId == u.IdUtilizator && r.Scor >= 80)
                        .Select(r => r.Scor >= 100 ? 20 : r.Scor >= 90 ? 15 : 10)
                        .Sum()
                })
                .OrderByDescending(u => u.Scor)
                .Take(3)
                .ToListAsync();

            var utilizator = top3
                .Select((u, index) => new { u.IdUtilizator, u.Email, Loc = index + 1 })
                .FirstOrDefault(u => u.IdUtilizator == id);

            if (utilizator == null)
                return Unauthorized(); 

            var fileName = $"diploma_{id}.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "diplome", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                DiplomaGenerator.GenereazaDiploma(utilizator.Email, utilizator.Loc, filePath);
            }

            return PhysicalFile(filePath, "application/pdf", "Diploma.pdf");
        }

    }
}
