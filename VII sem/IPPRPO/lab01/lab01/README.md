# Лабораторная работа №1 — Cinema

Вариант 2: кинотеатр.

## Структура

- `src/main/java/cinema/model/Movie.java` — фильм.
- `src/main/java/cinema/model/Session.java` — сеанс и занятые места.
- `src/main/java/cinema/model/Ticket.java` — билет.
- `src/main/java/cinema/repository/TicketRepository.java` — интерфейс хранилища билетов.
- `src/main/java/cinema/repository/InMemoryTicketRepository.java` — реализация хранилища в памяти.
- `src/main/java/cinema/service/TicketService.java` — бизнес-логика продажи билета.
- `src/main/java/cinema/Main.java` — демонстрация работы программы.
- `src/test/java/cinema/CinemaTest.java` — 3 Unit-теста.

## Dependency Injection

`TicketService` не создаёт репозиторий самостоятельно. Репозиторий передаётся в конструктор:

```java
TicketRepository repository = new InMemoryTicketRepository();
TicketService ticketService = new TicketService(repository);
```

Поэтому `TicketService` зависит от интерфейса `TicketRepository`, а не от конкретного класса.

## Индивидуальное задание

Перед продажей билета проверяется, доступно ли выбранное место. Повторная покупка уже занятого места и покупка места вне диапазона отклоняются.

## Unit-тесты

В `CinemaTest.java` проверяются:

1. успешная продажа свободного места;
2. отказ при повторной покупке занятого места;
3. отказ при выборе несуществующего места.

### Если IntelliJ подсвечивает `org.junit.jupiter`

Проект намеренно оставлен обычным IntelliJ Java-проектом без Maven (`pom.xml` не нужен).

В IntelliJ IDEA:
`File → Project Structure → Modules → Dependencies → + → Library → From Maven...`

Найдите и добавьте:

`org.junit.jupiter:junit-jupiter:5.10.2`

После этого `CinemaTest` запускается как обычный JUnit 5 test.
