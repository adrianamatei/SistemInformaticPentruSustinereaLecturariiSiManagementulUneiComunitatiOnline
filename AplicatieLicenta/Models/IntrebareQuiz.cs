using System.ComponentModel.DataAnnotations;
namespace AplicatieLicenta.Models
{
    public class IntrebareQuiz
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        [Required(ErrorMessage = "Enunțul este obligatoriu !")]
        public string Enunt { get; set; }
        [Required(ErrorMessage = "Selectează o categorie !")]
        public string Categorie { get; set; } // Personaje, Emotii, Actiune

        public Quiz Quiz { get; set; }
        public ICollection<VariantaRaspuns> Variante { get; set; }
    }
}
