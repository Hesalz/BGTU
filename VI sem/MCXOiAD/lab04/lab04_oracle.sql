-- Задание 2
-- 2.1 Помесячно
SELECT 
    EXTRACT(YEAR FROM o.OrderDate) AS Year,
    EXTRACT(MONTH FROM o.OrderDate) AS Month,
    COUNT(DISTINCT o.OrderID) AS OrdersCount,
    NVL(SUM(oi.Quantity), 0) AS TotalQuantity,
    NVL(SUM(oi.Amount), 0) AS TotalAmount
FROM Orders o
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID 
    AND oi.ServiceID = (SELECT ServiceID FROM Services WHERE ServiceName = 'Ремонт смартфона')
GROUP BY EXTRACT(YEAR FROM o.OrderDate), EXTRACT(MONTH FROM o.OrderDate)
ORDER BY Year, Month;

-- 2.2 Поквартально
SELECT 
    EXTRACT(YEAR FROM o.OrderDate) AS Year,
    TO_CHAR(o.OrderDate, 'Q') AS Quarter,
    COUNT(DISTINCT o.OrderID) AS OrdersCount,
    NVL(SUM(oi.Quantity), 0) AS TotalQuantity,
    NVL(SUM(oi.Amount), 0) AS TotalAmount
FROM Orders o
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID 
    AND oi.ServiceID = (SELECT ServiceID FROM Services WHERE ServiceName = 'Ремонт смартфона')
GROUP BY EXTRACT(YEAR FROM o.OrderDate), TO_CHAR(o.OrderDate, 'Q')
ORDER BY Year, Quarter;

-- 2.3 По полугодиям
SELECT 
    EXTRACT(YEAR FROM o.OrderDate) AS Year,
    CASE WHEN EXTRACT(MONTH FROM o.OrderDate) <= 6 THEN 1 ELSE 2 END AS HalfYear,
    COUNT(DISTINCT o.OrderID) AS OrdersCount,
    NVL(SUM(oi.Quantity), 0) AS TotalQuantity,
    NVL(SUM(oi.Amount), 0) AS TotalAmount
FROM Orders o
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID 
    AND oi.ServiceID = (SELECT ServiceID FROM Services WHERE ServiceName = 'Ремонт смартфона')
GROUP BY EXTRACT(YEAR FROM o.OrderDate), 
         CASE WHEN EXTRACT(MONTH FROM o.OrderDate) <= 6 THEN 1 ELSE 2 END
ORDER BY Year, HalfYear;

-- 2.4 По годам
SELECT 
    EXTRACT(YEAR FROM o.OrderDate) AS Year,
    COUNT(DISTINCT o.OrderID) AS OrdersCount,
    NVL(SUM(oi.Quantity), 0) AS TotalQuantity,
    NVL(SUM(oi.Amount), 0) AS TotalAmount
FROM Orders o
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID 
    AND oi.ServiceID = (SELECT ServiceID FROM Services WHERE ServiceName = 'Ремонт смартфона')
GROUP BY EXTRACT(YEAR FROM o.OrderDate)
ORDER BY Year;


-- Задание 3
WITH
service_totals AS (
    SELECT 
        oi.ServiceID,
        SUM(oi.Quantity) AS Qty,
        SUM(oi.Amount) AS Amt
    FROM Orders o
    JOIN OrderItems oi ON o.OrderID = oi.OrderID
    WHERE o.OrderDate BETWEEN DATE '2024-01-01' AND DATE '2026-12-31'
    GROUP BY oi.ServiceID
),
service_data AS (
    SELECT 
        NVL(SUM(oi.Quantity), 0) AS ServiceQuantity,
        NVL(SUM(oi.Amount), 0) AS ServiceAmount
    FROM Orders o
    LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID 
        AND oi.ServiceID = (SELECT ServiceID FROM Services WHERE ServiceName = 'Ремонт смартфона')
    WHERE o.OrderDate BETWEEN DATE '2024-01-01' AND DATE '2026-12-31'
),
all_data AS (
    SELECT 
        NVL(SUM(Qty), 0) AS TotalQuantity,
        NVL(MAX(Amt), 0) AS MaxAmount
    FROM service_totals
)
SELECT 
    s.ServiceQuantity AS "Объём услуг",
    s.ServiceAmount AS "Сумма",
    CASE 
        WHEN a.TotalQuantity > 0 
        THEN ROUND(s.ServiceQuantity * 100 / a.TotalQuantity, 2)
        ELSE 0 
    END AS "% от общего объёма",
    CASE 
        WHEN a.MaxAmount > 0 
        THEN ROUND(s.ServiceAmount * 100 / a.MaxAmount, 2)
        ELSE 0 
    END AS "% от макс. суммы"
