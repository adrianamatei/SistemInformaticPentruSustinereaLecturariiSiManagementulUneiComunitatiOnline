using AplicatieLicenta.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AplicatieLicenta.Pages.Admin
{
    public class VizualizareTesteModel : PageModel
    {
        private readonly AppDbContext _context;

        public VizualizareTesteModel(AppDbContext context)
        {
            _context = context;
        }

        public List<TestStatsDto> StatisticiTeste { get; set; } = new();

        public async Task OnGetAsync()
        {
            StatisticiTeste = await _context.Quizuri
                .Include(q => q.Carte)
                .Select(q => new TestStatsDto
                {
                    TitluTest = q.Titlu,
                    TitluCarte = q.Carte.Titlu,
                    UtilizatoriPromovati = _context.RezultateQuiz
                        .Where(r => r.QuizId == q.Id && r.Scor >= 80)
                        .Select(r => r.Utilizator.Email)
                        .ToList(),

                    UtilizatoriPicati = _context.RezultateQuiz
                        .Where(r => r.QuizId == q.Id && r.Scor < 80)
                        .Select(r => r.Utilizator.Email)
                        .ToList()
                })
                .ToListAsync();
        }

        public class TestStatsDto
        {
            public string TitluTest { get; set; }
            public string TitluCarte { get; set; }
            public List<string> UtilizatoriPromovati { get; set; }
            public List<string> UtilizatoriPicati { get; set; }
        }
    }
}
