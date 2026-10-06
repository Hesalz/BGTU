package cinema;

import cinema.model.Movie;
import cinema.model.Session;
import cinema.model.Ticket;
import cinema.repository.InMemoryTicketRepository;
import cinema.repository.TicketRepository;
import cinema.service.TicketService;

import java.time.LocalDateTime;

public class Main {
    public static void main(String[] args) {
        Movie movie = new Movie(
                1,
                "Интерстеллар",
                169,
                "Фантастика"
        );

        Session session = new Session(
                1,
                movie,
                LocalDateTime.of(2026, 9, 24, 19, 30),
                "Зал 1",
                12.50,
                10
        );

        TicketRepository repository = new InMemoryTicketRepository();
        TicketService ticketService = new TicketService(repository);

        System.out.println(movie);
        System.out.println(session);

        Ticket firstTicket = ticketService.sellTicket(session, 5, "Иван Иванов");
        System.out.println("\nБилет успешно продан:");
        System.out.println(firstTicket);
        System.out.println("Свободно мест: " + session.getAvailableSeats());

        System.out.println("\nПопытка повторно купить место 5:");
        try {
            ticketService.sellTicket(session, 5, "Петр Петров");
        } catch (IllegalArgumentException e) {
            System.out.println("Покупка отклонена: " + e.getMessage());
        }

        System.out.println("\nВсе проданные билеты:");
        for (Ticket ticket : repository.findAll()) {
            System.out.println(ticket);
        }
    }
}
