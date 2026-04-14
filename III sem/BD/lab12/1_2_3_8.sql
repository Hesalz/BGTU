---1 (неявная транзакция)
SET NOCOUNT ON
IF  EXISTS (SELECT * FROM  SYS.OBJECTS        -- таблица X есть?
	WHERE OBJECT_ID= object_id(N'DBO.X') )	            
DROP TABLE X;   

DECLARE @c INT, @flag CHAR = 'c';           -- commit или rollback?
SET IMPLICIT_TRANSACTIONS  ON   -- включ. режим неявной транзакции
CREATE table X(K INT );                         -- начало транзакции 
	INSERT X VALUES (1),(2),(3);
	SET @c = (SELECT COUNT(*) FROM X);
	PRINT 'количество строк в таблице X: ' + CAST( @c AS VARCHAR(2));

IF @flag = 'c'  COMMIT;                   -- завершение транзакции: фиксация 
ELSE   ROLLBACK;                                 -- завершение транзакции: откат  
SET IMPLICIT_TRANSACTIONS  OFF   -- выключ. режим неявной транзакции
	
IF  EXISTS (SELECT * FROM  SYS.OBJECTS       -- таблица X есть?
	WHERE OBJECT_ID= object_id(N'DBO.X') )
PRINT 'таблица X есть';  
ELSE PRINT 'таблицы X нет'

SELECT * FROM X;

---2 (атомарность явной транзакции)
BEGIN TRY 
	BEGIN TRAN --- начало явной транзакции
		INSERT Товары VALUES ('IKE73', 'Шина Зима', 140, 14);
	COMMIT TRAN; --- фиксация транзакции 
END TRY
BEGIN CATCH
	PRINT 'Ошибка: ' + CASE
		WHEN ERROR_NUMBER() = 2627 AND PATINDEX('%PK_Товары%', ERROR_MESSAGE()) > 0
		THEN 'дублирование товара'
		ELSE 'неизвестная ошибка: ' + CAST(ERROR_NUMBER() AS VARCHAR(5)) + ERROR_MESSAGE()
	END;
	IF @@TRANCOUNT > 0 ROLLBACK TRAN;
END CATCH;

--- 3 (SAVE TRAN)
DECLARE @point VARCHAR(32); --- макс. длина имени 32
BEGIN TRY
	BEGIN TRAN  --- начало явной транзакции
		DELETE Товары WHERE Товар = 'IKE73';
		SET @point = 'p1'; SAVE TRAN @point; --- контрольная точка p1
		INSERT Товары VALUES ('JJD002', 'Диск ALM', 192, 12);
		SET @point = 'p2'; SAVE TRAN @point; ---контрольная точка p2
		INSERT Товары VALUES ('IKE73', 'Шина Зима', 140, 14);
	COMMIT TRAN; --- фиксация транзакции
END TRY
BEGIN CATCH
	PRINT 'Ошибка: ' + CASE 
		WHEN ERROR_NUMBER() = 2627 AND PATINDEX('%PK_Товары%', ERROR_MESSAGE()) > 0
		THEN 'дублирование товара'
		ELSE 'неизвестная ошибка: ' + CAST(ERROR_NUMBER() AS VARCHAR(5)) + ERROR_MESSAGE()
	END;
	IF @@TRANCOUNT > 0 
	 BEGIN 
		PRINT 'Контрольная точка: ' + @point;
		ROLLBACK TRAN @point; --- откат к контрольной точке
		COMMIT TRAN; --- фиксация изменений, выплненных до контрольной точки
	END;
END CATCH;

---8 (вложенные транзакции)
BEGIN TRAN --- внешняя транзакция 
	INSERT Товары VALUES ('GGG', 'ggg', 300, 16);
		BEGIN TRAN  ---внутренняя транзакция
			UPDATE Товары SET Название_детали = 'GGG' WHERE Товар = 'GGG';
		COMMIT; ---внутреннняя транзакция
IF @@TRANCOUNT > 0 ROLLBACK;  ---внешняя транзакция
SELECT 
	(SELECT COUNT(*) FROM Товары WHERE Товар = 'GGG') 'Товары',
	(SELECT COUNT(*) FROM Поставки WHERE Артикул = 'GGG') 'Поставки';

