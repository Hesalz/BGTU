package cinema;

import cinema.model.Movie;
import cinema.model.Session;
import cinema.repository.InMemoryTicketRepository;
import cinema.repository.TicketRepository;
import cinema.service.TicketService;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpServer;
import io.micrometer.prometheusmetrics.PrometheusConfig;
import io.micrometer.prometheusmetrics.PrometheusMeterRegistry;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.net.URLDecoder;
import java.nio.charset.StandardCharsets;
import java.time.LocalDateTime;
import java.util.HashMap;
import java.util.Map;

public class Main {
    private static final Logger LOGGER = LoggerFactory.getLogger(Main.class);
    private static final int PORT = 8081;

    private Main() {
    }

    public static void main(String[] args) throws IOException {
        Movie movie = new Movie(1, "Interstellar", 169, "Science Fiction");
        Session session = new Session(
                1, movie, LocalDateTime.of(2026, 9, 24, 19, 30),
                "Hall 1", 12.50, 10
        );

        TicketRepository repository = new InMemoryTicketRepository();
        PrometheusMeterRegistry registry = new PrometheusMeterRegistry(PrometheusConfig.DEFAULT);
        TicketService ticketService = new TicketService(repository, registry);

        HttpServer server = HttpServer.create(new InetSocketAddress(PORT), 0);
        server.createContext("/metrics", exchange -> respond(exchange, 200, registry.scrape()));
        server.createContext("/api/status", exchange -> respond(exchange, 200,
                "Cinema monitoring application is running. Available seats: "
                        + session.getAvailableSeats()));
        server.createContext("/api/sell", exchange -> handleSale(exchange, ticketService, session));
        server.setExecutor(null);
        server.start();

        LOGGER.info("Cinema application started on port {}", PORT);
        LOGGER.info("Metrics endpoint: http://localhost:{}/metrics", PORT);
        LOGGER.info("Example purchase: http://localhost:{}/api/sell?seat=5&buyer=John", PORT);
        LOGGER.info("Status endpoint: http://localhost:{}/api/status", PORT);
        Runtime.getRuntime().addShutdownHook(new Thread(() -> {
            server.stop(0);
            registry.close();
        }));
    }

    private static void handleSale(HttpExchange exchange, TicketService ticketService, Session session)
            throws IOException {
        if (!"GET".equalsIgnoreCase(exchange.getRequestMethod())) {
            respond(exchange, 405, "Use GET");
            return;
        }

        Map<String, String> parameters = parseQuery(exchange.getRequestURI().getRawQuery());
        try {
            int seat = Integer.parseInt(parameters.getOrDefault("seat", ""));
            String buyer = parameters.getOrDefault("buyer", "");
            respond(exchange, 200, "Ticket sold: " + ticketService.sellTicket(
                    session, seat, buyer));
        } catch (NumberFormatException exception) {
            respond(exchange, 400, "Parameter 'seat' must be an integer");
        } catch (IllegalArgumentException exception) {
            respond(exchange, 400, "Purchase rejected: " + exception.getMessage());
        } catch (RuntimeException exception) {
            LOGGER.error("Unexpected purchase error", exception);
            respond(exchange, 500, "Internal application error");
        }
    }

    private static Map<String, String> parseQuery(String query) {
        Map<String, String> parameters = new HashMap<>();
        if (query == null || query.isEmpty()) {
            return parameters;
        }
        for (String pair : query.split("&")) {
            String[] parts = pair.split("=", 2);
            String key = URLDecoder.decode(parts[0], StandardCharsets.UTF_8);
            String value = parts.length > 1
                    ? URLDecoder.decode(parts[1], StandardCharsets.UTF_8) : "";
            parameters.put(key, value);
        }
        return parameters;
    }

    private static void respond(HttpExchange exchange, int status, String body) throws IOException {
        byte[] response = body.getBytes(StandardCharsets.UTF_8);
        exchange.getResponseHeaders().set("Content-Type", "text/plain; charset=UTF-8");
        exchange.sendResponseHeaders(status, response.length);
        try (var output = exchange.getResponseBody()) {
            output.write(response);
        }
    }
}
