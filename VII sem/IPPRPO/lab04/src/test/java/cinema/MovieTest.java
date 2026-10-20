package cinema;

import cinema.model.Movie;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class MovieTest {

    @Test
    void shouldReturnCorrectMovieInformation() {
        Movie movie = new Movie(2, "Дюна", 155, "Фантастика");

        assertEquals(2, movie.getId());
        assertEquals("Дюна", movie.getTitle());
        assertEquals(155, movie.getDurationMinutes());
        assertEquals("Фантастика", movie.getGenre());
        assertEquals("Дюна (Фантастика, 155 мин.)", movie.toString());
    }
}
