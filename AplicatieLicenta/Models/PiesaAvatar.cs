namespace AplicatieLicenta.Models
{
    public class PiesaAvatar
    {
        public int Id { get; set; }
        public int AvatarId { get; set; }

        public string Nume { get; set; }
        public string ImagineUrl { get; set; }

        public Avatar Avatar { get; set; }
    }
}
