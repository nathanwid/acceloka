using System;
using System.Collections.Generic;

namespace Acceloka.Entities;

public partial class Ticket
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public int Quota { get; set; }

    public DateTime Date { get; set; }

    public int CategoryId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTimeOffset UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Detail> Details { get; set; } = new List<Detail>();
}
