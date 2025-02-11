using System;
using System.Collections.Generic;

namespace AplicatieLicenta.Models;

public partial class Carti
{
    public int IdCarte { get; set; }

    public string Titlu { get; set; } = null!;

    public string Autor { get; set; } = null!;

    public TimeOnly? DurataAscultare { get; set; }

    public string ImagineCoperta { get; set; } = null!;

    public DateTime? DataAdaugarii { get; set; }

    public string UrlFisier { get; set; } = null!;

    public string TipCarte { get; set; } = null!;

    public string CategorieVarsta { get; set; } = null!;

    public virtual ICollection<Recenzii> Recenzii { get; set; } = new List<Recenzii>();
}
