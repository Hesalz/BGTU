-- Задание 2
DECLARE @ServiceName NVARCHAR(200) = N'Ремонт смартфона';
DECLARE @ServiceID INT = (SELECT ServiceID FROM Services WHERE ServiceName = @ServiceName);

-- 2.1 Помесячно
SELECT 
    YEAR(o.OrderDate) AS Year,
    MONTH(o.OrderDate) AS Month,
    COUNT(DISTINCT o.OrderID) AS OrdersCount,
    ISNULL(SUM(oi.Quantity), 0) AS TotalQuantity,
    ISNULL(SUM(oi.Amount), 0) AS TotalAmount
FROM Orders o
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID AND oi.ServiceID = @ServiceID
GROUP BY YEAR(o.OrderDate), MONTH(o.OrderDate)
ORDER BY Year, Month;

-- 2.2 Поквартально
SELECT 
    YEAR(o.OrderDate) AS Year,
    DATEPART(QUARTER, o.OrderDate) AS Quarter,
    COUNT(DISTINCT o.OrderID) AS OrdersCount,
    ISNULL(SUM(oi.Quantity), 0) AS TotalQuantity,
    ISNULL(SUM(oi.Amount), 0) AS TotalAmount
FROM Orders o
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID AND oi.ServiceID = @ServiceID
GROUP BY YEAR(o.OrderDate), DATEPART(QUARTER, o.OrderDate)
ORDER BY Year, Quarter;

-- 2.3 По полугодиям
SELECT 
    YEAR(o.OrderDate) AS Year,
    CASE WHEN MONTH(o.OrderDate) <= 6 THEN 1 ELSE 2 END AS HalfYear,
    COUNT(DISTINCT o.OrderID) AS OrdersCount,
    ISNULL(SUM(oi.Quantity), 0) AS TotalQuantity,
    ISNULL(SUM(oi.Amount), 0) AS TotalAmount
FROM Orders o
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID AND oi.ServiceID = @ServiceID
GROUP BY YEAR(o.OrderDate), CASE WHEN MONTH(o.OrderDate) <= 6 THEN 1 ELSE 2 END
ORDER BY Year, HalfYear;

-- 2.4 По годам
SELECT 
    YEAR(o.OrderDate) AS Year,
    COUNT(DISTINCT o.OrderID) AS OrdersCount,
    ISNULL(SUM(oi.Quantity), 0) AS TotalQuantity,
    ISNULL(SUM(oi.Amount), 0) AS TotalAmount
FROM Orders o
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID AND oi.ServiceID = @ServiceID
GROUP BY YEAR(o.OrderDate)
ORDER BY Year;


-- Задание 3
DECLARE @ServiceName2 NVARCHAR(200) = N'Ремонт смартфона';
DECLARE @ServiceID2 INT = (SELECT ServiceID FROM Services WHERE ServiceName = @ServiceName2);
DECLARE @StartDate DATE = '2024-01-01';
DECLARE @EndDate DATE = '2026-12-31';

WITH 
ServiceData AS (
    SELECT 
        ISNULL(SUM(oi.Quantity), 0) AS ServiceQuantity,
        ISNULL(SUM(oi.Amount), 0) AS ServiceAmount
    FROM Orders o
    LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID AND oi.ServiceID = @ServiceID2
    WHERE o.OrderDate BETWEEN @StartDate AND @EndDate
),
AllServices AS (
    SELECT 
        ISNULL(SUM(oi.Quantity), 0) AS TotalQuantity,
        ISNULL(MAX(ServiceAmounts.Amount), 0) AS MaxAmount
    FROM Orders o
    LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID
    LEFT JOIN (
        SELECT oi2.ServiceID, SUM(oi2.Amount) AS Amount
        FROM Orders o2
        LEFT JOIN OrderItems oi2 ON o2.OrderID = oi2.OrderID
        WHERE o2.OrderDate BETWEEN @StartDate AND @EndDate
        GROUP BY oi2.ServiceID
    ) ServiceAmounts ON 1=1
    WHERE o.OrderDate BETWEEN @StartDate AND @EndDate
)
SELECT 
    s.ServiceQuantity AS [Объём услуг],
    s.ServiceAmount AS [Сумма],
    CASE 
        WHEN a.TotalQuantity > 0 
        THEN CAST(s.ServiceQuantity * 100.0 / a.TotalQuantity AS DECIMAL(10,2))
        ELSE 0 
    END AS [% от общего объёма],
    CASE 
        WHEN a.MaxAmount > 0 
        THEN CAST(s.ServiceAmount * 100.0 / a.MaxAmount AS DECIMAL(10,2))
        ELSE 0 
    END AS [% от макс. суммы]
