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
import java.util.Collections;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertThrows;

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
                "Interstellar",
                169,
                "Science Fiction"
        );

        return new Session(
                1,
                movie,
                LocalDateTime.of(2026, 9, 24, 19, 30),
                "Hall 1",
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
        Ticket ticket = ticketService.sellTicket(session, 5, "John Smith");

        assertNotNull(ticket);
        assertEquals(5, ticket.getSeatNumber());
        assertEquals("John Smith", ticket.getBuyer());
        assertFalse(session.isSeatAvailable(5));
        assertEquals(9, session.getAvailableSeats());
    }

    @Test
    void shouldRejectSecondPurchaseOfSameSeatBySameBuyer() {
        ticketService.sellTicket(session, 5, "John Smith");

        IllegalArgumentException exception = assertThrows(
                IllegalArgumentException.class,
                () -> ticketService.sellTicket(session, 5, "John Smith")
        );

        assertEquals("Seat 5 is unavailable", exception.getMessage());
        assertEquals(9, session.getAvailableSeats());
    }

    @Test
    void shouldRejectPurchaseOfSeatOccupiedByAnotherBuyer() {
        ticketService.sellTicket(session, 5, "John Smith");

        IllegalArgumentException exception = assertThrows(
                IllegalArgumentException.class,
                () -> ticketService.sellTicket(session, 5, "Peter Brown")
        );

        assertEquals("Seat 5 is unavailable", exception.getMessage());
        assertEquals(9, session.getAvailableSeats());
    }

    @Test
    void shouldRejectPurchaseForSeatOutOfRange() {
        IllegalArgumentException exception = assertThrows(
                IllegalArgumentException.class,
                () -> ticketService.sellTicket(session, 11, "John Smith")
        );

        assertEquals("Seat 11 is unavailable", exception.getMessage());
        assertEquals(10, session.getAvailableSeats());
    }

    @Test
    void shouldLogAndPropagateRepositoryFailure() {
        TicketRepository failingRepository = new TicketRepository() {
            @Override
            public void save(Ticket ticket) {
                throw new IllegalStateException("Ticket storage failed");
            }

            @Override
            public Ticket findBySessionAndSeat(Session targetSession, int seatNumber) {
                return null;
            }

            @Override
            public List<Ticket> findAll() {
                return Collections.emptyList();
            }
        };

        TicketService failingService = new TicketService(failingRepository);

        assertThrows(
                IllegalStateException.class,
                () -> failingService.sellTicket(session, 3, "John Smith")
        );
        assertEquals(10, session.getAvailableSeats());
    }

}
