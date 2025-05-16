using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AplicatieLicenta.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AplicatieLicenta.Pages
{
    public class RecenziiModel : PageModel
    {
        private readonly AppDbContext _context;

        public RecenziiModel(AppDbContext context)
        {
            _context = context;
        }

        public List<CarteCuRating> Recenzii { get; set; } = new();

        public async Task OnGetAsync()
        {
            Recenzii = await _context.Carti
                .Include(c => c.Recenzii)
                .Where(c => c.Recenzii.Any())
                .Select(c => new CarteCuRating
                {
                    Titlu = c.Titlu,
                    Autor = c.Autor,
                    RatingMediu = c.Recenzii
                        .Where(r => r.Rating.HasValue)
                        .Average(r => r.Rating.Value)
                })
                .ToListAsync();
        }

        public class CarteCuRating
        {
            public string Titlu { get; set; }
            public string Autor { get; set; }
            public double RatingMediu { get; set; }
        }
    }
}
