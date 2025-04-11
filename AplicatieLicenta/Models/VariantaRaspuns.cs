namespace AplicatieLicenta.Models
{
    public class VariantaRaspuns
    {
        public int Id { get; set; }
        public int IntrebareQuizId { get; set; }

        public string Text { get; set; }
        public bool EsteCorect { get; set; }

        public IntrebareQuiz Intrebare { get; set; }
    }
}
