using System;
using System.Collections.Generic;

namespace AplicatieLicenta.Models;

public partial class MembriClub
{
    public int IdMembru { get; set; }

    public int IdClub { get; set; }

    public int IdUtilizator { get; set; }

    public DateTime? DataInscrierii { get; set; }
    public string Status { get; set; } = "In asteptare";

    public virtual CluburiLectura IdClubNavigation { get; set; } = null!;

    public virtual User IdUtilizatorNavigation { get; set; } = null!;
}
