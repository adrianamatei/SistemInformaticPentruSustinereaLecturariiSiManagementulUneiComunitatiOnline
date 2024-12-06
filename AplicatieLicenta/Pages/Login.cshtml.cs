using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AplicatieLicenta.Pages.Users
{
    public class LoginModel : PageModel
    {
        public IActionResult OnPostL()
        {
            // Logicã pentru autentificare
            return Page();
        }

        public IActionResult OnPostRegister()
        {
            // Logicã pentru înregistrare
            return Page();
        }
        public void OnGet()
        {
        }
    }
}
