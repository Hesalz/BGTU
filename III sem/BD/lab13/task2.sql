USE [lab3]
GO

/****** Object:  StoredProcedure [dbo].[PrTovary]    Script Date: 24.12.2024 6:14:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[PrTovary] @p VARCHAR(20), @c INT OUTPUT
AS
BEGIN
	DECLARE @s INT = (SELECT COUNT(*) FROM Товары);
	PRINT 'параметры: @p = ' + @p + ', @c = ' + CAST(@c AS VARCHAR(3));
	SELECT * FROM Товары WHERE Цена = @p;
	SET @c = @@ROWCOUNT;
	PRINT 'параметры после установки для @c нового значения: @p = ' + @p + ', @c = ' + CAST(@c AS VARCHAR(3));
	RETURN @s;
END;
GO


DECLARE @s INT = 0, @r INT = 0, @l INT = 3;
EXEC @s = PrTovary @p = @l, @c = @r OUTPUT;
PRINT 'Кол-во товаров всего: ' + CAST(@s AS VARCHAR(3));
PRINT 'Кол-во товаров со стоимостью = ' + CAST(@l AS VARCHAR(3)) + ': ' + CAST(@r AS VARCHAR(3));