using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AplicatieLicenta.Models;
namespace AplicatieLicenta.Models
{
    public class Gen
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Denumire { get; set; }
        public ICollection<Carti> Carti { get; set; }=new List<Carti>();
    }
}
