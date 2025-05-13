using System.ComponentModel.DataAnnotations;
namespace AplicatieLicenta.Models
{
    public class VariantaRaspuns
    {
        public int Id { get; set; }
        public int IntrebareQuizId { get; set; }
        [Required(ErrorMessage = "Textul răspunsului este obligatoriu !")]
        public string Text { get; set; }
        public bool EsteCorect { get; set; }

        public IntrebareQuiz Intrebare { get; set; }
    }
}
