using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AplicatieLicenta.Pages.Users
{
    public class VizualizareTesteModel : PageModel
    {
        private readonly AppDbContext _context;
        public VizualizareTesteModel(AppDbContext context) => _context = context;

        public List<Quiz> TesteDisponibile { get; set; } = new();
        public Dictionary<int, List<RezultatQuiz>> RezultateUser { get; set; } = new();


        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            TesteDisponibile = await _context.Quizuri
                .Include(q => q.Carte)
                .ToListAsync();

            RezultateUser = await _context.RezultateQuiz
              .Where(r => r.UserId == userId.Value)
              .GroupBy(r => r.QuizId)
              .ToDictionaryAsync(g => g.Key, g => g.ToList());


            return Page();
        }
    }
}
