IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ServiceCategories') AND name = 'NodePath')
    ALTER TABLE ServiceCategories DROP COLUMN NodePath;
ALTER TABLE ServiceCategories ADD NodePath hierarchyid NULL;
GO

DELETE FROM ServiceCategories;
DBCC CHECKIDENT ('ServiceCategories', RESEED, 0);
GO

INSERT INTO ServiceCategories (CategoryName, NodePath) VALUES ('Категории', hierarchyid::GetRoot());
GO

CREATE OR ALTER PROCEDURE ShowSubtree
    @NodeName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NodePath hierarchyid = (SELECT NodePath FROM ServiceCategories WHERE CategoryName = @NodeName);
    IF @NodePath IS NULL
    BEGIN
        PRINT 'Узел "' + @NodeName + '" не найден.';
        RETURN;
    END;

    WITH Tree AS (
        SELECT CategoryName, NodePath, 0 AS Level
        FROM ServiceCategories WHERE NodePath = @NodePath
        UNION ALL
        SELECT c.CategoryName, c.NodePath, t.Level + 1
        FROM ServiceCategories c
        INNER JOIN Tree t ON c.NodePath.GetAncestor(1) = t.NodePath
    )
    SELECT Level AS [Уровень], REPLICATE('  ', Level) + CategoryName AS [Дерево]
    FROM Tree
    ORDER BY NodePath;
END;
GO

CREATE OR ALTER PROCEDURE AddChildNode
    @ParentName NVARCHAR(100),
    @ChildName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ParentPath hierarchyid = (SELECT NodePath FROM ServiceCategories WHERE CategoryName = @ParentName);
    IF @ParentPath IS NULL
    BEGIN
        PRINT 'Родитель "' + @ParentName + '" не найден.';
        RETURN;
    END;

    DECLARE @LastChild hierarchyid = (
        SELECT MAX(NodePath) FROM ServiceCategories 
        WHERE NodePath.GetAncestor(1) = @ParentPath
    );

    DECLARE @NewPath hierarchyid = @ParentPath.GetDescendant(@LastChild, NULL);

    INSERT INTO ServiceCategories (CategoryName, NodePath)
    VALUES (@ChildName, @NewPath);

    PRINT 'Узел "' + @ChildName + '" добавлен в "' + @ParentName + '".';
END;
GO

CREATE OR ALTER PROCEDURE MoveSubtree
    @SourceName NVARCHAR(100),
    @TargetName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @SourcePath hierarchyid = (SELECT NodePath FROM ServiceCategories WHERE CategoryName = @SourceName);
    DECLARE @TargetPath hierarchyid = (SELECT NodePath FROM ServiceCategories WHERE CategoryName = @TargetName);

    IF @SourcePath IS NULL OR @TargetPath IS NULL
    BEGIN
        PRINT 'Один из узлов не найден.';
        RETURN;
    END;

    IF @SourcePath = @TargetPath OR @TargetPath.IsDescendantOf(@SourcePath) = 1
    BEGIN
        PRINT 'Ошибка: нельзя переместить узел в самого себя или в своего потомка.';
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @LastChild hierarchyid = (
            SELECT MAX(NodePath) FROM ServiceCategories 
            WHERE NodePath.GetAncestor(1) = @TargetPath
        );
        DECLARE @NewRootPath hierarchyid = @TargetPath.GetDescendant(@LastChild, NULL);

        UPDATE ServiceCategories
        SET NodePath = NodePath.GetReparentedValue(@SourcePath, @NewRootPath)
        WHERE NodePath.IsDescendantOf(@SourcePath) = 1;

        COMMIT;
        PRINT 'Ветка "' + @SourceName + '" перемещена в "' + @TargetName + '".';
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END;
GO

-- test
EXEC AddChildNode 'Категории', 'Услуги';
EXEC AddChildNode 'Категории', 'Товары';
EXEC AddChildNode 'Услуги', 'Ремонт';
EXEC AddChildNode 'Услуги', 'Диагностика';
EXEC AddChildNode 'Ремонт', 'Запчасти';
EXEC AddChildNode 'Ремонт', 'Гарантия';
EXEC AddChildNode 'Товары', 'Софт';
GO

EXEC ShowSubtree 'Категории';

--2 
EXEC ShowSubtree 'Ремонт';

--3
EXEC AddChildNode 'Ремонт', 'Тест';

EXEC ShowSubtree 'Категории';

--4
EXEC MoveSubtree 'Ремонт', 'Товары';

--final
EXEC ShowSubtree 'Категории';
GO