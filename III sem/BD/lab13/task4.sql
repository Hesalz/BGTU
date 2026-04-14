CREATE TABLE #temptovary (
    Название_детали NVARCHAR(25) PRIMARY KEY,
    Количество_деталей INT,
    Цена INT
);

CREATE PROCEDURE TovaryInsert
	@t	NVARCHAR(50), @cn REAL, @kl INT = NULL
AS
	DECLARE @rc INT = 1;
BEGIN TRY 
	INSERT INTO #temptovary (Название_детали, Количество_деталей, Цена)
		VALUES (@t, @cn, @kl)
	RETURN @rc;
END TRY
BEGIN CATCH 
	PRINT 'номер ошибки: ' + CAST(ERROR_NUMBER() AS VARCHAR(6));
	PRINT 'сообщение: ' + ERROR_MESSAGE();
	PRINT 'уровень: ' + CAST(ERROR_SEVERITY() AS VARCHAR(6));
	PRINT 'метка: ' + CAST(ERROR_STATE() AS VARCHAR(6));
	PRINT 'номер строки: ' + CAST(ERROR_LINE() AS VARCHAR(6));
	IF ERROR_PROCEDURE() IS NOT NULL
	PRINT 'имя процедуры: ' + ERROR_PROCEDURE();
	RETURN -1;
END CATCH;

DECLARE @rc INT;
EXEC @rc = TovaryInsert @t = Мышь, @cn = 65, @kl = 2500;
PRINT 'код ошибки: ' + CAST(@rc AS VARCHAR(3));

DROP TABLE #temptovary;
DROP PROCEDURE TovaryInsert;