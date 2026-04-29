using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketBookingSystem.Models;

public class EventPhoto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public byte[] ImageData { get; set; } = Array.Empty<byte>();
    public int SortOrder { get; set; }

    [ForeignKey(nameof(EventId))]
    public virtual Event Event { get; set; } = null!;
}


