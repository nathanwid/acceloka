using System;
using System.Collections.Generic;

namespace Acceloka.Entities;

public partial class Detail
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public string TicketCode { get; set; } = null!;

    public int BookedTicketId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTimeOffset UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual BookedTicket BookedTicket { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
