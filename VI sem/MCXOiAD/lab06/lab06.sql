PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Customers (
    CustomerID INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    ContactPerson TEXT,
    Phone TEXT,
    Email TEXT UNIQUE,
    Address TEXT,
    RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Employees (
    EmployeeID INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT NOT NULL,
    Position TEXT,
    Phone TEXT,
    Email TEXT UNIQUE,
    HireDate DATE NOT NULL
);

CREATE TABLE IF NOT EXISTS ServiceCategories (
    CategoryID INTEGER PRIMARY KEY AUTOINCREMENT,
    CategoryName TEXT NOT NULL,
    Description TEXT
);

CREATE TABLE IF NOT EXISTS Services (
    ServiceID INTEGER PRIMARY KEY AUTOINCREMENT,
    ServiceName TEXT NOT NULL,
    Description TEXT,
    DefaultPrice REAL NOT NULL,
    Unit TEXT,
    CategoryID INTEGER,
    FOREIGN KEY (CategoryID) REFERENCES ServiceCategories(CategoryID) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS OrderStatuses (
    StatusID INTEGER PRIMARY KEY AUTOINCREMENT,
    StatusName TEXT NOT NULL,
    Description TEXT
);

CREATE TABLE IF NOT EXISTS Orders (
    OrderID INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerID INTEGER NOT NULL,
    OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    RequiredDate DATE,
    StatusID INTEGER NOT NULL,
    TotalAmount REAL,
    Notes TEXT,
    CreatedBy INTEGER,
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE CASCADE,
    FOREIGN KEY (StatusID) REFERENCES OrderStatuses(StatusID),
    FOREIGN KEY (CreatedBy) REFERENCES Employees(EmployeeID) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS OrderItems (
    OrderItemID INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderID INTEGER NOT NULL,
    ServiceID INTEGER NOT NULL,
    Quantity REAL NOT NULL,
    UnitPrice REAL NOT NULL,
    Discount REAL DEFAULT 0,
    Amount REAL DEFAULT 0,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID)
);

CREATE TABLE IF NOT EXISTS Materials (
    MaterialID INTEGER PRIMARY KEY AUTOINCREMENT,
    MaterialName TEXT NOT NULL,
    Unit TEXT,
    CurrentStock REAL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS OrderMaterials (
    OrderID INTEGER NOT NULL,
    MaterialID INTEGER NOT NULL,
    QuantityUsed REAL NOT NULL,
    UnitCost REAL,
    PRIMARY KEY (OrderID, MaterialID),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    FOREIGN KEY (MaterialID) REFERENCES Materials(MaterialID)
);

CREATE TABLE IF NOT EXISTS Invoices (
    InvoiceID INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderID INTEGER NOT NULL,
    InvoiceDate DATE DEFAULT CURRENT_DATE,
    DueDate DATE,
    TotalAmount REAL NOT NULL,
    PaidAmount REAL DEFAULT 0,
    Status TEXT DEFAULT 'Unpaid',
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    CHECK (Status IN ('Unpaid', 'Partial', 'Paid'))
);

CREATE TABLE IF NOT EXISTS Payments (
    PaymentID INTEGER PRIMARY KEY AUTOINCREMENT,
    InvoiceID INTEGER NOT NULL,
    PaymentDate DATE DEFAULT CURRENT_DATE,
    Amount REAL NOT NULL,
    PaymentMethod TEXT,
    FOREIGN KEY (InvoiceID) REFERENCES Invoices(InvoiceID) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Timesheets (
    TimesheetID INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderItemID INTEGER NOT NULL,
    EmployeeID INTEGER NOT NULL,
    WorkDate DATE NOT NULL,
    HoursWorked REAL NOT NULL,
    FOREIGN KEY (OrderItemID) REFERENCES OrderItems(OrderItemID) ON DELETE CASCADE,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

INSERT OR IGNORE INTO OrderStatuses (StatusID, StatusName, Description) VALUES
(1, 'Новый', 'Только что созданный заказ'),
(2, 'В работе', 'Заказ выполняется'),
(3, 'Завершен', 'Заказ выполнен'),
(4, 'Отменен', 'Заказ отменен');

INSERT OR IGNORE INTO ServiceCategories (CategoryID, CategoryName, Description) VALUES
(1, 'Ремонт', 'Ремонтные работы'),
(2, 'Диагностика', 'Диагностические услуги'),
(3, 'Обслуживание', 'Регулярное обслуживание');

INSERT OR IGNORE INTO Services (ServiceID, ServiceName, DefaultPrice, Unit, CategoryID) VALUES
(1, 'Ремонт компьютера', 1500, 'шт', 1),
(2, 'Замена комплектующих', 800, 'услуга', 1),
(3, 'Диагностика ноутбука', 1000, 'шт', 2),
(4, 'Профилактическая чистка', 1200, 'услуга', 3);

INSERT OR IGNORE INTO Customers (CustomerID, Name, ContactPerson, Phone, Email, Address) VALUES
(1, 'ООО "ТехноСервис"', 'Иванов Иван', '+7 (495) 123-45-67', 'info@technoservice.ru', 'г. Москва, ул. Ленина, д. 10'),
(2, 'ИП Петров', 'Петров Петр', '+7 (916) 987-65-43', 'petrov@mail.ru', 'г. Москва, ул. Садовая, д. 5');

INSERT OR IGNORE INTO Employees (EmployeeID, FullName, Position, Phone, Email, HireDate) VALUES
(1, 'Алексеев Алексей', 'Мастер', '+7 (903) 111-22-33', 'alexeev@company.ru', '2023-01-15'),
(2, 'Борисов Борис', 'Инженер', '+7 (903) 222-33-44', 'borisov@company.ru', '2023-02-20');

INSERT OR IGNORE INTO Materials (MaterialID, MaterialName, Unit, CurrentStock) VALUES
(1, 'Термопаста', 'тюбик', 10),
(2, 'Винт M3', 'шт', 500),
(3, 'Кабель SATA', 'шт', 30),
(4, 'Блок питания 500W', 'шт', 5);

-- Единый триггер для вставки
CREATE TRIGGER IF NOT EXISTS trg_OrderItems_AfterInsert
AFTER INSERT ON OrderItems
BEGIN
    UPDATE OrderItems 
    SET Amount = NEW.Quantity * NEW.UnitPrice * (1 - NEW.Discount/100)
    WHERE OrderItemID = NEW.OrderItemID;
    
    UPDATE Orders
    SET TotalAmount = (
        SELECT COALESCE(SUM(Quantity * UnitPrice * (1 - Discount/100)), 0)
        FROM OrderItems
        WHERE OrderID = NEW.OrderID
    )
    WHERE OrderID = NEW.OrderID;
END;

-- Единый триггер для обновления
CREATE TRIGGER IF NOT EXISTS trg_OrderItems_AfterUpdate
AFTER UPDATE OF Quantity, UnitPrice, Discount ON OrderItems
BEGIN
    UPDATE OrderItems 
    SET Amount = NEW.Quantity * NEW.UnitPrice * (1 - NEW.Discount/100)
    WHERE OrderItemID = NEW.OrderItemID;
    
    UPDATE Orders
    SET TotalAmount = (
        SELECT COALESCE(SUM(Quantity * UnitPrice * (1 - Discount/100)), 0)
        FROM OrderItems
        WHERE OrderID = NEW.OrderID
    )
    WHERE OrderID = NEW.OrderID;
END;

-- Триггер для удаления
CREATE TRIGGER IF NOT EXISTS trg_OrderItems_AfterDelete
AFTER DELETE ON OrderItems
BEGIN
    UPDATE Orders
    SET TotalAmount = (
        SELECT COALESCE(SUM(Quantity * UnitPrice * (1 - Discount/100)), 0)
        FROM OrderItems
        WHERE OrderID = OLD.OrderID
    )
    WHERE OrderID = OLD.OrderID;
END;

-- Триггер для проверки дат
CREATE TRIGGER IF NOT EXISTS trg_Orders_CheckDates
BEFORE INSERT ON Orders
BEGIN
    SELECT CASE
        WHEN NEW.RequiredDate < NEW.OrderDate THEN
            RAISE(ABORT, 'Required date cannot be earlier than order date')
    END;
END;

-- Триггер для обновления статуса счета при платеже
CREATE TRIGGER IF NOT EXISTS trg_Payments_UpdateInvoice
AFTER INSERT ON Payments
BEGIN
    UPDATE Invoices
    SET PaidAmount = PaidAmount + NEW.Amount,
        Status = CASE 
            WHEN PaidAmount + NEW.Amount >= TotalAmount THEN 'Paid'
            WHEN PaidAmount + NEW.Amount > 0 THEN 'Partial'
            ELSE 'Unpaid'
        END
    WHERE InvoiceID = NEW.InvoiceID;
END;

-- Представление активных заказов
CREATE VIEW IF NOT EXISTS vw_ActiveOrders AS
SELECT 
    o.OrderID,
    c.Name AS CustomerName,
    o.OrderDate,
    o.RequiredDate,
    os.StatusName,
    o.TotalAmount,
    julianday(o.RequiredDate) - julianday(o.OrderDate) AS DaysToComplete
FROM Orders o
JOIN Customers c ON o.CustomerID = c.CustomerID
JOIN OrderStatuses os ON o.StatusID = os.StatusID
WHERE os.StatusName NOT IN ('Завершен', 'Отменен');

-- Представление детализации заказов
CREATE VIEW IF NOT EXISTS vw_OrderDetails AS
SELECT 
    o.OrderID,
    c.Name AS CustomerName,
    s.ServiceName,
    oi.Quantity,
    oi.UnitPrice,
    oi.Discount,
    oi.Amount,
    os.StatusName
FROM Orders o
JOIN Customers c ON o.CustomerID = c.CustomerID
JOIN OrderItems oi ON o.OrderID = oi.OrderID
JOIN Services s ON oi.ServiceID = s.ServiceID
JOIN OrderStatuses os ON o.StatusID = os.StatusID;


CREATE INDEX IF NOT EXISTS idx_Orders_CustomerID ON Orders(CustomerID);
CREATE INDEX IF NOT EXISTS idx_Orders_StatusID ON Orders(StatusID);
CREATE INDEX IF NOT EXISTS idx_OrderItems_OrderID ON OrderItems(OrderID);
CREATE INDEX IF NOT EXISTS idx_OrderItems_ServiceID ON OrderItems(ServiceID);
CREATE INDEX IF NOT EXISTS idx_Invoices_OrderID ON Invoices(OrderID);
CREATE INDEX IF NOT EXISTS idx_Payments_InvoiceID ON Payments(InvoiceID);
CREATE INDEX IF NOT EXISTS idx_Timesheets_EmployeeID ON Timesheets(EmployeeID);
CREATE INDEX IF NOT EXISTS idx_Timesheets_OrderItemID ON Timesheets(OrderItemID);




BEGIN TRANSACTION;
INSERT INTO Orders (CustomerID, RequiredDate, StatusID, CreatedBy, Notes)
VALUES (1, DATE('now', '+7 days'), 1, 1, 'Новый заказ из SQLite');
INSERT INTO OrderItems (OrderID, ServiceID, Quantity, UnitPrice, Discount)
VALUES (last_insert_rowid(), 1, 1, 1500, 0);
SELECT '=== Демо 1: TotalAmount после добавления ===' AS '';
SELECT OrderID, TotalAmount FROM Orders WHERE OrderID = last_insert_rowid();
COMMIT;

BEGIN TRANSACTION;
UPDATE OrderItems 
SET Quantity = 2, Discount = 10
WHERE OrderID = (SELECT MAX(OrderID) FROM Orders);
SELECT '=== Демо 2: Позиция после обновления ===' AS '';
SELECT OrderItemID, Quantity, UnitPrice, Discount, Amount
FROM OrderItems 
WHERE OrderID = (SELECT MAX(OrderID) FROM Orders);
COMMIT;

BEGIN TRANSACTION;
DELETE FROM OrderItems 
WHERE OrderID = (SELECT MAX(OrderID) FROM Orders);
SELECT '=== Демо 3: TotalAmount после удаления ===' AS '';
SELECT OrderID, TotalAmount FROM Orders WHERE OrderID = (SELECT MAX(OrderID) FROM Orders);
COMMIT;
DELETE FROM Orders WHERE Notes = 'Новый заказ из SQLite';

SELECT '=== Демо 4: Проверка внешнего ключа ===' AS '';
INSERT INTO OrderItems (OrderID, ServiceID, Quantity, UnitPrice) VALUES (1, 999, 1, 100);



SELECT '=== Активные заказы (vw_ActiveOrders) ===' AS '';
SELECT * FROM vw_ActiveOrders;

SELECT '=== Детали заказов (vw_OrderDetails) ===' AS '';
SELECT * FROM vw_OrderDetails;

SELECT '=== Индексы ===' AS '';
SELECT name FROM sqlite_master WHERE type='index' ORDER BY name;

SELECT '=== Триггеры ===' AS '';
SELECT name FROM sqlite_master WHERE type='trigger' ORDER BY name;


INSERT INTO Orders (CustomerID, RequiredDate, StatusID, CreatedBy, Notes)
VALUES (1, DATE('now', '+5 days'), 1, 1, 'Тестовый заказ для проверки триггеров');

INSERT INTO OrderItems (OrderID, ServiceID, Quantity, UnitPrice, Discount)
VALUES (last_insert_rowid(), 2, 1, 800, 0);

SELECT 'Amount рассчитан автоматически:' AS '';
SELECT OrderItemID, Quantity, UnitPrice, Discount, Amount FROM OrderItems WHERE OrderID = (SELECT MAX(OrderID) FROM Orders);

SELECT 'TotalAmount рассчитан автоматически:' AS '';
SELECT OrderID, TotalAmount FROM Orders WHERE OrderID = (SELECT MAX(OrderID) FROM Orders);