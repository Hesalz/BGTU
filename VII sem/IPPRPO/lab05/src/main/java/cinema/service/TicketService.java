package cinema.service;

import cinema.model.Session;
import cinema.model.Ticket;
import cinema.repository.TicketRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class TicketService {
    private static final Logger LOGGER = LoggerFactory.getLogger(TicketService.class);

    private final TicketRepository ticketRepository;
    private int nextTicketId = 1;

    public TicketService(TicketRepository ticketRepository) {
        if (ticketRepository == null) {
            throw new IllegalArgumentException("TicketRepository must not be null");
        }
        this.ticketRepository = ticketRepository;
    }

    public Ticket sellTicket(Session session, int seatNumber, String buyer) {
        LOGGER.info("Ticket purchase requested: session {}, seat {}, buyer {}",
                session == null ? "not specified" : session.getId(), seatNumber, buyer);

        if (session == null) {
            throw new IllegalArgumentException("Session must not be null");
        }

        if (!session.isSeatAvailable(seatNumber)) {
            LOGGER.warn("Purchase rejected: seat {} in session {} is unavailable",
                    seatNumber, session.getId());
            throw new IllegalArgumentException("Seat " + seatNumber + " is unavailable");
        }

        if (ticketRepository.findBySessionAndSeat(session, seatNumber) != null) {
            LOGGER.warn("Purchase rejected: seat {} in session {} has already been sold",
                    seatNumber, session.getId());
            throw new IllegalArgumentException(
                    "A ticket for seat " + seatNumber + " has already been sold");
        }

        Ticket ticket = new Ticket(nextTicketId++, session, seatNumber, buyer);

        try {
            ticketRepository.save(ticket);
            session.occupySeat(seatNumber);
        } catch (RuntimeException exception) {
            LOGGER.error("Failed to issue ticket: session {}, seat {}",
                    session.getId(), seatNumber, exception);
            throw exception;
        }

        LOGGER.info("Ticket #{} successfully sold: session {}, seat {}, buyer {}",
                ticket.getId(), session.getId(), seatNumber, buyer);
        return ticket;
    }
}
