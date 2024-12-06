using System;
using System.Collections.Generic;

namespace AplicatieLicenta.Models;

public partial class Admin
{
    public int IdAdmin { get; set; }

    public string Email { get; set; } = null!;

    public string Parola { get; set; } = null!;
}
