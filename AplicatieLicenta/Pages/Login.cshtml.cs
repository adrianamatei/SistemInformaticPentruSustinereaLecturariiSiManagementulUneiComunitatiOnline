using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using BCrypt.Net; // Utilizare pentru hashing
using AplicatieLicenta.Data;

namespace AplicatieLicenta.Pages.Users
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessageReader { get; set; }
        public string ErrorMessageAdmin { get; set; }

        // Autentificare Cititor
        public IActionResult OnPostLoginReader()
        {
            // Verificare câmpuri obligatorii
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessageReader = "Ambele campuri sunt obligatorii!";
                return Page();
            }

            var reader = _context.Users.FirstOrDefault(u => u.Email == Email);

            // Verificã dacã utilizatorul existã
            if (reader == null)
            {
                ErrorMessageReader = "Email-ul introdus nu exista in baza de date!";
                return Page();
            }

            // Verificã dacã cititorul este blocat
            if (reader.BlocatPanaLa.HasValue && reader.BlocatPanaLa.Value > DateTime.Now)
            {
                ErrorMessageReader = $"Contul este blocat. Incercati din nou dupa {reader.BlocatPanaLa.Value.ToString("HH:mm:ss")}.";
                return Page();
            }

            // Verificare parolã
            if (!BCrypt.Net.BCrypt.Verify(Password, reader.Parola))
            {
                reader.NumarIncercariEsec += 1;

                // Blocheazã utilizatorul dacã depã?e?te limita de încercãri
                if (reader.NumarIncercariEsec >= 3)
                {
                    reader.BlocatPanaLa = DateTime.Now.AddMinutes(5); // Blocare pentru 5 minute
                    ErrorMessageReader = "Ati depasit numarul de incercari. Contul este blocat pentru 5 minute.";
                }
                else
                {
                    ErrorMessageReader = $"Parola este incorecta! Mai aveti {3 - reader.NumarIncercariEsec} incercari.";
                }

                _context.SaveChanges();
                return Page();
            }

            // Resetare contor la autentificare reu?itã
            reader.NumarIncercariEsec = 0;
            reader.BlocatPanaLa = null;
            _context.SaveChanges();

            return RedirectToPage("/Users/StartUser");
        }

        // Autentificare Administrator
        public IActionResult OnPostLoginAdmin()
        {
            // Verificare câmpuri obligatorii
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessageAdmin = "Ambele campuri sunt obligatorii!";
                return Page();
            }

            var admin = _context.Admins.FirstOrDefault(a => a.Email == Email);

            // Verificã dacã utilizatorul existã
            if (admin == null)
            {
                ErrorMessageAdmin = "Email-ul introdus nu exista in baza de date!";
                return Page();
            }

            // Verificã dacã administratorul este blocat
            if (admin.BlocatPanaLa.HasValue && admin.BlocatPanaLa.Value > DateTime.Now)
            {
                ErrorMessageAdmin = $"Contul este blocat. Incercati din nou dupa {admin.BlocatPanaLa.Value.ToString("HH:mm:ss")}.";
                return Page();
            }

            // Verificare parolã
            if (!BCrypt.Net.BCrypt.Verify(Password, admin.Parola))
            {
                admin.NumarIncercariEsec += 1;

                // Blocheazã administratorul dacã depã?e?te limita de încercãri
                if (admin.NumarIncercariEsec >= 3)
                {
                    admin.BlocatPanaLa = DateTime.Now.AddMinutes(5); // Blocare pentru 5 minute
                    ErrorMessageAdmin = "Ati depasit numarul de incercari. Contul este blocat pentru 5 minute.";
                }
                else
                {
                    ErrorMessageAdmin = $"Parola este incorecta! Mai aveti {3 - admin.NumarIncercariEsec} incercari.";
                }

                _context.SaveChanges();
                return Page();
            }

            admin.NumarIncercariEsec = 0;
            admin.BlocatPanaLa = null;
            _context.SaveChanges();

            return RedirectToPage("/Admin/StartAdmin");
        }
    }
}
