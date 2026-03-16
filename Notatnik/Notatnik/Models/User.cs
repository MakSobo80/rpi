using System;
using System.Collections.Generic;

namespace Notatnik.Models;

public partial class User
{
    public byte Id { get; set; }

    public string Username { get; set; } = null!;

    public bool IsManager { get; set; }

    public byte OrganizationId { get; set; }

    public virtual ICollection<Filez> Filezs { get; set; } = new List<Filez>();

    public virtual Organization Organization { get; set; } = null!;
}
