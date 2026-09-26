package cinema;

import cinema.model.Movie;
import cinema.model.Session;
import cinema.model.Ticket;
import cinema.repository.InMemoryTicketRepository;
import cinema.repository.TicketRepository;
import cinema.service.TicketService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.time.LocalDateTime;

import static org.junit.jupiter.api.Assertions.*;

public class CinemaTest {

    private Session session;
    private TicketService ticketService;

    @BeforeEach
    void setUp() {
        session = createSession(10);
        ticketService = createTicketService();
    }

    private Session createSession(int totalSeats) {
        Movie movie = new Movie(
                1,
                "Интерстеллар",
                169,
                "Фантастика"
        );

        return new Session(
                1,
                movie,
                LocalDateTime.of(2026, 9, 24, 19, 30),
                "Зал 1",
                12.50,
                totalSeats
        );
    }

    private TicketService createTicketService() {
        TicketRepository repository = new InMemoryTicketRepository();
        return new TicketService(repository);
    }

    @Test
    void shouldSellTicketForAvailableSeat() {
        Ticket ticket = ticketService.sellTicket(session, 5, "Иван Иванов");

        assertNotNull(ticket);
        assertEquals(5, ticket.getSeatNumber());
        assertEquals("Иван Иванов", ticket.getBuyer());
        assertFalse(session.isSeatAvailable(5));
        assertEquals(9, session.getAvailableSeats());
    }

    @Test
    void shouldRejectSecondPurchaseOfSameSeatBySameBuyer() {
        ticketService.sellTicket(session, 5, "Иван Иванов");

        IllegalArgumentException exception = assertThrows(
                IllegalArgumentException.class,
                () -> ticketService.sellTicket(session, 5, "Иван Иванов")
        );

        assertEquals("Место 5 недоступно", exception.getMessage());
        assertEquals(9, session.getAvailableSeats());
    }

    @Test
    void shouldRejectPurchaseOfSeatOccupiedByAnotherBuyer() {
        ticketService.sellTicket(session, 5, "Иван Иванов");

        IllegalArgumentException exception = assertThrows(
                IllegalArgumentException.class,
                () -> ticketService.sellTicket(session, 5, "Петр Петров")
        );

        assertEquals("Место 5 недоступно", exception.getMessage());
        assertEquals(9, session.getAvailableSeats());
    }

    @Test
    void shouldRejectPurchaseForSeatOutOfRange() {
        IllegalArgumentException exception = assertThrows(
                IllegalArgumentException.class,
                () -> ticketService.sellTicket(session, 11, "Иван Иванов")
        );

        assertEquals("Место 11 недоступно", exception.getMessage());
        assertEquals(10, session.getAvailableSeats());
    }
}