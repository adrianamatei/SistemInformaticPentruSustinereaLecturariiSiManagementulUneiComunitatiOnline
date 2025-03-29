using System.Collections.Generic;
using AplicatieLicenta.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections;
namespace AplicatieLicenta.Models
{
    public class CategorieVarsta
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Denumire {  get; set; }
        public ICollection<Carti> Carti { get; set; } = new List<Carti>();
    }
}
