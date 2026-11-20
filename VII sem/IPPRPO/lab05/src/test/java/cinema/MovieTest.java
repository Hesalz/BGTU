package cinema;

import cinema.model.Movie;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class MovieTest {

    @Test
    void shouldReturnCorrectMovieInformation() {
        Movie movie = new Movie(2, "Dune", 155, "Science Fiction");

        assertEquals(2, movie.getId());
        assertEquals("Dune", movie.getTitle());
        assertEquals(155, movie.getDurationMinutes());
        assertEquals("Science Fiction", movie.getGenre());
        assertEquals("Dune (Science Fiction, 155 min.)", movie.toString());
    }
}
