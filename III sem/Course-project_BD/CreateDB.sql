USE master
GO

CREATE DATABASE Cinema ON
    (NAME = 'Cinema', FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\Cinema.mdf',
    SIZE = 5, MAXSIZE = 100, FILEGROWTH = 1)
LOG ON
    (NAME = 'CinemaLog', FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\CinemaLog.ldf',
    SIZE = 5, MAXSIZE = 100, FILEGROWTH = 1)
GO

USE Cinema
GO

CREATE SCHEMA SCHEMA1;
GO

-- Включаем необходимые компоненты для JSON
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'xp_cmdshell', 1;
RECONFIGURE;
GO

/*Тип для формата телефона клиента"*/

CREATE TYPE Phone
FROM NVARCHAR(13) NOT NULL;
GO

CREATE RULE PhoneRule
AS @x LIKE '+375[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'
GO

EXEC sp_bindrule 'PhoneRule', 'Phone'
GO

/* Создание таблиц */

CREATE TABLE Фильмы (
    ID_фильма INT PRIMARY KEY IDENTITY(1,1),
    Название NVARCHAR(100) NOT NULL,
    Год_выпуска INT NOT NULL,
    Режиссёр NVARCHAR(100) NOT NULL,
    Жанр NVARCHAR(50) NOT NULL,
    Рейтинг NVARCHAR(10),
    Роли NVARCHAR(MAX),
    Страна NVARCHAR(50),
    Описание NVARCHAR(MAX),
    Ограничение NVARCHAR(50)
);
GO

