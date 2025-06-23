namespace AplicatieLicenta.Models
{
    public class RecomandareZilnica
    {
        public int Id { get; set; }
        public int CarteId { get; set; }
        public DateTime DataGenerare { get; set; }

        public Carti Carte { get; set; } 
    }
}
