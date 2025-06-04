using System;
using System.Collections.Generic;

namespace AplicatieLicenta.Models;

public partial class Carti
{
    public int IdCarte { get; set; }

    public string Titlu { get; set; } = null!;

    public string Autor { get; set; } = null!;

    public TimeSpan? DurataAscultare { get; set; }

    public string ImagineCoperta { get; set; } = null!;

    public DateTime? DataAdaugarii { get; set; }

    public string UrlFisier { get; set; } = null!;

    public string TipCarte { get; set; } = null!;

   
    public List<CategorieVarsta> CategoriiVarsta { get; set; } = new List<CategorieVarsta>();
    public List<Gen> Genuri { get; set; } = new List<Gen>();

    public virtual ICollection<Recenzii> Recenzii { get; set; } = new List<Recenzii>();

}