CREATE TABLE Оператор (
    ID_оператора INT PRIMARY KEY IDENTITY(1,1),
    Фамилия NVARCHAR(50) NOT NULL,
    Имя NVARCHAR(50) NOT NULL,
    Отчество NVARCHAR(50),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Телефон Phone NOT NULL,
    Пароль NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Служба_поддержки (
    ID_сотрудника INT PRIMARY KEY IDENTITY(1,1),
    Фамилия NVARCHAR(50) NOT NULL,
    Имя NVARCHAR(50) NOT NULL,
    Отчество NVARCHAR(50),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Телефон Phone NOT NULL,
    Пароль NVARCHAR(100) NOT NULL,
    Количество_обращений INT DEFAULT 0
);
GO

CREATE TABLE Кинотеатр (
    ID_кинотеатра INT PRIMARY KEY IDENTITY(1,1),
    Название NVARCHAR(100) NOT NULL,
    Адрес NVARCHAR(200) NOT NULL,
    Описание NVARCHAR(MAX),
    Режим_работы NVARCHAR(100),
    Контакты NVARCHAR(100),
    ID_оператора INT NOT NULL,
    CONSTRAINT FK_Кинотеатр_Оператор FOREIGN KEY (ID_оператора) REFERENCES Оператор(ID_оператора)
);
GO

CREATE TABLE Зал (
    ID_зала INT PRIMARY KEY IDENTITY(1,1),
    Номер INT NOT NULL,
    Тип_зала NVARCHAR(50),
    ID_кинотеатра INT NOT NULL,
    FOREIGN KEY (ID_кинотеатра) REFERENCES Кинотеатр(ID_кинотеатра)
);
GO

CREATE TABLE Место (
    ID_места INT PRIMARY KEY IDENTITY(1,1),
    ID_зала INT NOT NULL,
    Номер_ряда INT NOT NULL,
    Номер_места INT NOT NULL,
    FOREIGN KEY (ID_зала) REFERENCES Зал(ID_зала)
);
GO

CREATE TABLE Клиент (
    ID_клиента INT PRIMARY KEY IDENTITY(1,1),
    Фамилия NVARCHAR(50) NOT NULL,
    Имя NVARCHAR(50) NOT NULL,
    Отчество NVARCHAR(50),
    Email NVARCHAR(100) UNIQUE,
    Телефон NVARCHAR(20) UNIQUE,
    Пароль NVARCHAR(100) NOT NULL,
    Дата_создания DATE DEFAULT GETDATE()
);
GO

CREATE TABLE Сеанс (
    ID_сеанса INT PRIMARY KEY IDENTITY(1,1),
    Время DATETIME NOT NULL,
    ID_фильма INT NOT NULL,
    ID_зала INT NOT NULL,
    FOREIGN KEY (ID_фильма) REFERENCES Фильмы(ID_фильма),
    FOREIGN KEY (ID_зала) REFERENCES Зал(ID_зала)
);
GO

CREATE TABLE Билет (
    ID_билета INT PRIMARY KEY IDENTITY(1,1),
    Цена MONEY NOT NULL,
    Льгота NVARCHAR(50),
    Дата_время DATETIME NOT NULL,
	Статус NVARCHAR(20) DEFAULT 'Забронирован' NOT NULL,
    ID_сеанса INT NOT NULL,
    ID_места INT NOT NULL,
    ID_клиента INT NOT NULL,
    FOREIGN KEY (ID_сеанса) REFERENCES Сеанс(ID_сеанса),
    FOREIGN KEY (ID_места) REFERENCES Место(ID_места),
    FOREIGN KEY (ID_клиента) REFERENCES Клиент(ID_клиента),
);
GO

CREATE TABLE Возвраты (
    ID_возврата INT PRIMARY KEY IDENTITY(1,1),
    Дата_время DATETIME DEFAULT GETDATE() NOT NULL,
    Статус NVARCHAR(20) DEFAULT 'Не обработан' NOT NULL CHECK (Статус IN ('Обработан', 'Не обработан')),
    ID_билета INT NOT NULL UNIQUE,
    FOREIGN KEY (ID_билета) REFERENCES Билет(ID_билета)
);
GO

CREATE TABLE Отзывы (
    ID_отзыва INT PRIMARY KEY IDENTITY(1,1),
    ID_клиента INT NOT NULL,
    ID_фильма INT NOT NULL,
    Оценка INT CHECK (Оценка BETWEEN 1 AND 10),
    Текст NVARCHAR(MAX),
    Дата DATE DEFAULT GETDATE(),
    FOREIGN KEY (ID_клиента) REFERENCES Клиент(ID_клиента),
    FOREIGN KEY (ID_фильма) REFERENCES Фильмы(ID_фильма)
);
GO

CREATE TABLE Обращения (
    ID_обращения INT PRIMARY KEY IDENTITY(1,1),
    ID_клиента INT NOT NULL,
    Тема NVARCHAR(100) NOT NULL,
    Текст NVARCHAR(MAX) NOT NULL,
    Дата DATE DEFAULT GETDATE(),
    Статус NVARCHAR(50) DEFAULT 'Ожидает',
    Ответ NVARCHAR(MAX),
    ID_сотрудника INT,
    FOREIGN KEY (ID_клиента) REFERENCES Клиент(ID_клиента),
    FOREIGN KEY (ID_сотрудника) REFERENCES Служба_поддержки(ID_сотрудника)
);
GO


/* Все представления */
-- 1. Топ клиентов по количеству купленных билетов
CREATE OR ALTER VIEW Топ_клиентов AS
SELECT TOP(5) 
    c.Фамилия + ' ' + c.Имя AS ФИО_клиента, 
    COUNT(b.ID_билета) AS Количество_билетов
FROM Клиент c
JOIN Билет b ON c.ID_клиента = b.ID_клиента
WHERE b.Статус = 'Активен' OR b.Статус = 'Истекший'
GROUP BY c.Фамилия, c.Имя
ORDER BY COUNT(b.ID_билета) DESC;
GO

-- 2. Количество билетов по залам
CREATE VIEW Кол_билетов_зала AS
SELECT 
    z.ID_зала AS Номер_зала, 
    COUNT(b.ID_билета) AS Количество_билетов
FROM Зал z
LEFT JOIN Сеанс s ON z.ID_зала = s.ID_зала
LEFT JOIN Билет b ON s.ID_сеанса = b.ID_сеанса
GROUP BY z.ID_зала;
GO

-- 3. Топ фильмов по количеству проданных билетов
CREATE VIEW Топ_фильмов AS
SELECT TOP(5) 
    f.Название AS Название_фильма, 
    COUNT(b.ID_билета) AS Количество_билетов
FROM Фильмы f
JOIN Сеанс s ON f.ID_фильма = s.ID_фильма
JOIN Билет b ON s.ID_сеанса = b.ID_сеанса
GROUP BY f.Название
ORDER BY COUNT(b.ID_билета) DESC;
GO

-- 4. Залы без сеансов
CREATE VIEW Залы_без_фильмов AS
SELECT TOP 100 PERCENT
    z.ID_зала AS Номер_зала, 
    (SELECT COUNT(*) FROM Место m WHERE m.ID_зала = z.ID_зала) AS Количество_мест
FROM Зал z
WHERE z.ID_зала NOT IN (SELECT DISTINCT ID_зала FROM Сеанс)
ORDER BY Количество_мест DESC;
GO


-- 5. Свободные места на сеансах
CREATE OR ALTER VIEW Свободные_места AS
SELECT 
    s.ID_сеанса, 
    s.Время, 
    f.Название AS Название_фильма, 
    z.ID_зала AS Номер_зала, 
    m.Номер_ряда, 
    m.Номер_места,
    CASE 
        WHEN b.ID_билета IS NULL THEN 'Свободно'
        ELSE 'Занято'
    END AS Статус_места
FROM Сеанс s
JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
JOIN Зал z ON s.ID_зала = z.ID_зала
JOIN Место m ON z.ID_зала = m.ID_зала
LEFT JOIN Билет b ON b.ID_сеанса = s.ID_сеанса 
    AND b.ID_места = m.ID_места
    AND b.Статус IN ('Забронирован', 'Активен', 'Оплачен')
WHERE b.ID_билета IS NULL;
GO


/* Все функции */
-- Средний рейтинг фильма
CREATE FUNCTION dbo.GetAverageRating(@FilmID INT)
RETURNS FLOAT
AS
BEGIN
    DECLARE @AverageRating FLOAT;

    SELECT @AverageRating = AVG(CAST(Оценка AS FLOAT))
    FROM Отзывы
    WHERE ID_фильма = @FilmID;

    RETURN ISNULL(@AverageRating, 0);
END;
GO

-- Общая выручка по фильму
CREATE FUNCTION dbo.GetMovieRevenue(@MovieID INT)
RETURNS MONEY
AS
BEGIN
    DECLARE @TotalRevenue MONEY;
    
    SELECT @TotalRevenue = SUM(b.Цена)
    FROM Билет b
    JOIN Сеанс s ON b.ID_сеанса = s.ID_сеанса
    WHERE s.ID_фильма = @MovieID;
    
    RETURN ISNULL(@TotalRevenue, 0);
END;
GO

-- Проверка доступности места на сеансе
CREATE OR ALTER FUNCTION dbo.IsSeatAvailable(@SessionID INT, @SeatID INT)
RETURNS BIT
AS
BEGIN
    DECLARE @IsAvailable BIT = 1;
    
    IF EXISTS (
        SELECT 1 FROM Билет 
        WHERE ID_сеанса = @SessionID 
        AND ID_места = @SeatID
        AND Статус IN ('Забронирован', 'Активен')
    )
    BEGIN
        SET @IsAvailable = 0;
    END
    
    RETURN @IsAvailable;
END
GO

-- Общее кол-во свободных мест в зале
CREATE OR ALTER FUNCTION dbo.GetAvailableSeatsCount(@SessionID INT)
RETURNS INT
AS
BEGIN
    DECLARE @TotalSeats INT, @BookedSeats INT, @AvailableSeats INT;
    
    SELECT @TotalSeats = COUNT(*) 
    FROM Место m
    JOIN Зал z ON m.ID_зала = z.ID_зала
    JOIN Сеанс s ON z.ID_зала = s.ID_зала
    WHERE s.ID_сеанса = @SessionID;
    
    SELECT @BookedSeats = COUNT(*) 
    FROM Билет 
    WHERE ID_сеанса = @SessionID
    AND Статус IN ('Забронирован', 'Активен');
    
    SET @AvailableSeats = @TotalSeats - @BookedSeats;
    
    RETURN @AvailableSeats;
END
GO

/* Все процедуры */
-- Общий функционал
CREATE PROCEDURE СортироватьФильмы
    @ПолеСортировки NVARCHAR(50) = 'Название',
    @НаправлениеСортировки NVARCHAR(4) = 'ASC'
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    
    SET @SQL = N'SELECT * FROM Фильмы ORDER BY ' + 
               QUOTENAME(@ПолеСортировки) + ' ' + @НаправлениеСортировки;
    
    EXEC sp_executesql @SQL;
END
GO

CREATE PROCEDURE ФильтроватьФильмы
    @Жанр NVARCHAR(50) = NULL,
    @ГодВыпуска INT = NULL,
    @Рейтинг NVARCHAR(10) = NULL
AS
BEGIN
    SELECT * FROM Фильмы
    WHERE (@Жанр IS NULL OR Жанр = @Жанр)
      AND (@ГодВыпуска IS NULL OR Год_выпуска = @ГодВыпуска)
      AND (@Рейтинг IS NULL OR Рейтинг = @Рейтинг)
    ORDER BY Название;
END
GO

CREATE PROCEDURE ПоискФильмов
    @ПоисковыйЗапрос NVARCHAR(100)
AS
BEGIN
    SELECT * FROM Фильмы
    WHERE Название LIKE '%' + @ПоисковыйЗапрос + '%'
       OR Режиссёр LIKE '%' + @ПоисковыйЗапрос + '%'
       OR Жанр LIKE '%' + @ПоисковыйЗапрос + '%'
       OR Роли LIKE '%' + @ПоисковыйЗапрос + '%'
    ORDER BY Название;
END
GO

CREATE PROCEDURE СортироватьСеансы
    @ПолеСортировки NVARCHAR(50) = 'Время',
    @НаправлениеСортировки NVARCHAR(4) = 'ASC'
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    IF @ПолеСортировки NOT IN ('ID_сеанса', 'Время', 'ID_фильма', 'ID_зала')
    BEGIN
        SET @ПолеСортировки = 'Время';
    END
    
    SET @SQL = N'SELECT s.*, f.Название AS Название_фильма, z.Номер AS Номер_зала 
                FROM Сеанс s
                JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
                JOIN Зал z ON s.ID_зала = z.ID_зала
                ORDER BY ' + QUOTENAME(@ПолеСортировки) + ' ' + @НаправлениеСортировки;
    
    EXEC sp_executesql @SQL;
END
GO

CREATE PROCEDURE ФильтроватьСеансы
    @ДатаНачала DATETIME = NULL,
    @ДатаОкончания DATETIME = NULL,
    @ID_фильма INT = NULL,
    @ID_зала INT = NULL
AS
BEGIN
    SELECT 
        s.ID_сеанса,
        s.Время,
        f.Название AS Название_фильма,
        z.Номер AS Номер_зала,
        k.Название AS Кинотеатр,
        dbo.GetAvailableSeatsCount(s.ID_сеанса) AS Свободные_места
    FROM Сеанс s
    JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
    JOIN Зал z ON s.ID_зала = z.ID_зала
    JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
    WHERE (@ДатаНачала IS NULL OR s.Время >= @ДатаНачала)
      AND (@ДатаОкончания IS NULL OR s.Время <= @ДатаОкончания)
      AND (@ID_фильма IS NULL OR s.ID_фильма = @ID_фильма)
      AND (@ID_зала IS NULL OR s.ID_зала = @ID_зала)
    ORDER BY s.Время;
END
GO

CREATE OR ALTER PROCEDURE ПоискСеансов
    @ПоисковыйЗапрос NVARCHAR(100)
AS
BEGIN
    SELECT 
        s.ID_сеанса,
        s.Время,
        f.Название AS Название_фильма,
        z.Номер AS Номер_зала,
        k.Название AS Кинотеатр,
        CASE 
            WHEN dbo.GetAvailableSeatsCount(s.ID_сеанса) < 0 THEN 'Нет данных'
            ELSE CAST(dbo.GetAvailableSeatsCount(s.ID_сеанса) AS NVARCHAR)
        END AS Свободные_места
    FROM Сеанс s
    JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
    JOIN Зал z ON s.ID_зала = z.ID_зала
    JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
    WHERE 
        f.Название LIKE '%' + @ПоисковыйЗапрос + '%' OR
        k.Название LIKE '%' + @ПоисковыйЗапрос + '%' OR
        CAST(z.Номер AS NVARCHAR) LIKE '%' + @ПоисковыйЗапрос + '%' OR
        CAST(s.ID_сеанса AS NVARCHAR) LIKE '%' + @ПоисковыйЗапрос + '%'
    ORDER BY s.Время;
END
GO
------------------------------------------

CREATE PROCEDURE dbo.ПолучитьФильмы
AS
BEGIN
    SELECT
        ID_фильма,
        Название,
        Год_выпуска,
        Режиссёр,
        Жанр,
        Рейтинг,
        Роли,
        Страна,
        Описание,
        Ограничение
    FROM
        Фильмы;
END
GO

CREATE PROCEDURE dbo.ПолучитьСеансы
AS
BEGIN
    SELECT
        s.ID_сеанса,
        s.Время,
        f.Название AS Название_фильма,
        z.Номер AS Номер_зала,
        k.Название AS Кинотеатр,
        dbo.GetAvailableSeatsCount(s.ID_сеанса) AS Свободные_места
    FROM
        Сеанс s
    JOIN
        Фильмы f ON s.ID_фильма = f.ID_фильма
    JOIN
        Зал z ON s.ID_зала = z.ID_зала
    JOIN
        Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
    ORDER BY
        s.Время;
END
GO

CREATE PROCEDURE ДобавитьКинотеатр
    @Название NVARCHAR(100),
    @Адрес NVARCHAR(200),
    @Описание NVARCHAR(MAX),
    @Режим_работы NVARCHAR(100),
    @Контакты NVARCHAR(100),
    @ID_оператора INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Кинотеатр WHERE Название = @Название AND Адрес = @Адрес)
    BEGIN
        IF EXISTS (SELECT 1 FROM Оператор WHERE ID_оператора = @ID_оператора)
        BEGIN
            INSERT INTO Кинотеатр (Название, Адрес, Описание, Режим_работы, Контакты, ID_оператора)
            VALUES (@Название, @Адрес, @Описание, @Режим_работы, @Контакты, @ID_оператора);
        END
        ELSE
            PRINT N'Оператор не найден!';
    END
    ELSE
        PRINT N'Кинотеатр с таким названием и адресом уже существует!';
