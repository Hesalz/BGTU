---1
DECLARE @TV CHAR(20), @T CHAR(300) = '';
DECLARE POST CURSOR 
	FOR SELECT Название FROM Поставщики;

OPEN POST;
FETCH POST INTO @TV;
PRINT 'Поставщики';
WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @T = RTRIM(@TV) + ', ' + @T;
		FETCH POST INTO @TV;
	END;
PRINT @T;
CLOSE POST;
DEALLOCATE POST;
---2
DECLARE @TVV CHAR(20), @TT CHAR(300) = '';
DECLARE POSTAV CURSOR LOCAL
	FOR SELECT Название FROM Поставщики;

OPEN POSTAV;
FETCH POSTAV INTO @TVV;
PRINT 'Поставщики';
WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @TT = RTRIM(@TVV) + ', ' + @TT;
		FETCH POSTAV INTO @TVV;
	END;
PRINT @TT;
CLOSE POSTAV;

DECLARE @TV CHAR(20), @T CHAR(300) = '';
DECLARE N CURSOR GLOBAL
	FOR SELECT Название FROM Поставщики;

OPEN N;
FETCH N INTO @TV;
PRINT 'Поставщики';
WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @T = RTRIM(@TV) + ', ' + @T;
		FETCH N INTO @TV;
	END;
PRINT @T;
CLOSE N;
DEALLOCATE N;

---3
CREATE TABLE #P (
    Поставщик CHAR(50),
    Товар CHAR(50),
    Количество INT
);

INSERT INTO #P (Поставщик, Товар, Количество) VALUES
('Компания А', 'Мониторы', 50),
('Компания Б', 'Клавиатуры', 30),
('Компания В', 'Мыши', 20),
('Компания Г', 'Ноутбуки', 15),
('Компания Д', 'Принтеры', 10),
('Компания Е', 'Сканеры', 5),
('Компания Ж', 'Серверы', 8),
('Компания З', 'Телефоны', 25),
('Компания И', 'Планшеты', 12),
('Компания К', 'Проекторы', 7);
SELECT * FROM #P;

DECLARE @TID CHAR(50), @TNM CHAR(50), @TGN INT;
DECLARE POSTAVKA CURSOR LOCAL STATIC 
	FOR SELECT Поставщик, Товар, Количество
	FROM #P
	OPEN POSTAVKA;
	PRINT 'Количество строк: ' + CAST(@@CURSOR_ROWS as varchar(5));
	UPDATE #P set Количество = 13 WHERE Товар = 'Серверы';
	INSERT #P (Поставщик, Товар, Количество)
			values ('Компания Л', 'Интерактивные доски', 5);
	FETCH POSTAVKA INTO @TID, @TNM, @TGN;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		print @TID + ' ' + @TNM + ' ' + CAST (@TGN AS CHAR(4));
		FETCH POSTAVKA INTO @TID, @TNM, @TGN;
	END;
	CLOSE POSTAVKA;
	DROP TABLE #P;
	--

	CREATE TABLE #P2 (
    Поставщик CHAR(50),
    Товар CHAR(50),
    Количество INT
);

INSERT INTO #P2 (Поставщик, Товар, Количество) VALUES
('Компания А', 'Мониторы', 50),
('Компания Б', 'Клавиатуры', 30),
('Компания В', 'Мыши', 20),
('Компания Г', 'Ноутбуки', 15),
('Компания Д', 'Принтеры', 10),
('Компания Е', 'Сканеры', 5),
('Компания Ж', 'Серверы', 8),
('Компания З', 'Телефоны', 25),
('Компания И', 'Планшеты', 12),
('Компания К', 'Проекторы', 7);
SELECT * FROM #P2;
<<<<<<< HEAD

DECLARE @TID CHAR(10), @TNM CHAR(40), @TGN CHAR(2);
DECLARE POSTAVKA2 CURSOR LOCAL DYNAMIC
	FOR SELECT Поставщик, Товар, Количество
	FROM #P2
=======
DECLARE @TID CHAR(10), @TNM CHAR(40), @TGN CHAR(2);
DECLARE POSTAVKA2 CURSOR LOCAL DYNAMIC
	FOR SELECT Поставщик, Товар, Количество
	FROM #P
