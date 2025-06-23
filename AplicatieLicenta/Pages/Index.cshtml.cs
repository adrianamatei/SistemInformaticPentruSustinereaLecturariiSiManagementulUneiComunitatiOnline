using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace AplicatieLicenta.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public Carti Recomandare { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var azi = DateTime.Today;
            var recomandareAzi = await _context.RecomandariZilnice
                .Include(r => r.Carte)
                .FirstOrDefaultAsync(r => r.DataGenerare.Date == azi);

            if (recomandareAzi != null)
            {
                Recomandare = recomandareAzi.Carte;
            }
            else
            {
                var carti = await _context.Carti.ToListAsync();
                if (carti.Any())
                {
                    var random = new Random();
                    var carteNoua = carti[random.Next(carti.Count)];

                    Recomandare = carteNoua;

                    var recomandareNoua = new RecomandareZilnica
                    {
                        CarteId = carteNoua.IdCarte,
                        DataGenerare = DateTime.Now
                    };

                    _context.RecomandariZilnice.Add(recomandareNoua);
                    await _context.SaveChangesAsync();
                }
            }
            return Page();
        }
    }
}
