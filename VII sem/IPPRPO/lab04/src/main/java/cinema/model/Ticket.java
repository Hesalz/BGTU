package cinema.model;

public class Ticket {
    private final int id;
    private final Session session;
    private final int seatNumber;
    private final String buyer;

    public Ticket(int id, Session session, int seatNumber, String buyer) {
        if (session == null) {
            throw new IllegalArgumentException("Сеанс не может быть null");
        }
        if (buyer == null || buyer.trim().isEmpty()) {
            throw new IllegalArgumentException("Покупатель не может быть пустым");
        }

        this.id = id;
        this.session = session;
        this.seatNumber = seatNumber;
        this.buyer = buyer;
    }

    public int getId() {
        return id;
    }

    public Session getSession() {
        return session;
    }

    public int getSeatNumber() {
        return seatNumber;
    }

    public String getBuyer() {
        return buyer;
    }

    @Override
    public String toString() {
        return "Билет №" + id
                + ": фильм " + session.getMovie().getTitle()
                + ", сеанс №" + session.getId()
                + ", место " + seatNumber
                + ", покупатель: " + buyer;
    }
}
