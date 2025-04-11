namespace AplicatieLicenta.Models
{
    public class PiesaDeblocata
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public int PiesaAvatarId { get; set; }
        public DateTime DataDeblocare { get; set; }

        public User Utilizator { get; set; }
        public PiesaAvatar PiesaAvatar { get; set; }
    }
}
