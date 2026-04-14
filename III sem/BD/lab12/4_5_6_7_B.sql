---4
---B---
BEGIN TRANSACTION
SELECT @@SPID
INSERT Товары VALUES ('BBB', 'BBB', 90, 15);
UPDATE Поставщики SET Название = 'OOOO' WHERE Название = 'ДемХаб';
-----t1-----
-----t2-----
ROLLBACK;


---5
---B---
BEGIN TRANSACTION 	  
-----t1------
UPDATE Поставки SET Артикул = 'IKE73' WHERE Код_поставщика = 'JCE016';
COMMIT; 
-----t2-----


---6
---B---
BEGIN TRANSACTION   
-----t1-----
UPDATE Товары SET Цена = 25 WHERE Товар = 'DDD'
COMMIT; 
-----t2-----
DELETE FROM Товары WHERE Товар = 'DDD'
---7
---B---
BEGIN TRANSACTION
	DELETE Товары WHERE Товар = 'JJD002';  
	INSERT Товары VALUES ('JJD002', 'Диск ALM', 310, 15);
	UPDATE Товары SET Цена = 15 WHERE Товар = 'JJD002';
	SELECT  Цена FROM Товары  WHERE Товар = 'JJD002';
-----t1-----
COMMIT; 
SELECT  Цена FROM Товары  WHERE Товар = 'JJD002';
-----t2------
