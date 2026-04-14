namespace TicketBookingSystem.Models.Enums;

[Flags]
public enum EventCategory
{
    None = 0,
    Music = 1 << 0,
    Art = 1 << 1,
    ActiveLeisure = 1 << 2,
    Education = 1 << 3,
    Food = 1 << 4,
    Technology = 1 << 5
}


