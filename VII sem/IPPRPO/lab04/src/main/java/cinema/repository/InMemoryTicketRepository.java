package cinema.repository;

import cinema.model.Session;
import cinema.model.Ticket;

import java.util.ArrayList;
import java.util.List;

public class InMemoryTicketRepository implements TicketRepository {
    private final List<Ticket> tickets = new ArrayList<>();

    @Override
    public void save(Ticket ticket) {
        if (ticket == null) {
            throw new IllegalArgumentException("Билет не может быть null");
        }
        tickets.add(ticket);
    }

    @Override
    public Ticket findBySessionAndSeat(Session session, int seatNumber) {
        for (Ticket ticket : tickets) {
            if (ticket.getSession() == session && ticket.getSeatNumber() == seatNumber) {
                return ticket;
            }
        }
        return null;
    }

    @Override
    public List<Ticket> findAll() {
        return new ArrayList<>(tickets);
    }
}
