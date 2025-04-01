using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AplicatieLicenta.Pages.Admin
{
    public class GestionareCluburiLecturaModel : PageModel
    {
        private readonly AppDbContext _context;

        public GestionareCluburiLecturaModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CluburiLectura ClubNou { get; set; } = new();

        public List<CluburiLectura> Cluburi { get; set; } = new();

        public async Task OnGetAsync()
        {
            Cluburi = await _context.CluburiLectura
                .Include(c => c.MembriClub)
                    .ThenInclude(m => m.IdUtilizatorNavigation)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCreeazaAsync()
        {
            var adminId = 1; // Înlocuie?te cu ID-ul real dacã ai autentificare
            ClubNou.IdCreator = adminId;
            ClubNou.DataCrearii = DateTime.Now;

            _context.CluburiLectura.Add(ClubNou);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostStergeClubAsync(int clubId)
        {
            var club = await _context.CluburiLectura
                .Include(c => c.MembriClub)
                .FirstOrDefaultAsync(c => c.IdClub == clubId);

            if (club != null)
            {
                // ?terge membrii clubului înainte
                _context.MembriClub.RemoveRange(club.MembriClub);
                _context.CluburiLectura.Remove(club);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAprobaMembruAsync(int membruId)
        {
            var membru = await _context.MembriClub.FindAsync(membruId);
            if (membru != null)
            {
                membru.Status = "Aprobat";
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminaMembruAsync(int membruId)
        {
            var membru = await _context.MembriClub.FindAsync(membruId);
            if (membru != null)
            {
                _context.MembriClub.Remove(membru);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
