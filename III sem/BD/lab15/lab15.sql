---1
 CREATE TABLE t 
 (
	ID INT IDENTITY, --- номер
	ST VARCHAR(20) CHECK (ST IN ('INS', 'DEL', 'UPD')), ---DML-оператор
	TRN VARCHAR(50), --- имя триггера
	C VARCHAR(300), --- комментарий
 )

 CREATE TRIGGER TRIG_t 
			ON Товары AFTER INSERT 
AS DECLARE @a1 VARCHAR(20), @a2 VARCHAR(20), @a3 INT, @a4 INT, @in VARCHAR(300);
PRINT 'Операция вставки';
SET @a1 = (SELECT Товар FROM INSERTED);
SET @a2 = (SELECT Название_детали FROM INSERTED);
SET @a3 = (SELECT Количество_деталей_на_складе FROM INSERTED);
SET @a4 = (SELECT Цена FROM INSERTED);
SET @in = @a1 + ' ' + @a2 + ' ' + CAST(@a3 AS VARCHAR(4)) + ' ' + CAST(@a4 AS VARCHAR(5));

INSERT INTO t(ST, TRN, C) VALUES ('INS', 'TRIFG_t_Ins', @in);
RETURN;

INSERT INTO Товары VALUES ('AAA', 'aaaaa', 300, 23); 
SELECT * FROM t;
SELECT * FROM Товары;

DROP TABLE t;
DROP TRIGGER TRIG_t;	
DELETE FROM Товары WHERE Товар = 'AAA';

---2, 3, 4
CREATE TRIGGER TRIG_t_2 ON Товары AFTER INSERT, DELETE, UPDATE
		AS DECLARE @a1 VARCHAR(20), @a2 VARCHAR(20), @a3 INT, @a4 INT, @in VARCHAR(300);
		DECLARE @ins INT = (SELECT COUNT(*) FROM INSERTED),
				@del INT = (SELECT COUNT(*) FROM DELETED);

IF @ins > 0 AND @del = 0
BEGIN
	PRINT 'Событие: INSERT';
	SET @a1 = (SELECT Товар FROM INSERTED);
	SET @a2 = (SELECT Название_детали FROM INSERTED);
	SET @a3 = (SELECT Количество_деталей_на_складе FROM INSERTED);
	SET @a4 = (SELECT Цена FROM INSERTED);
	SET @in = @a1 + ' ' + @a2 + ' ' + CAST(@a3 AS VARCHAR(4)) + ' ' + CAST(@a4 AS VARCHAR(5));

	INSERT INTO t(ST, TRN, C) VALUES ('INS', 'TRIFG_t_Ins', @in);
END;
ELSE IF @ins = 0 AND @del > 0
BEGIN 
	PRINT 'Событие: DELETE';
	SET @a1 = (SELECT Товар FROM DELETED);
	SET @a2 = (SELECT Название_детали FROM DELETED);
	SET @a3 = (SELECT Количество_деталей_на_складе FROM DELETED);
	SET @a4 = (SELECT Цена FROM DELETED);
	SET @in = @a1 + ' ' + @a2 + ' ' + CAST(@a3 AS VARCHAR(4)) + ' ' + CAST(@a4 AS VARCHAR(5));

	INSERT INTO t(ST, TRN, C) VALUES ('DEL', 'TRIFG_t_Del', @in);
END;
ELSE IF @ins > 0 AND @del > 0
BEGIN 
	PRINT 'Событие: UPDATE';
	SET @a1 = (SELECT Товар FROM INSERTED);
	SET @a2 = (SELECT Название_детали FROM INSERTED);
	SET @a3 = (SELECT Количество_деталей_на_складе FROM INSERTED);
	SET @a4 = (SELECT Цена FROM INSERTED);
	SET @in = @a1 + ' ' + @a2 + ' ' + CAST(@a3 AS VARCHAR(4)) + ' ' + CAST(@a4 AS VARCHAR(5));
	SET @a1 = (SELECT Товар FROM DELETED);
	SET @a2 = (SELECT Название_детали FROM DELETED);
	SET @a3 = (SELECT Количество_деталей_на_складе FROM DELETED);
	SET @a4 = (SELECT Цена FROM DELETED);
	SET @in = @a1 + ' ' + @a2 + ' ' + CAST(@a3 AS VARCHAR(4)) + ' ' + CAST(@a4 AS VARCHAR(5));

	INSERT INTO t(ST, TRN, C) VALUES ('UPD', 'TRIFG_t_Upd', @in);
