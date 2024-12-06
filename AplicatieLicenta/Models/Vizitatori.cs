using System;
using System.Collections.Generic;

namespace AplicatieLicenta.Models;

public partial class Vizitatori
{
    public int IdVizita { get; set; }

    public DateTime? DataVizitei { get; set; }

    public int? IdUtilizator { get; set; }

    public string TipUtilizator { get; set; } = null!;

    public virtual User? IdUtilizatorNavigation { get; set; }
}
