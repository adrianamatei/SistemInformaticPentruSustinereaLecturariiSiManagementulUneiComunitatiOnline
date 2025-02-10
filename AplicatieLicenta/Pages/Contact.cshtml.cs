using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Reflection;
using AplicatieLicenta.Models; 


namespace AplicatieLicenta.Pages
{
    public class ContactModel : PageModel
    {
        [BindProperty]
        public ContactForm ContactForm { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public void OnGet()
        {
            ContactForm = new ContactForm();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Toate campurile sunt obligatorii ! ";
                return Page();
            }
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential("ionelaamatei2004@gmail.com", "wnkx kjuh mawj cnjv"),
                    EnableSsl = true
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(ContactForm.Email, ContactForm.Name),
                    Subject = ContactForm.Subject,
                    Body = $"Mesaj de la {ContactForm.Name}({ContactForm.Email}):\n\n{ContactForm.Message}",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add("ionelaamatei2004@gmail.com");
                await smtpClient.SendMailAsync(mailMessage);
                SuccessMessage = "Mesajul a fost trimis cu succes !";
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"A aparut o eroare la trimiterea mesajului {ex.Message}";

            }
            return Page();
        }
    }
   
}
