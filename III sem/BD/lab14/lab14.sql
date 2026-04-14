---1
CREATE FUNCTION COUNT_TOVARY (@f VARCHAR(10)) RETURNS INT
AS
BEGIN
	DECLARE @k INT = 0;
	SET @k = (SELECT COUNT(Название_детали)
		FROM Товары JOIN Поставки
		ON Товар = Артикул 
		WHERE Код_поставщика = @f);
	RETURN @k;
END;

DECLARE @f INT = dbo.COUNT_TOVARY('SUP004');
PRINT 'Кол-во товаров = ' + CAST(@f AS VARCHAR(3));

SELECT Код_поставщика, dbo.COUNT_TOVARY(Код_поставщика) FROM Поставки;

---2
CREATE FUNCTION F (@tz CHAR(20)) RETURNS CHAR(300)
AS
BEGIN 
	DECLARE @tv CHAR(20);
	DECLARE @t VARCHAR(300) = 'Поставленные товары: ';
	DECLARE PostTovary CURSOR LOCAL 
	FOR SELECT Артикул FROM Поставки WHERE Код_поставщика = @tz;
	OPEN PostTovary;
	FETCH PostTovary INTO @tv;
	WHILE @@FETCH_STATUS = 0
		 BEGIN 
			SET @t = @t + ', ' + RTRIM(@tv);
			FETCH PostTovary INTO @tv;
		 END;
	RETURN @t;
END;

SELECT Код_поставщика, dbo.F (Код_поставщика) from Поставки;

---3
CREATE FUNCTION FTovCena(@f VARCHAR(50), @p INT)
    RETURNS TABLE
AS RETURN 
SELECT f.Товар, f.Цена, p.Артикул
    FROM Товары f LEFT OUTER JOIN Поставки p
        ON f.Товар = p.Артикул
		WHERE f.Товар = ISNULL(@f, f.Товар)
        AND 
		f.Цена = ISNULL(@p, f.Цена);



SELECT * FROM dbo.FTovCena(NULL, NULL);
SELECT * FROM dbo.FTovCena('PRD003', 12);
SELECT * FROM dbo.FTovCena(NULL, 3);
SELECT * FROM dbo.FTovCena('SWR003', 16);

DROP FUNCTION FTovCena;

---4 
CREATE FUNCTION FKolTov(@p VARCHAR(50)) 
RETURNS INT 
AS 
BEGIN 
    DECLARE @rc INT = (SELECT COUNT(*) 
                       FROM Поставки p
                       WHERE Код_поставщика = ISNULL(@p, p.Код_поставщика));
    RETURN @rc; 
END;
GO
SELECT Код_поставщика, dbo.FKolTov(Код_поставщика) [Количество заказов]
FROM Поставки;

SELECT dbo.FKolTov(NULL) [Всего заказов];

---6
CREATE FUNCTION COUNT_DEPARTMENTS(@faculty VARCHAR(50))
RETURNS INT
AS
BEGIN
    RETURN (SELECT COUNT(*) FROM PULPIT WHERE FACULTY = @faculty);
END;
GO

CREATE FUNCTION COUNT_GROUPS(@faculty VARCHAR(50))
RETURNS INT
AS
BEGIN
    RETURN (SELECT COUNT(*) FROM GROUPS WHERE FACULTY = @faculty);
END;
GO

CREATE FUNCTION COUNT_STUDENTS(@faculty VARCHAR(50))
RETURNS INT
AS
BEGIN
    RETURN (SELECT COUNT(*) FROM STUDENTS WHERE FACULTY = @faculty);
END;
GO

CREATE FUNCTION COUNT_PROFESSIONS(@faculty VARCHAR(50))
RETURNS INT
AS
BEGIN
    RETURN (SELECT COUNT(*) FROM PROFESSION WHERE FACULTY = @faculty);
END;
GO

CREATE FUNCTION FACULTY_REPORT(@c INT) 
RETURNS @fr TABLE
(
    [Факультет] VARCHAR(50), 
    [Количество кафедр] INT, 
    [Количество групп] INT, 
    [Количество студентов] INT, 
    [Количество специальностей] INT
)
AS
BEGIN 
    DECLARE cc CURSOR STATIC FOR 
    SELECT FACULTY 
    FROM FACULTY 
    WHERE dbo.COUNT_STUDENTS(FACULTY) > @c;

    DECLARE @f VARCHAR(50);
    OPEN cc;  
    FETCH cc INTO @f;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        INSERT INTO @fr 
        VALUES 
        (
            @f,  
            dbo.COUNT_DEPARTMENTS(@f), 
            dbo.COUNT_GROUPS(@f),   
            dbo.COUNT_STUDENTS(@f), 
            dbo.COUNT_PROFESSIONS(@f)
        ); 
        FETCH cc INTO @f;  
    END;   

    CLOSE cc;
    DEALLOCATE cc;
    RETURN; 
END;
GO
