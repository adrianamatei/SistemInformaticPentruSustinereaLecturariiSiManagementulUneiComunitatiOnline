using System;
using System.Collections.Generic;

namespace AplicatieLicenta.Models;

public partial class Recenzii
{
    public int IdRecenzie { get; set; }

    public int IdCarte { get; set; }

    public int? IdUtilizator { get; set; }

    public string EmailUtilizator { get; set; } = null!;

    public int? Rating { get; set; }

    public string Comentariu { get; set; } = null!;

    public DateOnly? DataPublicarii { get; set; }

    public virtual Carti IdCarteNavigation { get; set; } = null!;

    public virtual User? IdUtilizatorNavigation { get; set; }
}
