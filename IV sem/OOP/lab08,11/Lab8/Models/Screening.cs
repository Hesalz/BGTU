using System;

namespace CinemaManagement.Models
{
    public class Screening
    {
        public int ScreeningID { get; set; }
        public int FilmID { get; set; }
        public string FilmTitle { get; set; }
        public DateTime ScreeningDateTime { get; set; }
        public int HallNumber { get; set; }
        public decimal TicketPrice { get; set; }
    }
}