END;
RETURN;

INSERT INTO Товары VALUES('BBB', 'bbbbb', 150, 17);
UPDATE Товары SET Количество_деталей_на_складе = 50 WHERE Товар = 'BBB';
DELETE FROM Товары WHERE Товар = 'BBB';
SELECT * FROM t;


DROP TRIGGER TRIG_t_2;	
---5
ALTER TABLE Товары ADD CONSTRAINT Цена CHECK(Цена <=30);

CREATE TRIGGER TRIG_AfterUpdate 
		ON Товары AFTER UPDATE
AS
BEGIN
	PRINT 'Изменения в таблице Товары зарегистрированы'
END;

UPDATE Товары SET Цена = 900 WHERE Товар = 'JEK002';

DROP TRIGGER TRIG_AfterUpdate;	
---6
CREATE TRIGGER TOV_AFTER_UPDA ON Товары AFTER UPDATE 
	AS PRINT 'TOV_AFTER_UPDATE_A';
	RETURN;
GO

CREATE TRIGGER TOV_AFTER_UPDB ON Товары AFTER UPDATE 
	AS PRINT 'TOV_AFTER_UPDATE_B';
	RETURN;
GO

CREATE TRIGGER TOV_AFTER_UPDC ON Товары AFTER UPDATE 
	AS PRINT 'TOV_AFTER_UPDATE_C';
	RETURN;
GO

SELECT t.name, e.type_desc
	FROM sys.triggers  t join  sys.trigger_events e  
                  on t.object_id = e.object_id  
                            where OBJECT_NAME(t.parent_id) = 'Товары'
							and e.type_desc = 'UPDATE';

---
SELECT name, type_desc
FROM sys.triggers
WHERE parent_id = OBJECT_ID('Товары');
---

EXEC SP_SETTRIGGERORDER @triggername = 'TOV_AFTER_UPDC', 
	                        @order = 'First', @stmttype = 'UPDATE';
EXEC SP_SETTRIGGERORDER @triggername = 'TOV_AFTER_UPDA', 
	                        @order = 'Last', @stmttype = 'UPDATE';

							UPDATE Товары
SET Количество_деталей_на_складе = Количество_деталей_на_складе + 1
WHERE Товар = 'PRD003';

DROP TRIGGER TOV_AFTER_UPDA;
DROP TRIGGER TOV_AFTER_UPDB;
DROP TRIGGER TOV_AFTER_UPDC;

---7
CREATE TRIGGER t_TRAN 
				ON Товары AFTER INSERT, DELETE, UPDATE
					AS DECLARE @c INT = (SELECT SUM(Количество_деталей_на_складе) FROM Товары);
IF (@c > 10000)
BEGIN
	RAISERROR('Общее кол-во товаров на складе не может быть больше 1000', 10, 1);
	ROLLBACK;
END;
RETURN;

UPDATE Товары SET Количество_деталей_на_складе = 10000 WHERE Название_детали = 'Колодка';

DROP TRIGGER t_TRAN;
---8
CREATE TRIGGER t_INSTEAD_OF
			ON Поставки INSTEAD OF DELETE
				AS RAISERROR (N'Удаление запрещено', 10, 1);
RETURN;

DELETE FROM Поставки WHERE Артикул = 'PRD001';
SELECT * FROM Поставки;

DROP TRIGGER t_INSTEAD_OF;	
---9
CREATE TRIGGER DDL_POSTAVKI ON DATABASE
			FOR DDL_DATABASE_LEVEL_EVENTS AS
				DECLARE @t NVARCHAR(50) = EVENTDATA().value('(/EVENT_INSTANCE/EventType)[1]', 'nvarchar(50)');
				DECLARE @t1 NVARCHAR(50) = EVENTDATA().value('(/EVENT_INSTANCE/ObjectName)[1]', 'nvarchar(50)');
				DECLARE @t2 NVARCHAR(50) = EVENTDATA().value('(/EVENT_INSTANCE/ObjectType)[1]', 'nvarchar(50)');
IF @t1 =N'Поставки'
BEGIN 
	PRINT 'Тип события: '+@t;
	PRINT 'Имя объекта: '+@t1;
	PRINT 'Тип объекта: '+@t2;
	RAISERROR(N'Операции с таблицей Поставки запрещены', 16, 1);
	ROLLBACK;
END;

ALTER TABLE Поставки DROP COLUMN Дата_заказа;
DROP TRIGGER DDL_POSTAVKI;	

