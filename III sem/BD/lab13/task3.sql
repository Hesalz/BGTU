ALTER PROCEDURE [dbo].[PrTovary] @p INT
AS
BEGIN
	SELECT * FROM Товары WHERE Цена = @p;
END;
GO


---3
CREATE TABLE #temptovary (
    Товар NCHAR(10),
    Название_детали NVARCHAR(25),
    Количество_деталей INT,
    Цена INT
);

INSERT #temptovary EXEC PrTovary @p = 14;
INSERT #temptovary EXEC PrTovary @p = 3;

SELECT * FROM #temptovary;

DROP TABLE #temptovary