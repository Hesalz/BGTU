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
DECLARE @TV CHAR(20), @T CHAR(300) = '';
DECLARE POSTAV CURSOR LOCAL
	FOR SELECT Название FROM Поставщики;

OPEN POSTAV;
FETCH POSTAV INTO @TV;
PRINT 'Поставщики';
WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @T = RTRIM(@TV) + ', ' + @T;
		FETCH POSTAV INTO @TV;
	END;
PRINT @T;
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

DECLARE @TID CHAR(10), @TNMM CHAR(40), @TGN CHAR(1);
DECLARE POSTAVKA CURSOR LOCAL STATIC 
	FOR SELECT Поставщик, Товар, Количество
	FROM #P
	WHERE Поставщик = 'Компания А'
	OPEN POSTAVKA;
	PRINT "Количество строк: " + CAST(@@CURSOR_ROWS as varchar(5));
	UPDATE #P set Количество = 13 WHERE Товар = 'Серверы';
	INSERT #P (Поставщик, Товар, Количество)
			values ('Компания Л', 'Интерактивные доски', 5);
	FETCH POSTAVKA INTO @TID, @TNM, @TGN;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		print @TID + '' + @TNM + '' + @TGN;
		FETCH POSTAVKA INTO @TID, @TNM, @TGN;
	END;
	CLOSE POSTAVKA;