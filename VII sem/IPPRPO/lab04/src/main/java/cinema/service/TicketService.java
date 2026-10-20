package cinema.service;

import cinema.model.Session;
import cinema.model.Ticket;
import cinema.repository.TicketRepository;

public class TicketService {
    private final TicketRepository ticketRepository;
    private int nextTicketId = 1;

    public TicketService(TicketRepository ticketRepository) {
        if (ticketRepository == null) {
            throw new IllegalArgumentException("TicketRepository не может быть null");
        }
        this.ticketRepository = ticketRepository;
    }

    public Ticket sellTicket(Session session, int seatNumber, String buyer) {
        if (session == null) {
            throw new IllegalArgumentException("Сеанс не может быть null");
        }

        if (!session.isSeatAvailable(seatNumber)) {
            throw new IllegalArgumentException("Место " + seatNumber + " недоступно");
        }

        if (ticketRepository.findBySessionAndSeat(session, seatNumber) != null) {
            throw new IllegalArgumentException("Билет на место " + seatNumber + " уже продан");
        }

        Ticket ticket = new Ticket(nextTicketId++, session, seatNumber, buyer);
        session.occupySeat(seatNumber);
        ticketRepository.save(ticket);

        return ticket;
    }
}
