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
    public string? TokenResetare { get; set; }
    public int ScorTotal { get; set; } = 0;


    public DateTime? ExpirareToken { get; set; } 

    public virtual ICollection<CluburiLectura> CluburiLectura { get; set; } = new List<CluburiLectura>();

    public virtual ICollection<MembriClub> MembriClub { get; set; } = new List<MembriClub>();

    public virtual ICollection<Recenzii> Recenzii { get; set; } = new List<Recenzii>();
    public virtual ICollection<CartiPreferate> CartiPreferate { get; set; } = new List<CartiPreferate>();




}
