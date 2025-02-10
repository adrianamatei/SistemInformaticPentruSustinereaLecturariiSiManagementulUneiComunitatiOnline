using System;
namespace AplicatieLicenta.Models
{
    public class UsersActivity
    {
        public int Id { get; set; }
        public int   UserId { get; set; }
        public string Action{ get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
