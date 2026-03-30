using System;
using System.Collections.Generic;

namespace Notatnik.Models;

public partial class Filez
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public byte AuthorId { get; set; }

    public byte[] File { get; set; } = null!;

    public byte? Parent { get; set; }

    public byte OrganizationId { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Filez> InverseParentNavigation { get; set; } = new List<Filez>();

    public virtual Organization Organization { get; set; } = null!;

    public virtual Filez? ParentNavigation { get; set; }
}