FROM ServiceData s
CROSS JOIN AllServices a;


-- Задание 4
DECLARE @PageNumber INT = 1;
DECLARE @PageSize INT = 20;

WITH OrderedOrders AS (
    SELECT 
        o.OrderID,
        c.Name AS CustomerName,
        o.OrderDate,
        o.TotalAmount,
        ROW_NUMBER() OVER (ORDER BY o.OrderDate DESC, o.OrderID) AS RowNum
    FROM Orders o
    JOIN Customers c ON o.CustomerID = c.CustomerID
)
SELECT 
    OrderID,
    CustomerName,
    OrderDate,
    TotalAmount,
    RowNum
FROM OrderedOrders
WHERE RowNum BETWEEN (@PageNumber - 1) * @PageSize + 1 AND @PageNumber * @PageSize
ORDER BY RowNum;


-- Задание 5
-- 5.0 Добавляем дублирующую запись для услуги 'Ремонт смартфона'
INSERT INTO Services (ServiceName, Description, DefaultPrice, Unit, CategoryID)
SELECT ServiceName, Description, DefaultPrice, Unit, CategoryID
FROM Services
WHERE ServiceName = 'Ремонт смартфона';

-- 5.1 Показываем дубликаты (до удаления)
SELECT ServiceName, COUNT(*) AS DuplicateCount
FROM Services
GROUP BY ServiceName
HAVING COUNT(*) > 1
ORDER BY DuplicateCount DESC;

-- 5.2 Удаляем дубликаты, оставляя одну запись с наименьшим ServiceID
WITH Duplicates AS (
    SELECT 
        ServiceID,
        ROW_NUMBER() OVER (PARTITION BY ServiceName ORDER BY ServiceID) AS rn
    FROM Services
)
DELETE FROM Services
WHERE ServiceID IN (SELECT ServiceID FROM Duplicates WHERE rn > 1);

-- 5.3 Проверяем, что дубликатов больше нет
SELECT ServiceName, COUNT(*) AS DuplicateCount
FROM Services
GROUP BY ServiceName
HAVING COUNT(*) > 1
ORDER BY DuplicateCount DESC;


-- Задание 6
WITH Months AS (
    SELECT TOP 6
        DATEFROMPARTS(YEAR(DATEADD(MONTH, -n, GETDATE())), MONTH(DATEADD(MONTH, -n, GETDATE())), 1) AS MonthStart,
        EOMONTH(DATEFROMPARTS(YEAR(DATEADD(MONTH, -n, GETDATE())), MONTH(DATEADD(MONTH, -n, GETDATE())), 1)) AS MonthEnd
    FROM (VALUES (1),(2),(3),(4),(5),(6)) AS Numbers(n)
)
SELECT 
    c.CustomerID,
    c.Name AS CustomerName,
    YEAR(m.MonthStart) AS Year,
    MONTH(m.MonthStart) AS Month,
    FORMAT(m.MonthStart, 'yyyy-MM') AS MonthName,
    COUNT(DISTINCT oi.OrderItemID) AS ServicesCount
FROM Months m
CROSS JOIN Customers c
LEFT JOIN Orders o ON c.CustomerID = o.CustomerID 
    AND o.OrderDate BETWEEN m.MonthStart AND m.MonthEnd
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID
GROUP BY c.CustomerID, c.Name, YEAR(m.MonthStart), MONTH(m.MonthStart), FORMAT(m.MonthStart, 'yyyy-MM')
ORDER BY c.CustomerID, Year DESC, Month DESC;


-- Задание 7
WITH ServiceUsage AS (
    SELECT 
        sc.CategoryID,
        sc.CategoryName,
        s.ServiceID,
        s.ServiceName,
        COUNT(oi.OrderItemID) AS UsageCount,
        ROW_NUMBER() OVER (PARTITION BY sc.CategoryID ORDER BY COUNT(oi.OrderItemID) DESC, s.ServiceID) AS rn
    FROM ServiceCategories sc
    LEFT JOIN Services s ON sc.CategoryID = s.CategoryID
    LEFT JOIN OrderItems oi ON s.ServiceID = oi.ServiceID
    GROUP BY sc.CategoryID, sc.CategoryName, s.ServiceID, s.ServiceName
)
SELECT 
    CategoryName,
    ServiceName,
    ISNULL(UsageCount, 0) AS UsageCount
FROM ServiceUsage
WHERE rn = 1
ORDER BY UsageCount DESC;