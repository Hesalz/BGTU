CREATE TABLE #EXPLRE (PAR1 varchar(50), PAR2 int, PAR3 int);
SET NOCOUNT ON;
DECLARE @ii int = 0;
WHILE @ii < 1000
BEGIN
	INSERT INTO #EXPLRE(PAR1, PAR2, PAR3)
	VALUES 
	(
		CONCAT ('Параметр: ', FLOOR(500 * RAND())),
		FLOOR(500 * RAND()),
		FLOOR (1000 * RAND())
	);
SET @ii = @ii + 1;
END;

exec SP_HELPINDEX 'Поставки';
exec SP_HELPINDEX 'Поставщики';
exec SP_HELPINDEX 'Товары';

SELECT * FROM #EXPLRE WHERE PAR2 > 350;
DROP TABLE #EXPLRE;

CREATE CLUSTERED INDEX #EXPLRE_CL on #EXPLRE(PAR2 ASC);
DROP INDEX #EXPLRE_CL ON #EXPLRE;

---- 2

CREATE TABLE #EXPLRE2 (PAR1 varchar(50), PAR2 int, PAR3 int);
SET NOCOUNT ON; --откл. вывода счетчиков
DECLARE @FF int = 0;
WHILE @FF < 1000
BEGIN
	INSERT INTO #EXPLRE2(PAR1, PAR2, PAR3)
	VALUES 
	(
		CONCAT ('Параметр: ', FLOOR(500 * RAND())),
		FLOOR(500 * RAND()),
		FLOOR (1000 * RAND())
	);
SET @FF = @FF + 1;
END;

SELECT * FROM #EXPLRE2
WHERE PAR3 > 350 AND PAR2 > 100;
SELECT * FROM #EXPLRE2
WHERE PAR3 = 310 AND PAR2 > 100;

DROP TABLE #EXPLRE2;
CREATE INDEX #EXPLRE2_NONCLU ON #EXPLRE2(PAR3, PAR2);

---- 3 
CREATE TABLE #EXPLRE3 (PAR1 varchar(50), PAR2 int, PAR3 int);
SET NOCOUNT ON; --откл. вывода счетчиков
DECLARE @FF int = 0;
WHILE @FF < 10000
BEGIN
	INSERT INTO #EXPLRE3(PAR1, PAR2, PAR3)
	VALUES 
	(
		CONCAT ('Параметр: ', FLOOR(500 * RAND())),
		FLOOR(500 * RAND()),
		FLOOR (1000 * RAND())
	);
SET @FF = @FF + 1;
END;

DROP TABLE #EXPLRE3;


SELECT PAR3 FROM #EXPLRE3 WHERE PAR2 > 200;
CREATE INDEX #EX3 ON #EXPLRE3(PAR2) INCLUDE (PAR3);

---- 4

CREATE TABLE #EXPLRE4 (PAR1 varchar(50), PAR2 int, PAR3 int);
SET NOCOUNT ON; --откл. вывода счетчиков
DECLARE @FF int = 0;
WHILE @FF < 10000
BEGIN
	INSERT INTO #EXPLRE4(PAR1, PAR2, PAR3)
	VALUES 
	(
		CONCAT ('Параметр: ', FLOOR(500 * RAND())),
		FLOOR(500 * RAND()),
		FLOOR (1000 * RAND())
	);
SET @FF = @FF + 1;
END;

SELECT PAR3 FROM #EXPLRE4 WHERE PAR3 BETWEEN 100 AND 300;
SELECT PAR3 FROM #EXPLRE4 WHERE PAR3 >= 250 AND PAR3 < 350;
CREATE INDEX #EXPLRE4_IDX ON #EXPLRE4(PAR3) WHERE (PAR3 >= 100 AND PAR3 < 350);



----5
CREATE TABLE #EXPLRE5 (PAR1 varchar(50), PAR2 int, PAR3 int);
SET NOCOUNT ON; --откл. вывода счетчиков
DECLARE @FF int = 0;
WHILE @FF < 10000
BEGIN
	INSERT INTO #EXPLRE5(PAR1, PAR2, PAR3)
	VALUES 
	(
		CONCAT ('Параметр: ', FLOOR(500 * RAND())),
		FLOOR(500 * RAND()),
		FLOOR (1000 * RAND())
	);
SET @FF = @FF + 1;
END;

CREATE INDEX #EXPLRE5_IDX ON #EXPLRE5(PAR2);

USE tempdb;
SELECT 
    ii.name [Имя индекса], 
    ss.avg_fragmentation_in_percent [Фрагментация (%)]
FROM sys.dm_db_index_physical_stats(DB_ID(), OBJECT_ID(N'#EXPLRE5'), NULL, NULL, 'DETAILED') ss
  JOIN sys.indexes ii ON ss.OBJECT_ID = ii.OBJECT_ID AND ss.index_id = ii.index_id;

  INSERT TOP(10000) #EXPLRE5(PAR1, PAR2, PAR3) SELECT PAR1,PAR2,PAR3 FROM #EXPLRE5;
  ALTER INDEX #EXPLRE5_IDX ON #EXPLRE5 REORGANIZE;
  ALTER INDEX #EXPLRE5_IDX ON #EXPLRE5 REBUILD WITH (ONLINE = OFF);


  ----6 
  CREATE TABLE #EXPLRE6 (PAR1 varchar(50), PAR2 int, PAR3 int);
SET NOCOUNT ON; --откл. вывода счетчиков
DECLARE @FF int = 0;
WHILE @FF < 10000
BEGIN
	INSERT INTO #EXPLRE6(PAR1, PAR2, PAR3)
	VALUES 
	(
		CONCAT ('Параметр: ', FLOOR(500 * RAND())),
		FLOOR(500 * RAND()),
		FLOOR (1000 * RAND())
	);
SET @FF = @FF + 1;
END;

CREATE INDEX #EXPLRE6_IDX ON #EXPLRE6(PAR1) WITH (FILLFACTOR = 65);
USE tempdb;
SELECT 
    ii.name [Имя индекса], 
    ss.avg_fragmentation_in_percent [Фрагментация (%)]
FROM sys.dm_db_index_physical_stats(DB_ID(), OBJECT_ID(N'#EXPLRE6'), NULL, NULL, 'DETAILED') ss
  JOIN sys.indexes ii ON ss.OBJECT_ID = ii.OBJECT_ID AND ss.index_id = ii.index_id;

  INSERT TOP(50)PERCENT #EXPLRE6(PAR1, PAR2, PAR3) SELECT PAR1,PAR2,PAR3 FROM #EXPLRE6;