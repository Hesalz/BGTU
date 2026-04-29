using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string Location { get; set; } = string.Empty;
    public int TotalTickets { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public EventCategory Categories { get; set; } = EventCategory.None;

    public virtual ICollection<EventPhoto> Photos { get; set; } = new List<EventPhoto>();

    // Navigation properties
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    [NotMapped]
    public byte[]? PrimaryPhoto => Photos
        .OrderBy(p => p.SortOrder)
        .FirstOrDefault()
        ?.ImageData;
}