END
GO

CREATE PROCEDURE ОбновитьКинотеатр
    @ID_кинотеатра INT,
    @Название NVARCHAR(100),
    @Адрес NVARCHAR(200),
    @Описание NVARCHAR(MAX),
    @Режим_работы NVARCHAR(100),
    @Контакты NVARCHAR(100),
    @ID_оператора INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Кинотеатр WHERE ID_кинотеатра = @ID_кинотеатра)
    BEGIN
        IF EXISTS (SELECT 1 FROM Оператор WHERE ID_оператора = @ID_оператора)
        BEGIN
            UPDATE Кинотеатр
            SET Название = @Название,
                Адрес = @Адрес,
                Описание = @Описание,
                Режим_работы = @Режим_работы,
                Контакты = @Контакты,
                ID_оператора = @ID_оператора
            WHERE ID_кинотеатра = @ID_кинотеатра;
        END
        ELSE
            PRINT N'Оператор не найден!';
    END
    ELSE
        PRINT N'Кинотеатр не найден!';
END
GO

CREATE OR ALTER PROCEDURE УдалитьКинотеатр
    @ID_кинотеатра INT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        
        IF NOT EXISTS (SELECT 1 FROM Кинотеатр WHERE ID_кинотеатра = @ID_кинотеатра)
        BEGIN
            RAISERROR(N'Кинотеатр не найден!', 16, 1);
            RETURN;
        END
        
        DELETE FROM Возвраты
        WHERE ID_билета IN (
            SELECT b.ID_билета
            FROM Билет b
            JOIN Сеанс s ON b.ID_сеанса = s.ID_сеанса
            JOIN Зал z ON s.ID_зала = z.ID_зала
            WHERE z.ID_кинотеатра = @ID_кинотеатра
        );
        
        DELETE FROM Билет
        WHERE ID_сеанса IN (
            SELECT s.ID_сеанса
            FROM Сеанс s
            JOIN Зал z ON s.ID_зала = z.ID_зала
            WHERE z.ID_кинотеатра = @ID_кинотеатра
        );
        
        DELETE FROM Отзывы
        WHERE ID_фильма IN (
            SELECT DISTINCT s.ID_фильма
            FROM Сеанс s
            JOIN Зал z ON s.ID_зала = z.ID_зала
            WHERE z.ID_кинотеатра = @ID_кинотеатра
        );
        
        DELETE FROM Сеанс
        WHERE ID_зала IN (
            SELECT ID_зала FROM Зал WHERE ID_кинотеатра = @ID_кинотеатра
        );
        
        DELETE FROM Обращения
        WHERE ID_клиента IN (
            SELECT DISTINCT b.ID_клиента
            FROM Билет b
            JOIN Сеанс s ON b.ID_сеанса = s.ID_сеанса
            JOIN Зал z ON s.ID_зала = z.ID_зала
            WHERE z.ID_кинотеатра = @ID_кинотеатра
        );
        
        DELETE FROM Место
        WHERE ID_зала IN (
            SELECT ID_зала FROM Зал WHERE ID_кинотеатра = @ID_кинотеатра
        );
        
        DELETE FROM Зал WHERE ID_кинотеатра = @ID_кинотеатра;
        
        DELETE FROM Кинотеатр WHERE ID_кинотеатра = @ID_кинотеатра;
        
        COMMIT TRANSACTION;
        PRINT N'Кинотеатр и все связанные данные успешно удалены!';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT N'Ошибка при удалении кинотеатра: ' + ERROR_MESSAGE();
    END CATCH
