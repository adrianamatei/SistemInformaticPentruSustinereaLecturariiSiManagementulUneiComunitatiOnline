using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using BCrypt.Net;
using AplicatieLicenta.Data;
using System;
using AplicatieLicenta.Models;
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
        public string ErrorMessageReader {  get; set; }
        public string ErrorMessageAdmin { get; set; }
        public IActionResult OnPostLoginReader()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessageReader = "Ambele campuri sunt obligatorii !";
                return Page();
            }
            var reader=_context.Users.FirstOrDefault(u=>u.Email == Email);
            if (reader == null)
            {
                ErrorMessageReader = "Email-ul introdus nu exista !";
                return Page();
            }
            if(reader.BlocatPanaLa.HasValue && reader.BlocatPanaLa.Value >DateTime.Now)
            {
                ErrorMessageReader = $"Contul este blocat pana la {reader.BlocatPanaLa.Value:HH:MM:SS}.";
                return Page();
            }
            if (!BCrypt.Net.BCrypt.Verify(Password,reader.Parola))
            {
                reader.NumarIncercariEsec += 1;
                if(reader.NumarIncercariEsec>=3)
                {
                    reader.BlocatPanaLa = DateTime.Now.AddMinutes(5);
                    ErrorMessageReader = "Contul este blocat pentru 5 minute !";
                }
                else
                {
                    ErrorMessageReader = $"Parola este incorecta ! Mai aveti {3 - reader.NumarIncercariEsec} incercari !";

                }
                _context.SaveChanges();
                return Page();
            }
            reader.NumarIncercariEsec = 0;
            reader.BlocatPanaLa = null;
            _context.SaveChanges();
            _context.UsersActivity.Add(new UsersActivity
            {
                UserId=reader.IdUtilizator,
                Action="Autentificare reusita",
                Data=$"Utilizatorul {reader.Email} s-a logat cu succes",
                Timestamp=DateTime.Now

            });
            _context.SaveChanges();
            HttpContext.Session.SetString("UserEmail",reader.Email);
            HttpContext.Session.SetInt32("UserId",reader.IdUtilizator);
            return RedirectToPage("/Users/StartUser");
        }
        public IActionResult OnPostLoginAdmin()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessageAdmin = "Ambele campuri sunt obligatorii !";
                return Page();
            }

            var admin = _context.Admins.FirstOrDefault(a => a.Email == Email);
            if(admin==null)
            {
                ErrorMessageAdmin = "Emial-ul introdus nu exista !";
                return Page();  
            }
            if(!BCrypt.Net.BCrypt.Verify(Password,admin.Parola))
            {
                admin.NumarIncercariEsec += 1;
                if(admin.NumarIncercariEsec>=3)
                {
                    admin.BlocatPanaLa = DateTime.Now.AddMinutes(5);
                    ErrorMessageAdmin = "Contul este blocat pentru 5 minute";
                }
                else
                {
                    ErrorMessageAdmin = $"Parola este incorecta ! Mai aveti {3 - admin.NumarIncercariEsec} incercari";

                }
                _context.SaveChanges();
                return Page();
            }
            admin.NumarIncercariEsec = 0;
            admin.BlocatPanaLa = null;
            _context.SaveChanges();
            HttpContext.Session.SetString("AdminEmail",admin.Email);
            HttpContext.Session.SetInt32("AdminId", admin.IdAdmin);
            return RedirectToPage("/Admin/VizualizareStatistici");

        }
    }
}
