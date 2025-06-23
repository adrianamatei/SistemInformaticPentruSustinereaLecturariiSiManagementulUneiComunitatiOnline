namespace AplicatieLicenta.Models
{
    public class CartiPreferate
    {
        public int Id { get; set; }

        public int IdUtilizator { get; set; }
        public User Utilizator { get; set; } = null!;

        public int IdCarte { get; set; }
        public Carti Carte { get; set; } = null!;
    }
}
