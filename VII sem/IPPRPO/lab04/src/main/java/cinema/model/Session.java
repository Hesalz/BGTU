package cinema.model;

import java.time.LocalDateTime;
import java.util.HashSet;
import java.util.Set;

public class Session {
    private final int id;
    private final Movie movie;
    private final LocalDateTime startTime;
    private final String hall;
    private final double price;
    private final int totalSeats;
    private final Set<Integer> occupiedSeats = new HashSet<>();

    public Session(int id, Movie movie, LocalDateTime startTime, String hall,
                   double price, int totalSeats) {
        if (movie == null) {
            throw new IllegalArgumentException("Фильм не может быть null");
        }
        if (startTime == null) {
            throw new IllegalArgumentException("Время начала не может быть null");
        }
        if (hall == null || hall.trim().isEmpty()) {
            throw new IllegalArgumentException("Зал не может быть пустым");
        }
        if (price <= 0) {
            throw new IllegalArgumentException("Цена должна быть больше 0");
        }
        if (totalSeats <= 0) {
            throw new IllegalArgumentException("Количество мест должно быть больше 0");
        }

        this.id = id;
        this.movie = movie;
        this.startTime = startTime;
        this.hall = hall;
        this.price = price;
        this.totalSeats = totalSeats;
    }

    public int getId() {
        return id;
    }

    public Movie getMovie() {
        return movie;
    }

    public LocalDateTime getStartTime() {
        return startTime;
    }

    public String getHall() {
        return hall;
    }

    public double getPrice() {
        return price;
    }

    public int getTotalSeats() {
        return totalSeats;
    }

    public boolean isSeatAvailable(int seatNumber) {
        return seatNumber >= 1
                && seatNumber <= totalSeats
                && !occupiedSeats.contains(seatNumber);
    }

    public void occupySeat(int seatNumber) {
        if (!isSeatAvailable(seatNumber)) {
            throw new IllegalArgumentException("Место недоступно: " + seatNumber);
        }
        occupiedSeats.add(seatNumber);
    }

    public int getAvailableSeats() {
        return totalSeats - occupiedSeats.size();
    }

    @Override
    public String toString() {
        return "Сеанс №" + id + ": " + movie.getTitle()
                + ", " + startTime
                + ", зал " + hall
                + ", цена " + price
                + ", свободно мест: " + getAvailableSeats();
    }
}
