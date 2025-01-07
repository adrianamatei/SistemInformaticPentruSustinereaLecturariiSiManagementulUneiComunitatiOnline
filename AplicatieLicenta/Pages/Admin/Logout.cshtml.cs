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
            // Logout - eliminã cookie-ul de autentificare (dacã folose?ti Identity sau autentificare cu cookie-uri)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirec?ioneazã utilizatorul cãtre pagina principalã sau pagina de login
            return RedirectToPage("/Index");
        }
    }
}
