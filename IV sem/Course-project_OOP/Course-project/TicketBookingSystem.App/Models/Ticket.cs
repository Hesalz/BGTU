using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.Models;

public class Ticket
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int? BookingId { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Available;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Event Event { get; set; } = null!;
    public virtual Booking? Booking { get; set; }
}


