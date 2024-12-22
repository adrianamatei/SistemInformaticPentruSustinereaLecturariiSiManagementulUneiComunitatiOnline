using System;
using System.Collections.Generic;

namespace AplicatieLicenta.Models;

public partial class User
{
    public int IdUtilizator { get; set; }

    public string Email { get; set; } = null!;

    public string Parola { get; set; } = null!;

    public DateTime? DataInregistrare { get; set; }

    public string TipUtilizator { get; set; } = null!;

    public string CategorieVarsta { get; set; } = null!;
    public int NumarIncercariEsec { get; set; }
    public DateTime? BlocatPanaLa { get; set; }

    public virtual ICollection<CluburiLectura> CluburiLecturas { get; set; } = new List<CluburiLectura>();

    public virtual ICollection<MembriClub> MembriClubs { get; set; } = new List<MembriClub>();

    public virtual ICollection<Recenzii> Recenziis { get; set; } = new List<Recenzii>();

    public virtual ICollection<Vizitatori> Vizitatoris { get; set; } = new List<Vizitatori>();
}
