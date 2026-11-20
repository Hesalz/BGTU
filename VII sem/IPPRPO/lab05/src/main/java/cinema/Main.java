package cinema;

import cinema.model.Movie;
import cinema.model.Session;
import cinema.model.Ticket;
import cinema.repository.InMemoryTicketRepository;
import cinema.repository.TicketRepository;
import cinema.service.TicketService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.time.LocalDateTime;

public class Main {
    private static final Logger LOGGER = LoggerFactory.getLogger(Main.class);

    public static void main(String[] args) {
        Movie movie = new Movie(1, "Interstellar", 169, "Science Fiction");
        Session session = new Session(
                1,
                movie,
                LocalDateTime.of(2026, 9, 24, 19, 30),
                "Hall 1",
                12.50,
                10
        );

        TicketRepository repository = new InMemoryTicketRepository();
        TicketService ticketService = new TicketService(repository);
        LOGGER.info("Cinema application started");
        LOGGER.info("Selected movie: {}", movie);
        LOGGER.info("Selected session: {}", session);

        Ticket ticket = ticketService.sellTicket(session, 5, "John Smith");
        LOGGER.info("Purchase result: {}", ticket);
        LOGGER.info("Available seats after purchase: {}", session.getAvailableSeats());

        try {
            ticketService.sellTicket(session, 5, "Peter Brown");
        } catch (IllegalArgumentException exception) {
            LOGGER.info("Repeated purchase was correctly rejected: {}", exception.getMessage());
        }

        LOGGER.info("Total tickets sold: {}", repository.findAll().size());
    }
}
