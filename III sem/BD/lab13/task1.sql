CREATE PROCEDURE PrTovary 
AS
BEGIN
	DECLARE @k INT = (SELECT COUNT(*) FROM Товары);
	SELECT * FROM Товары;
	RETURN @k;
END;

DECLARE @k INT = 0;
EXEC @k = PrTovary;
PRINT 'Кол-во товаров = ' + CAST(@k AS VARCHAR(3));
