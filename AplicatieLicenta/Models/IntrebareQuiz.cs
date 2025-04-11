namespace AplicatieLicenta.Models
{
    public class IntrebareQuiz
    {
        public int Id { get; set; }
        public int QuizId { get; set; }

        public string Enunt { get; set; }
        public string Categorie { get; set; } // PersonajPrincipal, Emotii, Actiune

        public Quiz Quiz { get; set; }
        public ICollection<VariantaRaspuns> Variante { get; set; }
    }
}
