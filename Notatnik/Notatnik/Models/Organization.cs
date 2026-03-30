using System;
using System.Collections.Generic;

namespace Notatnik.Models;

public partial class Organization
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Filez> Filezs { get; set; } = new List<Filez>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
