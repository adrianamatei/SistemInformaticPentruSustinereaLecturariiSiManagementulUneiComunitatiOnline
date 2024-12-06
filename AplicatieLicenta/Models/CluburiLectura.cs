using System;
using System.Collections.Generic;

namespace AplicatieLicenta.Models;

public partial class CluburiLectura
{
    public int IdClub { get; set; }

    public string Nume { get; set; } = null!;

    public string Descriere { get; set; } = null!;

    public int IdCreator { get; set; }

    public DateTime? DataCrearii { get; set; }

    public virtual User IdCreatorNavigation { get; set; } = null!;

    public virtual ICollection<MembriClub> MembriClubs { get; set; } = new List<MembriClub>();
}
