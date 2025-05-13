using System.ComponentModel.DataAnnotations;
using AplicatieLicenta.Models;

namespace AplicatieLicenta.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Trebuie să selectezi o carte !")]
        public int CarteId { get; set; }
        [Required(ErrorMessage = "Titlul testului este obligatoriu !")]
        public string Titlu { get; set; }

        public Carti Carte { get; set; }
        public ICollection<IntrebareQuiz> Intrebari { get; set; }
    }
}