>>>>>>> 1eb568908d67eee9be534943dc44ad717487737c
	OPEN POSTAVKA2;
	PRINT 'Количество строк: ' + CAST(@@CURSOR_ROWS as varchar(5));
	UPDATE #P2 set Количество = 13 WHERE Товар = 'Серверы';
	INSERT #P2 (Поставщик, Товар, Количество)
			values ('Компания Л', 'Интерактивные доски', 5);
	FETCH POSTAVKA2 INTO @TID, @TNM, @TGN;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		print @TID + ' ' + @TNM + ' ' + @TGN;
		FETCH POSTAVKA2 INTO @TID, @TNM, @TGN;
	END;
	CLOSE POSTAVKA2;
	DROP TABLE #P2;
----- 4
DECLARE @TC INT, @RN CHAR(50);
DECLARE PRIMER1 CURSOR LOCAL DYNAMIC SCROLL
	FOR SELECT ROW_NUMBER() OVER (ORDER BY Товар) N, Товар FROM Товары 
			WHERE Количество_деталей_на_складе > 10;
	OPEN PRIMER1;
	FETCH PRIMER1 INTO @TC, @RN;
<<<<<<< HEAD
	PRINT 'Первая строка: ' + CAST(@TC AS VARCHAR(3)) + ' ' + RTRIM(@RN);
=======
	PRINT 'След. строка: ' + CAST(@TC AS VARCHAR(3)) + ' ' + RTRIM(@RN);
>>>>>>> 1eb568908d67eee9be534943dc44ad717487737c
	FETCH LAST FROM PRIMER1 INTO @TC, @RN;
	PRINT 'Последняя строка: ' + CAST(@TC AS VARCHAR(3)) + ' ' + RTRIM(@RN);

	CLOSE PRIMER1;


----- 5
CREATE TABLE #P (
    Поставщик CHAR(50),
    Товар CHAR(50),
    Количество INT
);

INSERT INTO #P (Поставщик, Товар, Количество) VALUES
('Компания А', 'Мониторы', 50),
('Компания Б', 'Клавиатуры', 30),
('Компания В', 'Мыши', 20),
('Компания Г', 'Ноутбуки', 15),
('Компания Д', 'Принтеры', 10),
('Компания Е', 'Сканеры', 5),
('Компания Ж', 'Серверы', 8),
('Компания З', 'Телефоны', 25),
('Компания И', 'Планшеты', 12),
('Компания К', 'Проекторы', 7);

DECLARE @TC INT, @RN CHAR(50), @RK CHAR(50);
DECLARE PRIMER2 CURSOR LOCAL DYNAMIC
    FOR SELECT Поставщик, Товар, Количество FROM #P
		FOR UPDATE;
OPEN PRIMER2;
FETCH FROM PRIMER2 INTO @RN, @RK, @TC;
UPDATE #P SET Количество = Количество + 5
    WHERE CURRENT OF PRIMER2;
FETCH LAST FROM PRIMER2 INTO @RN, @RK, @TC;
DELETE FROM #P
	WHERE CURRENT OF PRIMER2;
CLOSE PRIMER2;
SELECT * FROM #P;
DROP TABLE #P;


---- 6
CREATE TABLE #P (
    Поставщик CHAR(50),
    Товар CHAR(50),
    Количество INT
);

INSERT INTO #P (Поставщик, Товар, Количество) VALUES
('Компания А', 'Мониторы', 50),
('Компания Б', 'Клавиатуры', 30),
('Компания В', 'Мыши', 20),
('Компания Г', 'Ноутбуки', 15),
('Компания Д', 'Принтеры', 10),
('Компания Е', 'Сканеры', 5),
('Компания Ж', 'Серверы', 8),
('Компания З', 'Телефоны', 25),
('Компания И', 'Планшеты', 12),
('Компания К', 'Проекторы', 7);

DECLARE @Поставщик CHAR(50), @Товар CHAR(50), @Количество INT;
DECLARE CURSOR_DELETE CURSOR LOCAL DYNAMIC
FOR SELECT Поставщик, Товар, Количество
    FROM #P
    WHERE Количество <= 15