END
GO

CREATE PROCEDURE ДобавитьКлиента
    @Фамилия NVARCHAR(50),
    @Имя NVARCHAR(50),
    @Отчество NVARCHAR(50),
    @Email NVARCHAR(100),
    @Телефон NVARCHAR(20),
    @Пароль NVARCHAR(100)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Клиент WHERE Телефон = @Телефон)
    BEGIN
        INSERT INTO Клиент (Фамилия, Имя, Отчество, Email, Телефон, Пароль)
        VALUES (@Фамилия, @Имя, @Отчество, @Email, @Телефон, @Пароль);
    END
    ELSE 
        PRINT N'Клиент с таким телефоном уже существует!';
END
GO


CREATE PROCEDURE ОбновитьКлиента
    @ID_клиента INT,
    @Фамилия NVARCHAR(50),
    @Имя NVARCHAR(50),
    @Отчество NVARCHAR(50),
    @Email NVARCHAR(100),
    @Телефон NVARCHAR(20)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Клиент WHERE ID_клиента = @ID_клиента)
    BEGIN
        UPDATE Клиент
        SET Фамилия = @Фамилия,
            Имя = @Имя,
            Отчество = @Отчество,
            Email = @Email,
            Телефон = @Телефон
        WHERE ID_клиента = @ID_клиента;
    END
    ELSE
        PRINT N'Клиент не найден!';
END
GO


CREATE PROCEDURE УдалитьКлиента
    @ID_клиента INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Клиент WHERE ID_клиента = @ID_клиента)
    BEGIN
        DELETE FROM Отзывы WHERE ID_клиента = @ID_клиента;
        DELETE FROM Обращения WHERE ID_клиента = @ID_клиента;
        DELETE FROM Билет WHERE ID_клиента = @ID_клиента;
        DELETE FROM Клиент WHERE ID_клиента = @ID_клиента;
    END
    ELSE
        PRINT N'Клиент не найден!';
END
GO

CREATE PROCEDURE ДобавитьФильм
    @Название NVARCHAR(100),
    @Год_выпуска INT,
    @Режиссёр NVARCHAR(100),
    @Жанр NVARCHAR(50),
    @Рейтинг NVARCHAR(10),
    @Роли NVARCHAR(MAX),
    @Страна NVARCHAR(50),
    @Описание NVARCHAR(MAX),
    @Ограничение NVARCHAR(50)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Фильмы WHERE Название = @Название AND Год_выпуска = @Год_выпуска)
    BEGIN
        INSERT INTO Фильмы (Название, Год_выпуска, Режиссёр, Жанр, Рейтинг, Роли, Страна, Описание, Ограничение)
        VALUES (@Название, @Год_выпуска, @Режиссёр, @Жанр, @Рейтинг, @Роли, @Страна, @Описание, @Ограничение);
    END
    ELSE
        PRINT N'Фильм уже существует!';
END
GO


CREATE PROCEDURE ОбновитьФильм
    @ID_фильма INT,
    @Название NVARCHAR(100),
    @Год_выпуска INT,
    @Режиссёр NVARCHAR(100),
    @Жанр NVARCHAR(50),
    @Рейтинг NVARCHAR(10),
    @Роли NVARCHAR(MAX),
    @Страна NVARCHAR(50),
    @Описание NVARCHAR(MAX),
    @Ограничение NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Фильмы WHERE ID_фильма = @ID_фильма)
    BEGIN
        UPDATE Фильмы
        SET Название = @Название,
            Год_выпуска = @Год_выпуска,
            Режиссёр = @Режиссёр,
            Жанр = @Жанр,
            Рейтинг = @Рейтинг,
            Роли = @Роли,
            Страна = @Страна,
            Описание = @Описание,
            Ограничение = @Ограничение
        WHERE ID_фильма = @ID_фильма;
    END
    ELSE
        PRINT N'Фильм не найден!';
END
GO


CREATE PROCEDURE УдалитьФильм
    @ID_фильма INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Фильмы WHERE ID_фильма = @ID_фильма)
    BEGIN
        DELETE FROM Отзывы WHERE ID_фильма = @ID_фильма;
        DELETE FROM Сеанс WHERE ID_фильма = @ID_фильма;
        DELETE FROM Фильмы WHERE ID_фильма = @ID_фильма;
    END
    ELSE
        PRINT N'Фильм не найден!';
END
GO

CREATE PROCEDURE ДобавитьМесто
    @ID_зала INT,
    @Номер_ряда INT,
    @Номер_места INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Зал WHERE ID_зала = @ID_зала)
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM Место WHERE ID_зала = @ID_зала AND Номер_ряда = @Номер_ряда AND Номер_места = @Номер_места)
        BEGIN
            INSERT INTO Место (ID_зала, Номер_ряда, Номер_места)
            VALUES (@ID_зала, @Номер_ряда, @Номер_места);
        END
        ELSE
            PRINT N'Место уже существует!';
    END
    ELSE
        PRINT N'Зал не найден!';
END
GO



CREATE PROCEDURE ДобавитьМестаВРяду
    @Номер_зала INT,
    @Номер_ряда INT,
    @Количество_мест INT
AS
BEGIN
    DECLARE @i INT = 1;
    WHILE @i <= @Количество_мест
    BEGIN
        EXEC ДобавитьМесто @Номер_зала, @Номер_ряда, @i;
        SET @i = @i + 1;
    END
END
GO


CREATE PROCEDURE УдалитьМесто
    @ID_места INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Место WHERE ID_места = @ID_места)
    BEGIN
        DELETE FROM Билет WHERE ID_места = @ID_места;
        DELETE FROM Место WHERE ID_места = @ID_места;
    END
    ELSE
        PRINT N'Место не найдено!';
END
GO


CREATE OR ALTER PROCEDURE ДобавитьЗал
    @Номер INT,
    @Тип_зала NVARCHAR(50),
    @ID_кинотеатра INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Зал WHERE Номер = @Номер AND ID_кинотеатра = @ID_кинотеатра)
    BEGIN
        INSERT INTO Зал (Номер, Тип_зала, ID_кинотеатра)
        VALUES (@Номер, @Тип_зала, @ID_кинотеатра);
    END
    ELSE 
        PRINT N'Зал с таким номером уже существует в этом кинотеатре!';
END
GO

CREATE OR ALTER PROCEDURE ОбновитьЗал
    @ID_зала INT,
    @Номер INT,
    @Тип_зала NVARCHAR(50),
    @ID_кинотеатра INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Зал WHERE ID_зала = @ID_зала)
    BEGIN
        UPDATE Зал
        SET Номер = @Номер,
            Тип_зала = @Тип_зала,
            ID_кинотеатра = @ID_кинотеатра
        WHERE ID_зала = @ID_зала;
    END
    ELSE
        PRINT N'Зал не найден!';
