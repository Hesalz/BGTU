CREATE PROCEDURE PREP
    @a NVARCHAR(10),
    @b NVARCHAR(10),
    @c INT,
    @d DATE,
    @e NVARCHAR(50) = NULL
AS
DECLARE @rc INT = 1;
BEGIN TRY
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    BEGIN TRANSACTION;

    INSERT INTO Поставки (Код_поставщика, Артикул, Количество_заказанных_деталей, Дата_заказа, Примечание)
    VALUES (@a, @b, @c, @d, @e);

    COMMIT TRANSACTION;
    RETURN @rc;
END TRY
BEGIN CATCH
    PRINT 'номер ошибки  : ' + CAST(ERROR_NUMBER() AS VARCHAR(6));
    PRINT 'сообщение     : ' + ERROR_MESSAGE();
    PRINT 'уровень       : ' + CAST(ERROR_SEVERITY() AS VARCHAR(6));
    PRINT 'метка         : ' + CAST(ERROR_STATE() AS VARCHAR(8));
    PRINT 'номер строки  : ' + CAST(ERROR_LINE() AS VARCHAR(8));
    IF ERROR_PROCEDURE() IS NOT NULL
        PRINT 'имя процедуры : ' + ERROR_PROCEDURE();

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    RETURN -1;
END CATCH;


declare @rc int;  
exec @rc = PREP @a = 'SUP002', @b = 'IKE073', @c = 53,
@d =  '2023-03-11', @e =  '';  
print 'код ошибки=' + cast(@rc as varchar(3));  

DROP PROCEDURE PREP;
SELECT * FROM Поставки;