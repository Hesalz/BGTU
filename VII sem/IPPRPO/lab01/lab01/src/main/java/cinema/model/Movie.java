package cinema.model;

public class Movie {
    private final int id;
    private final String title;
    private final int durationMinutes;
    private final String genre;

    public Movie(int id, String title, int durationMinutes, String genre) {
        if (title == null || title.trim().isEmpty()) {
            throw new IllegalArgumentException("Название фильма не может быть пустым");
        }
        if (durationMinutes <= 0) {
            throw new IllegalArgumentException("Длительность фильма должна быть больше 0");
        }
        if (genre == null || genre.trim().isEmpty()) {
            throw new IllegalArgumentException("Жанр фильма не может быть пустым");
        }

        this.id = id;
        this.title = title;
        this.durationMinutes = durationMinutes;
        this.genre = genre;
    }

    public int getId() {
        return id;
    }

    public String getTitle() {
        return title;
    }

    public int getDurationMinutes() {
        return durationMinutes;
    }

    public String getGenre() {
        return genre;
    }

    @Override
    public String toString() {
        return title + " (" + genre + ", " + durationMinutes + " мин.)";
    }
}
