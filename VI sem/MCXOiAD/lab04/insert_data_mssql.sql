-- ===================================================================
-- ПОЛНЫЙ СКРИПТ ПЕРЕСОЗДАНИЯ ДАННЫХ ДЛЯ MSSQL
-- Очищает всё, создаёт категории и заполняет таблицы правдоподобными данными
-- ===================================================================

-- Отключаем проверку внешних ключей временно (для надёжности)
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT ALL";
GO

-- Удаляем данные из всех таблиц в правильном порядке
DELETE FROM Timesheets;
DELETE FROM Payments;
DELETE FROM Invoices;
DELETE FROM OrderMaterials;
DELETE FROM OrderItems;
DELETE FROM Orders;
DELETE FROM Materials;
DELETE FROM Services;
DELETE FROM OrderStatuses;
DELETE FROM Employees;
DELETE FROM Customers;
DELETE FROM ServiceCategories;
GO

-- Включаем обратно проверку внешних ключей
EXEC sp_MSforeachtable "ALTER TABLE ? CHECK CONSTRAINT ALL";
GO

-- Сбрасываем идентификаторы (автоинкремент) для всех таблиц
DBCC CHECKIDENT ('ServiceCategories', RESEED, 0);
DBCC CHECKIDENT ('Services', RESEED, 0);
DBCC CHECKIDENT ('Customers', RESEED, 0);
DBCC CHECKIDENT ('Employees', RESEED, 0);
DBCC CHECKIDENT ('OrderStatuses', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('OrderItems', RESEED, 0);
DBCC CHECKIDENT ('Materials', RESEED, 0);
DBCC CHECKIDENT ('Invoices', RESEED, 0);
DBCC CHECKIDENT ('Payments', RESEED, 0);
DBCC CHECKIDENT ('Timesheets', RESEED, 0);
GO

-- 1. Создаём категории услуг (простые, без иерархии)
INSERT INTO ServiceCategories (CategoryName) VALUES (N'Ремонт');
INSERT INTO ServiceCategories (CategoryName) VALUES (N'Диагностика');
INSERT INTO ServiceCategories (CategoryName) VALUES (N'Софт');
GO

-- 2. Customers
INSERT INTO Customers (Name, ContactPerson, Phone, Email, Address, RegistrationDate)
VALUES
    (N'ООО "Ромашка"', N'Иванов Иван', '+7(495)123-45-67', 'info@romashka.ru', N'ул. Ленина, д.1', '2023-01-15'),
    (N'ИП Петров', N'Петров Петр', '+7(495)234-56-78', 'petrov@mail.ru', N'ул. Гагарина, д.5', '2023-02-20'),
    (N'ЗАО "Техсервис"', N'Сидоров Сидор', '+7(495)345-67-89', 'sidorov@tehservice.ru', N'пр. Мира, д.10', '2023-03-10'),
    (N'ООО "СтройИнвест"', N'Николаев Николай', '+7(495)456-78-90', 'nikolaev@stroyinvest.ru', N'ул. Строителей, д.15', '2023-04-05'),
    (N'АО "ТоргМаш"', N'Алексеев Алексей', '+7(495)567-89-01', 'alekseev@torgmash.ru', N'ул. Машиностроителей, д.20', '2023-05-12');
GO

-- 3. Employees
INSERT INTO Employees (FullName, Position, Phone, Email, HireDate)
VALUES
    (N'Смирнов Андрей', N'Менеджер', '+7(495)111-22-33', 'a.smirnov@company.ru', '2022-01-10'),
    (N'Кузнецова Елена', N'Бухгалтер', '+7(495)222-33-44', 'e.kuznetsova@company.ru', '2022-02-15'),
    (N'Попов Дмитрий', N'Мастер', '+7(495)333-44-55', 'd.popov@company.ru', '2022-03-20'),
    (N'Васильева Ольга', N'Мастер', '+7(495)444-55-66', 'o.vasilieva@company.ru', '2022-04-25'),
    (N'Михайлов Игорь', N'Техник', '+7(495)555-66-77', 'i.mihaylov@company.ru', '2022-05-30'),
    (N'Федорова Наталья', N'Менеджер', '+7(495)666-77-88', 'n.fedorova@company.ru', '2022-06-05');
GO

-- 4. OrderStatuses
INSERT INTO OrderStatuses (StatusName, Description)
VALUES
    (N'Новый', N'Заказ только создан'),
    (N'В работе', N'Заказ выполняется'),
    (N'Завершен', N'Заказ выполнен'),
    (N'Отменен', N'Заказ отменен клиентом'),
    (N'Ожидание', N'Ожидает поступления материалов');
GO

-- 5. Services (привязываем к категориям)
INSERT INTO Services (ServiceName, Description, DefaultPrice, Unit, CategoryID)
VALUES
    (N'Ремонт смартфона', N'Замена экрана, аккумулятора', 3000.00, N'шт', 
        (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Ремонт')),
    (N'Ремонт ноутбука', N'Замена клавиатуры, матрицы', 3500.00, N'шт', 
        (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Ремонт')),
    (N'Ремонт планшета', N'Замена стекла, аккумулятора', 2800.00, N'шт', 
        (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Ремонт')),
    (N'Диагностика компьютера', N'Полная проверка оборудования', 1500.00, N'шт', 
        (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Диагностика')),
    (N'Диагностика ноутбука', N'Выявление неисправностей', 1200.00, N'шт', 
        (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Диагностика')),
    (N'Установка Windows', N'Установка ОС с настройкой', 2000.00, N'шт', 
        (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Софт')),
    (N'Установка офисного ПО', N'Microsoft Office, антивирусы', 1000.00, N'шт', 
        (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Софт')),
    (N'Настройка ПО', N'Установка драйверов, настройка', 800.00, N'шт', 
        (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Софт'));
GO

-- 6. Materials
INSERT INTO Materials (MaterialName, Unit, CurrentStock)
VALUES
    (N'Экран для iPhone X', N'шт', 15),
    (N'Аккумулятор для ноутбука ASUS', N'шт', 8),
    (N'Термопаста Arctic', N'г', 200),
    (N'Винт M2x5', N'упак', 50),
    (N'Кабель SATA', N'шт', 30),
    (N'Мышь оптическая', N'шт', 20);
GO

-- 7. Базовые 6 заказов (июнь 2024)
INSERT INTO Orders (CustomerID, OrderDate, RequiredDate, StatusID, CreatedBy, Notes)
VALUES
    ((SELECT CustomerID FROM Customers WHERE Email = 'info@romashka.ru'), '2024-06-01', '2024-06-05',
        (SELECT StatusID FROM OrderStatuses WHERE StatusName = N'Новый'),
        (SELECT EmployeeID FROM Employees WHERE Email = 'a.smirnov@company.ru'), N'Срочный ремонт'),
    ((SELECT CustomerID FROM Customers WHERE Email = 'petrov@mail.ru'), '2024-06-02', '2024-06-07',
        (SELECT StatusID FROM OrderStatuses WHERE StatusName = N'В работе'),
        (SELECT EmployeeID FROM Employees WHERE Email = 'n.fedorova@company.ru'), N'Диагностика и ремонт'),
    ((SELECT CustomerID FROM Customers WHERE Email = 'sidorov@tehservice.ru'), '2024-06-03', '2024-06-10',
        (SELECT StatusID FROM OrderStatuses WHERE StatusName = N'Завершен'),
        (SELECT EmployeeID FROM Employees WHERE Email = 'a.smirnov@company.ru'), N'Установка ПО'),
    ((SELECT CustomerID FROM Customers WHERE Email = 'nikolaev@stroyinvest.ru'), '2024-06-04', '2024-06-12',
        (SELECT StatusID FROM OrderStatuses WHERE StatusName = N'Ожидание'),
        (SELECT EmployeeID FROM Employees WHERE Email = 'd.popov@company.ru'), N'Ремонт принтера'),
    ((SELECT CustomerID FROM Customers WHERE Email = 'alekseev@torgmash.ru'), '2024-06-05', '2024-06-09',
        (SELECT StatusID FROM OrderStatuses WHERE StatusName = N'Новый'),
        (SELECT EmployeeID FROM Employees WHERE Email = 'i.mihaylov@company.ru'), N'Замена батареи'),
    ((SELECT CustomerID FROM Customers WHERE Email = 'info@romashka.ru'), '2024-06-06', '2024-06-11',
        (SELECT StatusID FROM OrderStatuses WHERE StatusName = N'В работе'),
        (SELECT EmployeeID FROM Employees WHERE Email = 'o.vasilieva@company.ru'), N'Повторный ремонт');
GO

-- 8. Генерация 200 дополнительных заказов с датами 2024–2026
DECLARE @i INT = 1;
DECLARE @custID INT, @statusID INT, @empID INT, @orderDate DATE;

WHILE @i <= 200
BEGIN
    SELECT TOP 1 @custID = CustomerID FROM Customers ORDER BY NEWID();
    SELECT TOP 1 @statusID = StatusID FROM OrderStatuses ORDER BY NEWID();
    SELECT TOP 1 @empID = EmployeeID FROM Employees ORDER BY NEWID();
    SET @orderDate = DATEADD(day, ABS(CHECKSUM(NEWID())) % 1095, '2024-01-01'); -- от 2024-01-01 до 2026-12-31
    
    INSERT INTO Orders (CustomerID, OrderDate, RequiredDate, StatusID, CreatedBy, Notes)
    VALUES (
        @custID,
        @orderDate,
        DATEADD(day, 3 + ABS(CHECKSUM(NEWID())) % 12, @orderDate),
        @statusID,
        @empID,
        'BULK_ORDER_' + CAST(@i AS VARCHAR(10))
    );
    
    SET @i = @i + 1;
END;
GO

-- 9. Вставка OrderItems с распределением по весам (50% ремонт, 30% диагностика, 20% софт)
INSERT INTO OrderItems (OrderID, ServiceID, Quantity, UnitPrice, Discount)
SELECT 
    o.OrderID,
    ca.ServiceID,
    1 + ABS(CHECKSUM(NEWID(), o.OrderID)) % 3 AS Quantity,
    ca.DefaultPrice,
    CASE ABS(CHECKSUM(NEWID(), o.OrderID)) % 4 
        WHEN 0 THEN 0 
        WHEN 1 THEN 5 
        WHEN 2 THEN 10 
        ELSE 15 
    END AS Discount
FROM Orders o
CROSS JOIN (SELECT 1 AS n UNION SELECT 2 UNION SELECT 3) nums
CROSS APPLY (
    SELECT TOP 1 ServiceID, DefaultPrice
    FROM Services
    WHERE CategoryID = 
        CASE ABS(CHECKSUM(NEWID(), o.OrderID, nums.n)) % 10
            WHEN 0 THEN (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Ремонт')
            WHEN 1 THEN (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Ремонт')
            WHEN 2 THEN (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Ремонт')
            WHEN 3 THEN (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Ремонт')
            WHEN 4 THEN (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Ремонт')
            WHEN 5 THEN (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Диагностика')
            WHEN 6 THEN (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Диагностика')
            WHEN 7 THEN (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Диагностика')
            WHEN 8 THEN (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Софт')
            WHEN 9 THEN (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = N'Софт')
        END
    ORDER BY NEWID()
) ca;
GO

-- 10. Вставка OrderMaterials (уникальные пары)
CREATE TABLE #OrderMaterialsTemp (
    OrderID INT,
    MaterialID INT,
    QuantityUsed DECIMAL(10,2),
    UnitCost DECIMAL(10,2)
);

WITH OrderRand AS (
    SELECT OrderID, ROW_NUMBER() OVER (ORDER BY NEWID()) AS rn
    FROM Orders
),
MaterialRand AS (
    SELECT MaterialID, ROW_NUMBER() OVER (ORDER BY NEWID()) AS rn
    FROM Materials
),
Pairs AS (
    SELECT 
        o.OrderID,
        m.MaterialID,
        ROW_NUMBER() OVER (ORDER BY NEWID()) AS rn
    FROM OrderRand o
    CROSS JOIN MaterialRand m
)
INSERT INTO #OrderMaterialsTemp (OrderID, MaterialID, QuantityUsed, UnitCost)
SELECT TOP 250
    OrderID,
    MaterialID,
    1 + ABS(CHECKSUM(NEWID())) % 5,
    50 + ABS(CHECKSUM(NEWID())) % 1950
FROM Pairs
ORDER BY rn;

INSERT INTO OrderMaterials (OrderID, MaterialID, QuantityUsed, UnitCost)
SELECT OrderID, MaterialID, QuantityUsed, UnitCost
FROM #OrderMaterialsTemp;

DROP TABLE #OrderMaterialsTemp;
GO

-- 11. Вставка Invoices для всех заказов
INSERT INTO Invoices (OrderID, InvoiceDate, DueDate, TotalAmount, PaidAmount)
SELECT 
    o.OrderID,
    o.OrderDate AS InvoiceDate,
    DATEADD(day, 14, o.OrderDate) AS DueDate,
    ISNULL((SELECT SUM(Amount) FROM OrderItems WHERE OrderID = o.OrderID), 0) AS TotalAmount,
    CASE ABS(CHECKSUM(NEWID())) % 3
        WHEN 0 THEN 0
        WHEN 1 THEN ISNULL((SELECT SUM(Amount) FROM OrderItems WHERE OrderID = o.OrderID), 0) / 2
        ELSE ISNULL((SELECT SUM(Amount) FROM OrderItems WHERE OrderID = o.OrderID), 0)
    END AS PaidAmount
FROM Orders o;
GO

-- 12. Вставка Payments
CREATE TABLE #PaymentsTemp (
    InvoiceID INT,
    PaymentDate DATE,
    Amount DECIMAL(12,2),
    PaymentMethod NVARCHAR(50)
);

WITH InvoicesWithPaid AS (
    SELECT InvoiceID, PaidAmount, InvoiceDate
    FROM Invoices
    WHERE PaidAmount > 0
),
Numbers AS (
    SELECT 1 AS n UNION SELECT 2
)
INSERT INTO #PaymentsTemp (InvoiceID, PaymentDate, Amount, PaymentMethod)
SELECT 
    i.InvoiceID,
    DATEADD(day, ABS(CHECKSUM(NEWID())) % 10, i.InvoiceDate) AS PaymentDate,
    i.PaidAmount / (SELECT COUNT(*) FROM Numbers n2 WHERE n2.n <= n.n) AS Amount,
    CHOOSE(1 + ABS(CHECKSUM(NEWID())) % 4, N'Наличные', N'Банковская карта', N'Безналичный расчет', N'Электронные деньги')
FROM InvoicesWithPaid i
CROSS JOIN Numbers n
WHERE n.n <= 1 + ABS(CHECKSUM(NEWID())) % 2;

INSERT INTO Payments (InvoiceID, PaymentDate, Amount, PaymentMethod)
SELECT InvoiceID, PaymentDate, Amount, PaymentMethod
FROM #PaymentsTemp
WHERE Amount > 0;

DROP TABLE #PaymentsTemp;
GO

-- 13. Вставка Timesheets
INSERT INTO Timesheets (OrderItemID, EmployeeID, WorkDate, HoursWorked)
SELECT TOP 350
    oi.OrderItemID,
    (SELECT TOP 1 EmployeeID FROM Employees ORDER BY NEWID()),
    DATEADD(day, ABS(CHECKSUM(NEWID())) % 30, o.OrderDate) AS WorkDate,
    1 + ABS(CHECKSUM(NEWID())) % 8 AS HoursWorked
FROM OrderItems oi
JOIN Orders o ON oi.OrderID = o.OrderID
CROSS JOIN (SELECT 1 AS n UNION SELECT 2) t
ORDER BY NEWID();
GO

-- 14. Обновление TotalAmount в Orders (на случай, если триггер не сработал)
UPDATE Orders
SET TotalAmount = (SELECT ISNULL(SUM(Amount), 0) FROM OrderItems WHERE OrderID = Orders.OrderID);
GO

-- 15. Проверка количества записей
SELECT 'Orders' AS TableName, COUNT(*) AS Rows FROM Orders
UNION ALL
SELECT 'OrderItems', COUNT(*) FROM OrderItems
UNION ALL
SELECT 'OrderMaterials', COUNT(*) FROM OrderMaterials
UNION ALL
SELECT 'Invoices', COUNT(*) FROM Invoices
UNION ALL
SELECT 'Payments', COUNT(*) FROM Payments
UNION ALL
SELECT 'Timesheets', COUNT(*) FROM Timesheets
ORDER BY TableName;
GO