FOR UPDATE;
OPEN CURSOR_DELETE;

FETCH FROM CURSOR_DELETE INTO @Поставщик, @Товар, @Количество;
WHILE @@FETCH_STATUS = 0
BEGIN
    DELETE FROM #P 
    WHERE CURRENT OF CURSOR_DELETE;

    FETCH FROM CURSOR_DELETE INTO @Поставщик, @Товар, @Количество;
END;

CLOSE CURSOR_DELETE;
SELECT * FROM #P;

----
CREATE TABLE #P10 (
    Поставщик CHAR(50),
    Товар CHAR(50),
    Количество INT
);

INSERT INTO #P10 (Поставщик, Товар, Количество) VALUES
('Компания А', 'Мониторы', 50),
('Компания Б', 'Клавиатуры', 30),
('Компания В', 'Мыши', 20),
('Компания Г', 'Ноутбуки', 15),
('Компания Д', 'Принтеры', 10),
('Компания Е', 'Сканеры', 5),
('Компания Ж', 'Серверы', 8),
('Компания З', 'Телефоны', 25),
('Компания И', 'Планшеты', 12),
('Компания К', 'Проекторы', 7);

DECLARE @Поставщик CHAR(50), @Товар CHAR(50), @Количество INT;
DECLARE CURSOR_UPDATE CURSOR LOCAL DYNAMIC
FOR SELECT Поставщик, Товар, Количество
    FROM #P10
    WHERE Товар = 'Мыши' 
FOR UPDATE;
OPEN CURSOR_UPDATE;

FETCH NEXT FROM CURSOR_UPDATE INTO @Поставщик, @Товар, @Количество;
WHILE @@FETCH_STATUS = 0
BEGIN
    UPDATE #P10 
    SET Количество = Количество + 1 
    WHERE CURRENT OF CURSOR_UPDATE;

    FETCH NEXT FROM CURSOR_UPDATE INTO @Поставщик, @Товар, @Количество;
END;

CLOSE CURSOR_UPDATE;
SELECT * FROM #P10;
<<<<<<< HEAD
DROP TABLE #P10;


---- доп задание
CREATE TABLE #T (
    a FLOAT,
    s FLOAT,
    y FLOAT
);

INSERT INTO #T (a, s)
VALUES (-1.1, 0.1), (-1.1, 0.2), (-1.1, 0.3), (-1.1, 0.4), (-1.1, 0.5),
       (-5, 0.1), (-5, 0.2), (-5, 0.3), (-5, 0.4), (-5, 0.5),
       (-3, 0.1), (-3, 0.2), (-3, 0.3), (-3, 0.4), (-3, 0.5),
       (-8, 0.1), (-8, 0.2), (-8, 0.3), (-8, 0.4), (-8, 0.5),
       (-2, 0.1), (-2, 0.2), (-2, 0.3), (-2, 0.4), (-2, 0.5);

DECLARE @f FLOAT = 2 * POWER(10, -3);
DECLARE @a FLOAT;
DECLARE @s FLOAT;
DECLARE @y FLOAT;

DECLARE Cur CURSOR LOCAL FOR
SELECT a, s FROM #T;

OPEN Cur;
FETCH NEXT FROM Cur INTO @a, @s;

WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY
        IF @a > 2
        BEGIN
            SET @y = @s * (LOG(5.2 * @f) / (EXP(-5 + @a)));
        END
        ELSE
        BEGIN
            SET @y = (1 + @s * @f - @s * @a) * SQRT(ABS(@a));
        END

        UPDATE #T
        SET y = @y
        WHERE a = @a AND s = @s;
    END TRY
    BEGIN CATCH
        PRINT 'Ошибка a = ' + CAST(@a AS VARCHAR) + ', s = ' + CAST(@s AS VARCHAR);
    END CATCH

    FETCH NEXT FROM Cur INTO @a, @s;
END;

CLOSE Cur;
DEALLOCATE Cur;

SELECT * FROM #T;
DROP TABLE #T;

=======
>>>>>>> 1eb568908d67eee9be534943dc44ad717487737c
