CREATE PROCEDURE POST_REPORT @p NVARCHAR(10)
AS
DECLARE @rc INT = 0;
BEGIN TRY 
	DECLARE @tv NVARCHAR(10), @t CHAR(300) = '';
	DECLARE PREP CURSOR FOR
	SELECT Артикул FROM Поставки WHERE RTRIM(Код_поставщика) = RTRIM(@p);
	IF NOT EXISTS (SELECT Артикул FROM Поставки WHERE Код_поставщика = @p)
		RAISERROR('ошибка', 11, 1);
	ELSE
		OPEN PREP;
		FETCH PREP INTO @tv;
		PRINT 'поставленные товары: ';
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @t = RTRIM(@tv) + ', ' + @t;
			SET @rc = @rc + 1;
			FETCH PREP INTO @tv;
		END;
	PRINT @t;
	CLOSE PREP;
	RETURN @rc;
END TRY
BEGIN CATCH
	PRINT 'ошибка в параметрах'
	IF ERROR_PROCEDURE() IS NOT NULL
		PRINT 'имя процедуры: ' + ERROR_PROCEDURE();
	RETURN @rc;
END CATCH;

DECLARE @rc INT;
EXEC @rc = POST_REPORT @p = 'SUP001';
PRINT 'количество товаров = ' + CAST(@rc AS VARCHAR(3));

DROP PROCEDURE POST_REPORT;
DEALLOCATE PREP;