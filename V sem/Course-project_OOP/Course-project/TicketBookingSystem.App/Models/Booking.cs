using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.Models;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public int TicketCount { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Event Event { get; set; } = null!;
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}


