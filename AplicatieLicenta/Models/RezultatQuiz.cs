using AplicatieLicenta.Models; 

namespace AplicatieLicenta.Models
{
    public class RezultatQuiz
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int QuizId { get; set; }
        public int Scor { get; set; }
        public DateTime Data { get; set; }

        public User Utilizator { get; set; }
        public Quiz Quiz { get; set; }
    }
}