END
GO

CREATE PROCEDURE УдалитьЗал
    @Номер INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Зал WHERE Номер = @Номер)
    BEGIN
        DELETE FROM Билет WHERE ID_сеанса IN (SELECT ID_сеанса FROM Сеанс WHERE ID_зала = (SELECT ID_зала FROM Зал WHERE Номер = @Номер));
        DELETE FROM Сеанс WHERE ID_зала = (SELECT ID_зала FROM Зал WHERE Номер = @Номер);
        DELETE FROM Место WHERE ID_зала = (SELECT ID_зала FROM Зал WHERE Номер = @Номер);
        DELETE FROM Зал WHERE Номер = @Номер;
    END
    ELSE
        PRINT N'Зал не найден!';
END
GO

CREATE PROCEDURE ДобавитьСеанс
    @Время DATETIME,
    @ID_фильма INT,
    @ID_зала INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Сеанс WHERE Время = @Время AND ID_зала = @ID_зала)
    BEGIN
        IF EXISTS (SELECT 1 FROM Фильмы WHERE ID_фильма = @ID_фильма)
        BEGIN
            IF EXISTS (SELECT 1 FROM Зал WHERE ID_зала = @ID_зала)
            BEGIN
                INSERT INTO Сеанс (Время, ID_фильма, ID_зала)
                VALUES (@Время, @ID_фильма, @ID_зала);
            END
            ELSE
                PRINT N'Зал не найден!';
        END
        ELSE
            PRINT N'Фильм не найден!';
    END
    ELSE
        PRINT N'Сеанс уже существует!';
END
GO

CREATE PROCEDURE ОбновитьСеанс
    @ID_сеанса INT,
    @Время DATETIME,
    @ID_фильма INT,
    @ID_зала INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Сеанс WHERE ID_сеанса = @ID_сеанса)
    BEGIN
        IF EXISTS (SELECT 1 FROM Фильмы WHERE ID_фильма = @ID_фильма)
        BEGIN
            IF EXISTS (SELECT 1 FROM Зал WHERE ID_зала = @ID_зала)
            BEGIN
                UPDATE Сеанс
                SET Время = @Время,
                    ID_фильма = @ID_фильма,
                    ID_зала = @ID_зала
                WHERE ID_сеанса = @ID_сеанса;
            END
            ELSE
                PRINT N'Зал не найден!';
        END
        ELSE
            PRINT N'Фильм не найден!';
    END
    ELSE
        PRINT N'Сеанс не найден!';
END
GO

CREATE PROCEDURE УдалитьСеанс
    @ID_сеанса INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Сеанс WHERE ID_сеанса = @ID_сеанса)
    BEGIN
        DELETE FROM Билет WHERE ID_сеанса = @ID_сеанса;
        DELETE FROM Сеанс WHERE ID_сеанса = @ID_сеанса;
    END
    ELSE
        PRINT N'Сеанс не найден!';
END
GO

CREATE OR ALTER PROCEDURE ДобавитьБилет
    @Цена MONEY,
    @Льгота NVARCHAR(50),
    @Дата_время DATETIME,
    @ID_сеанса INT,
    @ID_места INT,
    @ID_клиента INT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM Билет 
        WHERE ID_сеанса = @ID_сеанса 
        AND ID_места = @ID_места
        AND Статус IN ('Забронирован', 'Активен')
    )
    BEGIN
        IF EXISTS (SELECT 1 FROM Сеанс WHERE ID_сеанса = @ID_сеанса)
        BEGIN
            IF EXISTS (SELECT 1 FROM Место WHERE ID_места = @ID_места)
            BEGIN
                IF EXISTS (SELECT 1 FROM Клиент WHERE ID_клиента = @ID_клиента)
                BEGIN
                    INSERT INTO Билет (Цена, Льгота, Дата_время, Статус, ID_сеанса, ID_места, ID_клиента)
                    VALUES (@Цена, @Льгота, @Дата_время, 'Забронирован', @ID_сеанса, @ID_места, @ID_клиента);
                END
                ELSE
                    PRINT N'Клиент не найден!';
            END
            ELSE
                PRINT N'Место не найдено!';
        END
        ELSE
            PRINT N'Сеанс не найден!';
    END
    ELSE
        PRINT N'Место уже занято на данном сеансе!';
END
GO

CREATE OR ALTER PROCEDURE УдалитьБилет
    @ID_билета INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Билет WHERE ID_билета = @ID_билета)
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM Возвраты WHERE ID_билета = @ID_билета)
        BEGIN
            INSERT INTO Возвраты (Статус, ID_билета)
            VALUES ('Не обработан', @ID_билета);

            UPDATE Билет
            SET Статус = 'Возвращен'
            WHERE ID_билета = @ID_билета;
            
            PRINT N'Билет помечен как возвращенный и добавлен в таблицу Возвраты';
        END
        ELSE
            PRINT N'Этот билет уже был возвращен ранее!';
    END
    ELSE
        PRINT N'Билет не найден!';
END
GO

CREATE PROCEDURE ОбработатьВозврат
    @ID_возврата INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Возвраты WHERE ID_возврата = @ID_возврата AND Статус = 'Не обработан')
    BEGIN
        UPDATE Возвраты
        SET Статус = 'Обработан'
        WHERE ID_возврата = @ID_возврата;
        
        PRINT N'Возврат успешно обработан';
    END
    ELSE
        PRINT N'Возврат не найден или уже обработан!';
END
GO

CREATE OR ALTER PROCEDURE ОплатитьБилет
    @ID_билета INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Билет WHERE ID_билета = @ID_билета AND Статус = 'Забронирован')
    BEGIN
        UPDATE Билет
        SET Статус = 'Активен'
        WHERE ID_билета = @ID_билета;
        
        PRINT N'Билет успешно оплачен';
    END
    ELSE IF EXISTS (SELECT 1 FROM Билет WHERE ID_билета = @ID_билета AND Статус = 'Активен')
    BEGIN
        PRINT N'Билет уже оплачен ранее';
    END
    ELSE
        PRINT N'Билет не найден или не может быть оплачен';
END
GO

CREATE PROCEDURE ДобавитьОтзыв
    @ID_клиента INT,
    @ID_фильма INT,
    @Оценка INT,
    @Текст NVARCHAR(500) = NULL
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Клиент WHERE ID_клиента = @ID_клиента)
    BEGIN
        IF EXISTS (SELECT 1 FROM Фильмы WHERE ID_фильма = @ID_фильма)
        BEGIN
            INSERT INTO Отзывы (ID_клиента, ID_фильма, Оценка, Текст, Дата)
            VALUES (@ID_клиента, @ID_фильма, @Оценка, @Текст, GETDATE());
        END
        ELSE
            PRINT N'Фильм не найден!';
    END
    ELSE
        PRINT N'Клиент не найден!';
END
GO

CREATE PROCEDURE ОбновитьОтзыв
    @ID_отзыва INT,
    @Оценка INT,
    @Текст NVARCHAR(500) = NULL
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Отзывы WHERE ID_отзыва = @ID_отзыва)
    BEGIN
        UPDATE Отзывы
        SET Оценка = @Оценка,
            Текст = @Текст
        WHERE ID_отзыва = @ID_отзыва;
    END
    ELSE
        PRINT N'Отзыв не найден!';
