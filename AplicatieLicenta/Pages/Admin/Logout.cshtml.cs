using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AplicatieLicenta.Pages.Admin
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // Logout - elimin� cookie-ul de autentificare (dac� folose?ti Identity sau autentificare cu cookie-uri)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirec?ioneaz� utilizatorul c�tre pagina principal� sau pagina de login
            return RedirectToPage("/Index");
        }
    }
}