FROM service_data s
CROSS JOIN all_data a;


-- Задание 4
WITH OrderedOrders AS (
    SELECT 
        o.OrderID,
        c.Name AS CustomerName,
        o.OrderDate,
        o.TotalAmount,
        ROW_NUMBER() OVER (ORDER BY o.OrderDate DESC, o.OrderID) AS rn
    FROM Orders o
    JOIN Customers c ON o.CustomerID = c.CustomerID
)
SELECT 
    OrderID,
    CustomerName,
    OrderDate,
    TotalAmount,
    rn
FROM OrderedOrders
WHERE rn BETWEEN 1 AND 20
ORDER BY rn;


-- Задание 5
-- 5.0 Добавляем дублирующую запись для услуги 'Ремонт смартфона'
INSERT INTO Services (ServiceName, Description, DefaultPrice, Unit, CategoryID)
SELECT ServiceName, Description, DefaultPrice, Unit, CategoryID
FROM Services
WHERE ServiceName = 'Ремонт смартфона';
COMMIT;

-- 5.1 Показываем дубликаты (до удаления)
SELECT ServiceName, COUNT(*) AS DuplicateCount
FROM Services
GROUP BY ServiceName
HAVING COUNT(*) > 1
ORDER BY DuplicateCount DESC;

-- 5.2 Удаляем дубликаты, оставляя одну запись с наименьшим ServiceID
DELETE FROM Services
WHERE ServiceID IN (
    SELECT ServiceID
    FROM (
        SELECT 
            ServiceID,
            ROW_NUMBER() OVER (PARTITION BY ServiceName ORDER BY ServiceID) AS rn
        FROM Services
    )
    WHERE rn > 1
);
COMMIT;

-- 5.3 Проверяем, что дубликатов больше нет
SELECT ServiceName, COUNT(*) AS DuplicateCount
FROM Services
GROUP BY ServiceName
HAVING COUNT(*) > 1
ORDER BY DuplicateCount DESC;


-- Задание 6
WITH Months AS (
    SELECT 
        ADD_MONTHS(TRUNC(SYSDATE, 'MM'), -LEVEL + 1) AS MonthStart,
        LAST_DAY(ADD_MONTHS(TRUNC(SYSDATE, 'MM'), -LEVEL + 1)) AS MonthEnd
    FROM DUAL
    CONNECT BY LEVEL <= 6
)
SELECT 
    c.CustomerID,
    c.Name AS CustomerName,
    EXTRACT(YEAR FROM m.MonthStart) AS Year,
    EXTRACT(MONTH FROM m.MonthStart) AS Month,
    TO_CHAR(m.MonthStart, 'YYYY-MM') AS MonthName,
    COUNT(DISTINCT oi.OrderItemID) AS ServicesCount
FROM Months m
CROSS JOIN Customers c
LEFT JOIN Orders o ON c.CustomerID = o.CustomerID 
    AND o.OrderDate BETWEEN m.MonthStart AND m.MonthEnd
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID
GROUP BY c.CustomerID, c.Name, EXTRACT(YEAR FROM m.MonthStart), 
         EXTRACT(MONTH FROM m.MonthStart), TO_CHAR(m.MonthStart, 'YYYY-MM')
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
    NVL(UsageCount, 0) AS UsageCount
FROM ServiceUsage
WHERE rn = 1
ORDER BY UsageCount DESC;