using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicatieLicenta.Models
{
    public class MesajClub
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Club))]
        public int IdClub { get; set; }

        [ForeignKey(nameof(Utilizator))]
        public int IdUtilizator { get; set; }

        public string Continut { get; set; } = "";
        public DateTime DataTrimiterii { get; set; }

        public virtual CluburiLectura Club { get; set; } = null!;
        public virtual User Utilizator { get; set; } = null!;
    }
}
