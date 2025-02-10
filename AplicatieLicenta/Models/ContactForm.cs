using System.ComponentModel.DataAnnotations;

namespace AplicatieLicenta.Models
{
    public class ContactForm
    {

        [Required(ErrorMessage = "Numele este obligatoriu !")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Adresa de email este obligatorie !")]
        [EmailAddress(ErrorMessage = "Adresa de email nu este validă !")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Subiectul este obligatoriu !")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Mesajul este obligatoriu !")]
        public string Message { get; set; }
    }
}
