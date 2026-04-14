----1
DECLARE @i int = 5,
		@v varchar(20) = 'Человек',
		@c char,
		@dt datetime,
		@t time,
		@si smallint,
		@ti tinyint,
		@nu numeric(12, 5);
SET @c = 'A';
SET @dt = GETDATE();
SET @t = CAST(GETDATE() AS time);
SET @si = 920;
SET @ti = 2;
SET @nu = 12345.67890;

SELECT @i = 7,
	   @v = 'Ноутбук';

PRINT 'intValue = ' + CAST(@i AS varchar);
PRINT 'varcharValue = ' + CAST(@v AS varchar(20));
PRINT 'timeValue = ' + CAST(@t AS varchar);

SELECT @i intValue,
	   @v varcharValue,
	   @t timeValue;

----2
DECLARE @y1 float = (SELECT CAST(AVG(Цена) AS float) FROM Товары),
		@y2 int
IF @y1 < 9
BEGIN
	SET @y2 = (SELECT CAST(COUNT(*) AS int) FROM Товары)
	SELECT @y1 'Средняя цена', @y2 'Количество товарных единиц'
END 
ELSE IF @y1 > 9 PRINT 'Средняя цена больше 9'
ELSE IF @y1 < 12 PRINT 'Средняя цена от 9 до 12'
ELSE PRINT 'Средняя цена больше 12'


---3
PRINT 'Версия SQL: ' + CAST(@@VERSION  AS varchar(300));
PRINT 'Уровень вложенности транзакции: ' + CAST(@@TRANCOUNT  AS varchar);
PRINT 'Системный идентификатор процесса, назначенный сервером текущему подключению: ' + cast(@@SPID as varchar);
PRINT 'Количество обратботанных строк: ' + CAST(@@ROWCOUNT  AS varchar);
PRINT 'Код последней ошибки: ' + CAST(@@ERROR  AS varchar);
PRINT 'Имя сервера: ' + CAST(@@SERVERNAME  AS varchar);
PRINT 'Проверка результата считывания строк результирующего набора: ' + CAST(@@FETCH_STATUS  AS varchar);
PRINT 'Уровень вложенности текущей процедуры: ' + cast(@@NESTLEVEL  AS varchar);

----4
-----1
DECLARE @tt int = 5,
		@x float = 3.7,
		@z float;
IF @t > @x 
	SET @z = POWER(SIN(@t), 2);
ELSE IF @t = @x
	SET @z = 1 - EXP(@x - 2);
ELSE SET @z = 4 * (@t + @x);
PRINT 'z = ' + CAST(@z as varchar(10));

-----2
DECLARE @name varchar(50)
SELECT @name = Название
FROM Поставщики;

	SELECT TOP 1 LEFT(@name, 3) [Сокращенная форма названия поставщика] FROM Поставщики;

-----3
DECLARE @currentDate DATE = GETDATE();
DECLARE @targetMonth INT = 9;
SELECT 
    Артикул, 
    Дата_заказа, 
    DATEDIFF(DAY, Дата_заказа, @currentDate) AS [Дней прошло с заказа]
FROM Поставки
WHERE MONTH(Дата_заказа) = @targetMonth;

-----4
SELECT Код_поставщика, Дата_заказа, DATENAME(WEEKDAY, Дата_заказа) AS [День недели]
FROM Поставки
WHERE Дата_заказа IS NOT NULL;
-----5
DECLARE @x5 int = (SELECT COUNT(*) FROM Товары);
IF (SELECT COUNT(*) FROM Товары) < 14
BEGIN 
  PRINT 'Количесвто товаров < 14';
END;
ELSE 
BEGIN 
  PRINT 'Количесвто товаров >= 14 ';
END;

---6
DECLARE @expensive nvarchar(50) = 'Дорогой товар';
DECLARE @normal nvarchar(50) = 'Средний по цене товар';
DECLARE @cheap nvarchar(50) = 'Дешевый товар';

SELECT *
FROM (
    SELECT CASE 
            WHEN Цена between 3 and 5 THEN @expensive
            WHEN Цена between 1 and 2 THEN @normal
            ELSE @cheap
        END AS [Пояснение цены], 
        COUNT(*) AS [Количество товаров]
    FROM Товары
    GROUP BY CASE
            WHEN Цена between 3 and 5 THEN @expensive
            WHEN Цена between 1 and 2 THEN @normal
            ELSE @cheap
        END
) AS TABL;

---7
CREATE TABLE #EXPLRE (PAR1 varchar(50), PAR2 int, PAR3 int);
SET NOCOUNT ON; --откл. вывода счетчиков
DECLARE @ii int = 0;
WHILE @ii < 10
BEGIN
	INSERT INTO #EXPLRE(PAR1, PAR2, PAR3)
	VALUES 
	(
		CONCAT ('Параметр: ', FLOOR(100 * RAND())),
		FLOOR(100 * RAND()),
		FLOOR (50 * RAND())
	);
set @ii = @ii + 1;
end;

select * from #EXPLRE;
drop table #EXPLRE;

---8
DECLARE @p int = 1
	PRINT @p + 1
	PRINT @p + 2
	RETURN 
	PRINT @p + 3;

---9
BEGIN TRY 
	INSERT INTO dbo.Поставки (Код_поставщика, Артикул)
    VALUES ('SUP001', 'PPPPP');
END TRY
BEGIN CATCH
	PRINT 'Код ошибки: ' + CAST(error_number() AS varchar);
	PRINT 'Сообщение об ошибке: ' + error_message();
	PRINT 'Номер строки с ошибкой: ' + CAST(error_line() AS varchar);
	PRINT 'Уровень серьезности ошибки: ' + CAST(error_severity() AS varchar);
	PRINT 'Метка ошибки: ' + CAST(error_state() AS varchar);
END CATCH;