END
GO

CREATE PROCEDURE УдалитьОтзыв
    @ID_отзыва INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Отзывы WHERE ID_отзыва = @ID_отзыва)
    BEGIN
        DELETE FROM Отзывы WHERE ID_отзыва = @ID_отзыва;
    END
    ELSE
        PRINT N'Отзыв не найден!';
END
GO

CREATE PROCEDURE ДобавитьОбращение
    @ID_клиента INT,
    @Тема NVARCHAR(100),
    @Текст NVARCHAR(500)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Клиент WHERE ID_клиента = @ID_клиента)
    BEGIN
        INSERT INTO Обращения (ID_клиента, Тема, Текст, Дата, Статус)
        VALUES (@ID_клиента, @Тема, @Текст, GETDATE(), 'Ожидает');
    END
    ELSE
        PRINT N'Клиент не найден!';
END
GO

CREATE OR ALTER PROCEDURE ОбновитьСтатусОбращения
    @ID_обращения INT,
    @Статус NVARCHAR(20),
    @Ответ NVARCHAR(500) = NULL,
    @ID_сотрудника INT = NULL
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        
        IF NOT EXISTS (SELECT 1 FROM Обращения WHERE ID_обращения = @ID_обращения)
            RAISERROR('Обращение не найдено!', 16, 1);
        
        IF @ID_сотрудника IS NOT NULL 
           AND NOT EXISTS (SELECT 1 FROM Служба_поддержки WHERE ID_сотрудника = @ID_сотрудника)
            RAISERROR('Сотрудник не найден!', 16, 1);
        
        UPDATE Обращения
        SET 
            Статус = @Статус,
            Ответ = @Ответ,
            ID_сотрудника = ISNULL(@ID_сотрудника, ID_сотрудника)
        WHERE ID_обращения = @ID_обращения;
        
        IF @ID_сотрудника IS NOT NULL AND @Статус = 'Решено'
            UPDATE Служба_поддержки
            SET Количество_обращений = Количество_обращений + 1
            WHERE ID_сотрудника = @ID_сотрудника;
        
        COMMIT TRANSACTION;
        PRINT 'Статус обращения успешно обновлен';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Ошибка: ' + ERROR_MESSAGE();
    END CATCH
END
GO

CREATE PROCEDURE ПолучитьОбращенияПоСтатусу
    @Статус NVARCHAR(20)
AS
BEGIN
    SELECT o.ID_обращения, c.Фамилия + ' ' + c.Имя AS ФИО_клиента, o.Тема, o.Текст, 
           o.Дата, o.Статус, o.Ответ
    FROM Обращения o
    JOIN Клиент c ON o.ID_клиента = c.ID_клиента
    WHERE o.Статус = @Статус
    ORDER BY o.Дата DESC;
END
GO

CREATE OR ALTER PROCEDURE ExportFilmsToJSON
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @jsonResult NVARCHAR(MAX);
    DECLARE @outputPath VARCHAR(255) = 'D:\gg\export_films.json';
    DECLARE @errorMessage VARCHAR(1000);
    DECLARE @fileCheck INT;
    DECLARE @cmd VARCHAR(8000);

    SELECT @jsonResult = (
        SELECT TOP 100
            ID_фильма AS Id,
            Название AS Title,
            Год_выпуска AS ReleaseYear,
            Режиссёр AS Director,
            Жанр AS Genre,
            Рейтинг AS Rating,
            Роли AS Cast,
            Страна AS Country,
            Описание AS Description,
            Ограничение AS Restriction
        FROM Фильмы
        FOR JSON PATH, INCLUDE_NULL_VALUES
    );

    IF @jsonResult IS NULL OR LEN(@jsonResult) = 0
    BEGIN
        RAISERROR('Нет данных для экспорта', 16, 1);
        RETURN;
    END

    IF OBJECT_ID('tempdb..##FilmExport') IS NOT NULL
        DROP TABLE ##FilmExport;
    
    CREATE TABLE ##FilmExport (JsonData NVARCHAR(MAX));
    INSERT INTO ##FilmExport VALUES (@jsonResult);

    SET @cmd = 'bcp "SELECT JsonData FROM ##FilmExport" queryout "' 
             + @outputPath + '" -c -C 65001 -T -S ' + @@SERVERNAME;

    BEGIN TRY
        EXEC master.dbo.xp_cmdshell @cmd, NO_OUTPUT;
        
        EXEC master.dbo.xp_fileexist @outputPath, @fileCheck OUTPUT;
        
        IF @fileCheck = 1
            PRINT 'Файл успешно создан: ' + @outputPath;
        ELSE
            RAISERROR('Файл не был создан', 16, 1);
    END TRY
    BEGIN CATCH
        SET @errorMessage = 'Ошибка при экспорте: ' + ERROR_MESSAGE();
        RAISERROR(@errorMessage, 16, 1);
    END CATCH
    
    IF OBJECT_ID('tempdb..##FilmExport') IS NOT NULL
        DROP TABLE ##FilmExport;
END;
GO

CREATE OR ALTER PROCEDURE ImportFilmsFromJSON
    @FilePath NVARCHAR(255) = N'D:\gg\export_films.json'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE 
        @jsonData NVARCHAR(MAX),
        @sql      NVARCHAR(MAX);

    SET @sql = N'
        IF OBJECT_ID(N''tempdb..##FilmJsonStaging'') IS NOT NULL
            DROP TABLE ##FilmJsonStaging;

        CREATE TABLE ##FilmJsonStaging (JsonLine VARCHAR(MAX));

        BULK INSERT ##FilmJsonStaging
        FROM N''' + REPLACE(@FilePath,'''','''''') + '''
        WITH (
            DATAFILETYPE  = ''char'',
            CODEPAGE      = ''65001'',
            ROWTERMINATOR = ''0x0a''
        );

        -- Собираем весь текст файла в одну строку
        SELECT @jsonDataOUT = STRING_AGG(JsonLine, CHAR(10))
        FROM ##FilmJsonStaging;

        DROP TABLE ##FilmJsonStaging;
    ';

    EXEC sp_executesql
        @sql,
        N'@jsonDataOUT NVARCHAR(MAX) OUTPUT',
        @jsonDataOUT = @jsonData OUTPUT;

    IF @jsonData IS NULL OR LEN(@jsonData) = 0
    BEGIN
        RAISERROR('Файл пуст или не содержит данных', 16, 1);
        RETURN;
    END

    IF ISJSON(@jsonData) <> 1
    BEGIN
        RAISERROR('Файл не содержит валидный JSON', 16, 1);
        PRINT LEFT(@jsonData, 500);
        RETURN;
    END
    BEGIN TRY
        INSERT INTO Фильмы (
            Название,
            Год_выпуска,
            Режиссёр,
            Жанр,
            Рейтинг,
            Роли,
            Страна,
            Описание,
            Ограничение
        )
        SELECT
            json.Title,
            json.ReleaseYear,
            json.Director,
            json.Genre,
            json.Rating,
            json.Cast,
            json.Country,
            json.Description,
            json.Restriction
        FROM OPENJSON(@jsonData)
        WITH (
            Title       NVARCHAR(255)  '$.Title',
            ReleaseYear INT            '$.ReleaseYear',
            Director    NVARCHAR(255)  '$.Director',
            Genre       NVARCHAR(100)  '$.Genre',
            Rating      NVARCHAR(10)   '$.Rating',
            Cast        NVARCHAR(MAX)  '$.Cast',
            Country     NVARCHAR(100)  '$.Country',
            Description NVARCHAR(MAX)  '$.Description',
            Restriction NVARCHAR(20)   '$.Restriction'
        ) AS json;

        PRINT 'Успешно импортировано ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' записей';
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH;
END;
GO


/* Все триггеры */

CREATE OR ALTER TRIGGER ПроверкаСвободногоМеста ON Билет
INSTEAD OF INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM inserted i
        JOIN Билет b ON i.ID_сеанса = b.ID_сеанса AND i.ID_места = b.ID_места
        WHERE b.Статус IN ('Забронирован', 'Активен')
    )
    BEGIN
        RAISERROR(N'Одно или несколько мест уже заняты на данных сеансах!', 16, 1)
        RETURN
    END
    
    INSERT INTO Билет (Цена, Льгота, Дата_время, Статус, ID_сеанса, ID_места, ID_клиента)
    SELECT Цена, Льгота, Дата_время, 'Забронирован', ID_сеанса, ID_места, ID_клиента
    FROM inserted
