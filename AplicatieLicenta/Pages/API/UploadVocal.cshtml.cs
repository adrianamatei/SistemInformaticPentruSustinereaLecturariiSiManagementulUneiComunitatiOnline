using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AplicatieLicenta.Pages.API
{
    public class UploadVocalModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public UploadVocalModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<IActionResult> OnPostAsync(IFormFile vocal, int idClub, int userId, string userEmail)
        {
            if (vocal == null || vocal.Length == 0)
                return BadRequest("Fisier invalid.");

            var extensie = Path.GetExtension(vocal.FileName);
            var fileName = $"{Guid.NewGuid()}{extensie}";
            var path = Path.Combine(_env.WebRootPath, "vocale", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await vocal.CopyToAsync(stream);
            }

            return Content(fileName); 
        }
    }
}
