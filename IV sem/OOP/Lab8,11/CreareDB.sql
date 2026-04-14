-- Создаем базу данных Cinema
CREATE DATABASE Cinema;
GO

USE Cinema;
GO

-- Таблица Films
CREATE TABLE Films (
    FilmID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    Director NVARCHAR(100),
    ReleaseYear INT,
    Genre NVARCHAR(50),
    Duration INT, -- в минутах
    Poster VARBINARY(MAX) -- для хранения графических данных (постеров фильмов)
);

-- Таблица Screenings
CREATE TABLE Screenings (
    ScreeningID INT PRIMARY KEY IDENTITY(1,1),
    FilmID INT NOT NULL,
    ScreeningDateTime DATETIME NOT NULL,
    HallNumber INT NOT NULL,
    TicketPrice DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (FilmID) REFERENCES Films(FilmID) ON DELETE CASCADE
);
GO

-- Создаем триггер для проверки даты сеанса
CREATE TRIGGER trg_CheckScreeningDate
ON Screenings
AFTER INSERT, UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM inserted 
        WHERE ScreeningDateTime < GETDATE()
    )
    BEGIN
        RAISERROR('Дата сеанса не может быть в прошлом', 16, 1)
        ROLLBACK TRANSACTION
    END
END;
GO

-- Хранимая процедура для добавления фильма
CREATE OR ALTER PROCEDURE sp_AddFilm
    @Title NVARCHAR(100),
    @Director NVARCHAR(100),
    @ReleaseYear INT,
    @Genre NVARCHAR(50),
    @Duration INT,
    @Poster VARBINARY(MAX) = NULL
AS
BEGIN
    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Films (Title, Director, ReleaseYear, Genre, Duration, Poster)
        VALUES (@Title, @Director, @ReleaseYear, @Genre, @Duration, @Poster)
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
END;
GO

-- Хранимая процедура для поиска фильмов по жанру
CREATE OR ALTER PROCEDURE sp_GetFilmsByGenre
    @Genre NVARCHAR(50)
AS
BEGIN
    SELECT * FROM Films 
    WHERE Genre = @Genre
    ORDER BY Title
END;
GO

-- Хранимая процедура для получения всех сеансов на определенную дату
CREATE OR ALTER PROCEDURE sp_GetScreeningsByDate @Date DATE
AS
BEGIN
    SELECT s.ScreeningID, s.FilmID, f.Title AS FilmTitle, s.ScreeningDateTime, s.HallNumber, s.TicketPrice
    FROM Screenings s
    JOIN Films f ON s.FilmID = f.FilmID
    WHERE CAST(s.ScreeningDateTime AS DATE) = @Date
    ORDER BY s.ScreeningDateTime
END;


-- Вставка данных в таблицу Films
INSERT INTO Films (Title, Director, ReleaseYear, Genre, Duration, Poster)
VALUES ('Inception', 'Christopher Nolan', 2010, 'Sci-Fi', 148, NULL);

INSERT INTO Films (Title, Director, ReleaseYear, Genre, Duration, Poster)
VALUES ('The Shawshank Redemption', 'Frank Darabont', 1994, 'Drama', 142, NULL);

INSERT INTO Films (Title, Director, ReleaseYear, Genre, Duration, Poster)
VALUES ('The Dark Knight', 'Christopher Nolan', 2008, 'Action', 152, NULL);

INSERT INTO Films (Title, Director, ReleaseYear, Genre, Duration, Poster)
VALUES ('Pulp Fiction', 'Quentin Tarantino', 1994, 'Crime', 154, NULL);

INSERT INTO Films (Title, Director, ReleaseYear, Genre, Duration, Poster)
VALUES ('Fight Club', 'David Fincher', 1999, 'Drama', 139, NULL);

GO

-- Вставка данных в таблицу Screenings
INSERT INTO Screenings (FilmID, ScreeningDateTime, HallNumber, TicketPrice)
VALUES (1, '2026-10-15T18:00:00', 1, 10.50);

INSERT INTO Screenings (FilmID, ScreeningDateTime, HallNumber, TicketPrice)
VALUES (2, '2026-10-15T20:00:00', 2, 9.50);

INSERT INTO Screenings (FilmID, ScreeningDateTime, HallNumber, TicketPrice)
VALUES (3, '2026-10-16T19:00:00', 3, 11.00);

INSERT INTO Screenings (FilmID, ScreeningDateTime, HallNumber, TicketPrice)
VALUES (4, '2026-10-16T21:00:00', 1, 10.00);

INSERT INTO Screenings (FilmID, ScreeningDateTime, HallNumber, TicketPrice)
VALUES (5, '2026-10-17T18:30:00', 2, 9.00);