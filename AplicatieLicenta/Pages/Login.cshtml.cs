using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AplicatieLicenta.Pages.Users
{
    public class LoginModel : PageModel
    {
        public IActionResult OnPostL()
        {
            // Logic� pentru autentificare
            return Page();
        }

        public IActionResult OnPostRegister()
        {
            // Logic� pentru �nregistrare
            return Page();
        }
        public void OnGet()
        {
        }
    }
}
