namespace AplicatieLicenta.Models
{
    public class Avatar
    {
        public int Id { get; set; }
        public string Nume { get; set; }
        public int Nivel { get; set; }

        public ICollection<PiesaAvatar> Piese { get; set; }
    }
}
