package cinema.repository;

import cinema.model.Session;
import cinema.model.Ticket;

import java.util.List;

public interface TicketRepository {
    void save(Ticket ticket);

    Ticket findBySessionAndSeat(Session session, int seatNumber);

    List<Ticket> findAll();
}
