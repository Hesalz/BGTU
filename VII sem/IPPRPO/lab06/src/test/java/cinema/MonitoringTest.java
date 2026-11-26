package cinema;

import cinema.model.Movie;
import cinema.model.Session;
import cinema.repository.InMemoryTicketRepository;
import cinema.service.TicketService;
import io.micrometer.core.instrument.simple.SimpleMeterRegistry;
import org.junit.jupiter.api.Test;

import java.time.LocalDateTime;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class MonitoringTest {

    @Test
    void shouldUpdateCountersForSuccessfulAndFailedPurchases() {
        SimpleMeterRegistry registry = new SimpleMeterRegistry();
        Session session = new Session(
                1,
                new Movie(1, "Interstellar", 169, "Science Fiction"),
                LocalDateTime.of(2026, 9, 24, 19, 30),
                "Hall 1",
                12.50,
                10
        );
        TicketService service = new TicketService(new InMemoryTicketRepository(), registry);

        service.sellTicket(session, 5, "John Smith");
        assertThrows(IllegalArgumentException.class,
                () -> service.sellTicket(session, 5, "Peter Brown"));

        assertEquals(2.0, registry.get("cinema.ticket.operations").counter().count());
        assertEquals(1.0, registry.get("cinema.ticket.operations.success").counter().count());
        assertEquals(1.0, registry.get("cinema.ticket.operations.error").counter().count());
        assertEquals(1.0, registry.get("cinema.tickets.sold").counter().count());
        assertEquals(2L, registry.get("cinema.ticket.purchase.duration").timer().count());

        registry.close();
    }
}
