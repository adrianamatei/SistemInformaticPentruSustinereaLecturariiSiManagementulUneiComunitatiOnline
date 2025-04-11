using AplicatieLicenta.Models;

namespace AplicatieLicenta.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public int CarteId { get; set; }
        public string Titlu { get; set; }

        public Carti Carte { get; set; }
        public ICollection<IntrebareQuiz> Intrebari { get; set; }
    }
}