END
GO

CREATE OR ALTER TRIGGER ПроверкаКоличестваМест ON Место
AFTER INSERT
AS
BEGIN
    DECLARE @ID_зала INT, @МаксМест INT, @ТекущееКоличествоМест INT
    
    SELECT @ID_зала = i.ID_зала
    FROM inserted i;
    
    SET @МаксМест = 100; 
    
    SELECT @ТекущееКоличествоМест = COUNT(*) 
    FROM Место 
    WHERE ID_зала = @ID_зала;
    
    IF @ТекущееКоличествоМест > @МаксМест
    BEGIN
        RAISERROR(N'Зал полон! Невозможно добавить больше мест.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

CREATE OR ALTER TRIGGER ОбновитьСтатусБилетовПослеСеанса
ON Сеанс
AFTER UPDATE
AS
BEGIN
    IF UPDATE(Время)
    BEGIN
        UPDATE Билет
        SET Статус = 'Истекший'
        FROM Билет б
        JOIN inserted i ON б.ID_сеанса = i.ID_сеанса
        WHERE i.Время < GETDATE()
        AND б.Статус IN ('Забронирован', 'Активен');
    END
END
GO

CREATE OR ALTER TRIGGER ПроверкаДатыВремениСеанса
ON Сеанс
AFTER INSERT, UPDATE
AS
BEGIN
    DECLARE @Время DATETIME;

    SELECT @Время = Время FROM inserted;

    IF @Время < GETDATE()
    BEGIN
        RAISERROR('Дата и время сеанса не могут быть в прошлом.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

CREATE OR ALTER TRIGGER ПроверкаФильмаИЗалПриДобавленииСеанса
ON Сеанс
AFTER INSERT
AS
BEGIN
    DECLARE @ID_фильма INT, @ID_зала INT;

    SELECT @ID_фильма = i.ID_фильма, @ID_зала = i.ID_зала
    FROM inserted i;

    IF NOT EXISTS (SELECT 1 FROM Фильмы WHERE ID_фильма = @ID_фильма)
    BEGIN
        RAISERROR('Фильм не найден!', 16, 1);
        ROLLBACK TRANSACTION;
    END

    IF NOT EXISTS (SELECT 1 FROM Зал WHERE ID_зала = @ID_зала)
    BEGIN
        RAISERROR('Зал не найден!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

CREATE TRIGGER ПроверкаКлиентаПриДобавленииБилета
ON Билет
AFTER INSERT
AS
BEGIN
    DECLARE @ID_клиента INT;

    SELECT @ID_клиента = i.ID_клиента
    FROM inserted i;

    IF NOT EXISTS (SELECT 1 FROM Клиент WHERE ID_клиента = @ID_клиента)
    BEGIN
        RAISERROR('Клиент не найден!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

CREATE TRIGGER ПроверкаСеансаПриДобавленииБилета
ON Билет
AFTER INSERT
AS
BEGIN
    DECLARE @ID_сеанса INT;

    SELECT @ID_сеанса = i.ID_сеанса
    FROM inserted i;

    IF NOT EXISTS (SELECT 1 FROM Сеанс WHERE ID_сеанса = @ID_сеанса)
    BEGIN
        RAISERROR('Сеанс не найден!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

CREATE TRIGGER ПроверкаОператораПриДобавленииКинотеатра
ON Кинотеатр
AFTER INSERT
AS
BEGIN
    DECLARE @ID_оператора INT;

    SELECT @ID_оператора = i.ID_оператора
    FROM inserted i;

    IF NOT EXISTS (SELECT 1 FROM Оператор WHERE ID_оператора = @ID_оператора)
    BEGIN
        RAISERROR('Оператор не найден!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

CREATE OR ALTER TRIGGER ПроверкаУникальностиОтзываКлиента
ON Отзывы
INSTEAD OF INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN Отзывы o ON i.ID_клиента = o.ID_клиента AND i.ID_фильма = o.ID_фильма
    )
    BEGIN
        RAISERROR('Клиент уже оставил отзыв на один или несколько указанных фильмов.', 16, 1);
        RETURN;
    END
    INSERT INTO Отзывы (ID_клиента, ID_фильма, Оценка, Текст, Дата)
    SELECT ID_клиента, ID_фильма, Оценка, Текст, ISNULL(Дата, GETDATE())
    FROM inserted;
END
GO

CREATE TRIGGER ПроверкаПересеченияСеансов
ON Сеанс
AFTER INSERT, UPDATE
AS
BEGIN
    DECLARE @ID_зала INT, @ВремяНачала DATETIME, @ВремяОкончания DATETIME;

    SELECT @ID_зала = i.ID_зала, @ВремяНачала = i.Время, @ВремяОкончания = DATEADD(MINUTE, 120, i.Время)
    FROM inserted i;

    IF EXISTS (
        SELECT 1
        FROM Сеанс s
        WHERE s.ID_зала = @ID_зала
          AND s.ID_сеанса <> (SELECT ID_сеанса FROM inserted)
          AND (s.Время BETWEEN @ВремяНачала AND @ВремяОкончания
               OR @ВремяНачала BETWEEN s.Время AND DATEADD(MINUTE, 120, s.Время))
    )
    BEGIN
        RAISERROR('Сеанс пересекается с другим сеансом в этом зале.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

 /* Все роли */

CREATE ROLE Клиент;
GO
GRANT EXECUTE ON dbo.ПолучитьФильмы TO Клиент;
GRANT EXECUTE ON dbo.ПолучитьСеансы TO Клиент;
GRANT EXECUTE ON dbo.ДобавитьБилет TO Клиент;
GRANT EXECUTE ON dbo.ОплатитьБилет TO Клиент;
GRANT EXECUTE ON dbo.УдалитьБилет TO Клиент;
GRANT EXECUTE ON dbo.ДобавитьОтзыв TO Клиент;
GRANT EXECUTE ON dbo.ОбновитьОтзыв TO Клиент;
GRANT EXECUTE ON dbo.УдалитьОтзыв TO Клиент;
GRANT EXECUTE ON dbo.ДобавитьОбращение TO Клиент;
GRANT EXECUTE ON dbo.ПолучитьОбращенияПоСтатусу TO Клиент;
GO

CREATE ROLE Оператор;
GO
GRANT EXECUTE ON dbo.ДобавитьФильм TO Оператор;
GRANT EXECUTE ON dbo.ОбновитьФильм TO Оператор;
GRANT EXECUTE ON dbo.УдалитьФильм TO Оператор;
GRANT EXECUTE ON dbo.ДобавитьСеанс TO Оператор;
GRANT EXECUTE ON dbo.ОбновитьСеанс TO Оператор;
GRANT EXECUTE ON dbo.УдалитьСеанс TO Оператор;
GRANT EXECUTE ON dbo.ДобавитьЗал TO Оператор;
GRANT EXECUTE ON dbo.ОбновитьЗал TO Оператор;
GRANT EXECUTE ON dbo.УдалитьЗал TO Оператор;
GRANT EXECUTE ON dbo.ДобавитьМесто TO Оператор;
GRANT EXECUTE ON dbo.ДобавитьМестаВРяду TO Оператор;
GRANT EXECUTE ON dbo.УдалитьМесто TO Оператор;
GO

CREATE ROLE СотрудникСлужбыПоддержки;
GO
GRANT EXECUTE ON dbo.ОбновитьСтатусОбращения TO СотрудникСлужбыПоддержки;
GRANT EXECUTE ON dbo.УдалитьБилет TO СотрудникСлужбыПоддержки;
GO

CREATE ROLE Администратор;
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON Клиент TO Администратор;
GRANT SELECT, INSERT, UPDATE, DELETE ON Оператор TO Администратор;
GRANT SELECT, INSERT, UPDATE, DELETE ON Служба_поддержки TO Администратор;
GRANT SELECT, INSERT, UPDATE, DELETE ON Билет TO Администратор;
GRANT SELECT, INSERT, UPDATE, DELETE ON Фильмы TO Администратор;
GRANT SELECT, INSERT, UPDATE, DELETE ON Кинотеатр TO Администратор;
GRANT SELECT, INSERT, UPDATE, DELETE ON Зал TO Администратор;
GRANT SELECT, INSERT, UPDATE, DELETE ON Место TO Администратор;
GRANT SELECT, INSERT, UPDATE, DELETE ON Сеанс TO Администратор;
GRANT SELECT, INSERT, UPDATE, DELETE ON Отзывы TO Администратор;
GRANT SELECT, INSERT, UPDATE, DELETE ON Обращения TO Администратор;
GRANT EXECUTE ON SCHEMA::dbo TO Администратор;
GRANT SELECT ON SCHEMA::dbo TO Администратор;
GRANT ALTER ON SCHEMA::SCHEMA1 TO Администратор;
GRANT CREATE TABLE TO Администратор;
GRANT CREATE PROCEDURE TO Администратор;
GRANT CREATE VIEW TO Администратор;
GRANT CREATE FUNCTION TO Администратор;
GO

-- Общий функционал
GRANT EXECUTE ON dbo.СортироватьФильмы TO Клиент;
GRANT EXECUTE ON dbo.ФильтроватьФильмы TO Клиент;
GRANT EXECUTE ON dbo.ПоискФильмов TO Клиент;
GRANT EXECUTE ON dbo.СортироватьФильмы TO Оператор;
GRANT EXECUTE ON dbo.ФильтроватьФильмы TO Оператор;
GRANT EXECUTE ON dbo.ПоискФильмов TO Оператор;
GRANT EXECUTE ON dbo.СортироватьФильмы TO СотрудникСлужбыПоддержки;
GRANT EXECUTE ON dbo.ФильтроватьФильмы TO СотрудникСлужбыПоддержки;
GRANT EXECUTE ON dbo.ПоискФильмов TO СотрудникСлужбыПоддержки;
GRANT EXECUTE ON dbo.СортироватьФильмы TO Администратор;
GRANT EXECUTE ON dbo.ФильтроватьФильмы TO Администратор;
GRANT EXECUTE ON dbo.ПоискФильмов TO Администратор;

GRANT EXECUTE ON dbo.СортироватьСеансы TO Клиент;
GRANT EXECUTE ON dbo.ФильтроватьСеансы TO Клиент;
GRANT EXECUTE ON dbo.ПоискСеансов TO Клиент;
GRANT EXECUTE ON dbo.СортироватьСеансы TO Оператор;
GRANT EXECUTE ON dbo.ФильтроватьСеансы TO Оператор;
GRANT EXECUTE ON dbo.ПоискСеансов TO Оператор;
GRANT EXECUTE ON dbo.СортироватьСеансы TO СотрудникСлужбыПоддержки;
GRANT EXECUTE ON dbo.ФильтроватьСеансы TO СотрудникСлужбыПоддержки;
GRANT EXECUTE ON dbo.ПоискСеансов TO СотрудникСлужбыПоддержки;
GRANT EXECUTE ON dbo.СортироватьСеансы TO Администратор;
GRANT EXECUTE ON dbo.ФильтроватьСеансы TO Администратор;
GRANT EXECUTE ON dbo.ПоискСеансов TO Администратор;
GO

CREATE LOGIN client_login WITH PASSWORD = 'client';
CREATE LOGIN operator_login WITH PASSWORD = 'operator';
CREATE LOGIN support_login WITH PASSWORD = 'support';
CREATE LOGIN admin_login WITH PASSWORD = 'admin';

CREATE USER client_user FOR LOGIN client_login;
CREATE USER operator_user FOR LOGIN operator_login;
CREATE USER support_user FOR LOGIN support_login;
CREATE USER admin_user FOR LOGIN admin_login;

ALTER ROLE Клиент ADD MEMBER client_user;
ALTER ROLE Оператор ADD MEMBER operator_user;
ALTER ROLE СотрудникСлужбыПоддержки ADD MEMBER support_user;
ALTER ROLE Администратор ADD MEMBER admin_user;

-- Технология восстановления и резервного копирования БД
BACKUP DATABASE Cinema 
TO DISK = 'D:\gg\Cinema_Full.bak'
WITH INIT, 
NAME = 'Cinema-Full Database Backup',
DESCRIPTION = 'Полная резервная копия базы данных Cinema',
COMPRESSION, 
STATS = 10;
GO

BACKUP DATABASE Cinema 
TO DISK = 'D:\gg\Cinema_Diff.bak'
WITH DIFFERENTIAL, 
NAME = 'Cinema-Differential Database Backup',
DESCRIPTION = 'Дифференциальная резервная копия базы данных Cinema',
COMPRESSION, 
STATS = 10;
GO

BACKUP LOG Cinema 
TO DISK = 'D:\gg\Cinema_Log.trn'
WITH NAME = 'Cinema-Transaction Log Backup',
DESCRIPTION = 'Резервная копия журнала транзакций Cinema',
COMPRESSION, 
STATS = 10;
GO