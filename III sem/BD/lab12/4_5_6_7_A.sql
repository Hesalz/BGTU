---4
---A---
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
BEGIN TRANSACTION 
	-----t1-----
	SELECT @@SPID, 'insert Товары' 'результат', * FROM Товары WHERE Товар = 'BBB';
	SELECT @@SPID, 'insert Поставщики' 'результат', Поставщик, Название FROM Поставщики WHERE Название = 'OOOO';
COMMIT;
-----t2-----

---5 
---A---
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
BEGIN TRANSACTION 
	SELECT COUNT(*) FROM Поставки WHERE Код_поставщика = 'JCE016';
	-----t1-----
	-----t2-----
	SELECT 'update Поставки' 'результат', COUNT(*) from Поставки WHERE Артикул = 'IKE73';
COMMIT; 

---6 
---A---
SET TRANSACTION ISOLATION LEVEL   REPEATABLE READ 
BEGIN TRANSACTION 
	INSERT Товары VALUES ('DDD', 'ddd', 140, 10);
	SELECT Цена FROM Товары WHERE Товар = 'DDD';
	-----t1-----
	-----t2-----
	SELECT CASE
			WHEN Цена = 12 
			THEN 'insert Товары'  
			ELSE '!!' 
END 'результат', Цена FROM Товары  WHERE Товар = 'DDD';
COMMIT; 

---7
---A---
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE 
BEGIN TRANSACTION 
	DELETE Товары WHERE Товар = 'JJD002';  
	INSERT Товары VALUES ('JJD002', 'Диск ALM', 310, 15);
	UPDATE Товары SET Цена = 25 WHERE Товар = 'JJD002';
	SELECT  Цена FROM Товары  WHERE Товар = 'JJD002';
	-----t1------
	SELECT  Цена FROM Товары  WHERE Товар = 'JJD002';
	-----t2-----
COMMIT; 	
