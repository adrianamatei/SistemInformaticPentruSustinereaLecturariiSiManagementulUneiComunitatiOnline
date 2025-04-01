using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AplicatieLicenta.Pages.Admin
{
    public class VizualizareUtilizatoriInregistratiModel : PageModel
    {
        private readonly AppDbContext _context;
        public int TotalActivitati { get; set; }


        public VizualizareUtilizatoriInregistratiModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string DataSelectata { get; set; } = "";

        public List<UserCuActivitate> Activitati { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(DataSelectata))
            {
                if (DateTime.TryParseExact(DataSelectata, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
                {
                    Activitati = await (
                        from activitate in _context.UsersActivity
                        join user in _context.Users on activitate.UserId equals user.IdUtilizator
                        where activitate.Timestamp.Date == data.Date
                        select new UserCuActivitate
                        {
                            Email = user.Email,
                            Actiune = activitate.Action,
                            Detalii = activitate.Data, // <-- aici adãugãm titlul cãr?ii sau detalii
                            Timestamp = activitate.Timestamp
                        }).ToListAsync();
                    TotalActivitati = Activitati.Count;

                }
            }
        }

        public class UserCuActivitate
        {
            public string Email { get; set; }
            public string Actiune { get; set; }
            public string Detalii { get; set; } // <-- asigurã-te cã e definit aici
            public DateTime Timestamp { get; set; }
        }
    }
}
