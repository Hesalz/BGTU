package cinema.service;

import cinema.model.Session;
import cinema.model.Ticket;
import cinema.repository.TicketRepository;
import io.micrometer.core.instrument.Counter;
import io.micrometer.core.instrument.MeterRegistry;
import io.micrometer.core.instrument.Timer;
import io.micrometer.core.instrument.simple.SimpleMeterRegistry;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class TicketService {
    private static final Logger LOGGER = LoggerFactory.getLogger(TicketService.class);

    private final TicketRepository ticketRepository;
    private final Counter operations;
    private final Counter successfulOperations;
    private final Counter errorOperations;
    private final Counter ticketsSold;
    private final Timer purchaseDuration;
    private int nextTicketId = 1;

    public TicketService(TicketRepository ticketRepository) {
        this(ticketRepository, new SimpleMeterRegistry());
    }

    public TicketService(TicketRepository ticketRepository, MeterRegistry meterRegistry) {
        if (ticketRepository == null) {
            throw new IllegalArgumentException("TicketRepository must not be null");
        }
        if (meterRegistry == null) {
            throw new IllegalArgumentException("MeterRegistry must not be null");
        }

        this.ticketRepository = ticketRepository;
        this.operations = meterRegistry.counter("cinema.ticket.operations");
        this.successfulOperations = meterRegistry.counter("cinema.ticket.operations.success");
        this.errorOperations = meterRegistry.counter("cinema.ticket.operations.error");
        this.ticketsSold = meterRegistry.counter("cinema.tickets.sold");
        this.purchaseDuration = meterRegistry.timer("cinema.ticket.purchase.duration");
    }

    public Ticket sellTicket(Session session, int seatNumber, String buyer) {
        operations.increment();
        LOGGER.info("Ticket purchase requested: session {}, seat {}, buyer {}",
                session == null ? "not specified" : session.getId(), seatNumber, buyer);

        return purchaseDuration.record(() -> {
            try {
                Ticket ticket = performPurchase(session, seatNumber, buyer);
                successfulOperations.increment();
                ticketsSold.increment();
                LOGGER.info("Ticket #{} successfully sold: session {}, seat {}, buyer {}",
                        ticket.getId(), session.getId(), seatNumber, buyer);
                return ticket;
            } catch (RuntimeException exception) {
                errorOperations.increment();
                LOGGER.warn("Ticket purchase failed: {}", exception.getMessage());
                throw exception;
            }
        });
    }

    private Ticket performPurchase(Session session, int seatNumber, String buyer) {
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
        return ticket;
    }
}
