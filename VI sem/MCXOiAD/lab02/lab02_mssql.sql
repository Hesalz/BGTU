CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    ContactPerson NVARCHAR(100),
    Phone NVARCHAR(20),
    Email NVARCHAR(100) UNIQUE,
    Address NVARCHAR(255),
    RegistrationDate DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE Employees (
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100),
    Phone NVARCHAR(20),
    Email NVARCHAR(100) UNIQUE,
    HireDate DATE NOT NULL
);

CREATE TABLE ServiceCategories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255)
);

CREATE TABLE Services (
    ServiceID INT IDENTITY(1,1) PRIMARY KEY,
    ServiceName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    DefaultPrice DECIMAL(10,2) NOT NULL,
    Unit NVARCHAR(50),
    CategoryID INT FOREIGN KEY REFERENCES ServiceCategories(CategoryID)
);

CREATE TABLE OrderStatuses (
    StatusID INT IDENTITY(1,1) PRIMARY KEY,
    StatusName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255)
);

CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL FOREIGN KEY REFERENCES Customers(CustomerID),
    OrderDate DATETIME2 DEFAULT GETDATE(),
    RequiredDate DATE,
    StatusID INT NOT NULL FOREIGN KEY REFERENCES OrderStatuses(StatusID),
    TotalAmount DECIMAL(12,2),
    Notes NVARCHAR(MAX),
    CreatedBy INT FOREIGN KEY REFERENCES Employees(EmployeeID)
);

CREATE TABLE OrderItems (
    OrderItemID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL FOREIGN KEY REFERENCES Orders(OrderID),
    ServiceID INT NOT NULL FOREIGN KEY REFERENCES Services(ServiceID),
    Quantity DECIMAL(10,2) NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    Discount DECIMAL(5,2) DEFAULT 0,
    Amount AS (Quantity * UnitPrice * (1 - Discount/100)) PERSISTED
);

CREATE TABLE Materials (
    MaterialID INT IDENTITY(1,1) PRIMARY KEY,
    MaterialName NVARCHAR(200) NOT NULL,
    Unit NVARCHAR(50),
    CurrentStock DECIMAL(10,2)
);

CREATE TABLE OrderMaterials (
    OrderID INT NOT NULL FOREIGN KEY REFERENCES Orders(OrderID),
    MaterialID INT NOT NULL FOREIGN KEY REFERENCES Materials(MaterialID),
    QuantityUsed DECIMAL(10,2) NOT NULL,
    UnitCost DECIMAL(10,2),
    PRIMARY KEY (OrderID, MaterialID)
);

CREATE TABLE Invoices (
    InvoiceID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL FOREIGN KEY REFERENCES Orders(OrderID),
    InvoiceDate DATE DEFAULT GETDATE(),
    DueDate DATE,
    TotalAmount DECIMAL(12,2) NOT NULL,
    PaidAmount DECIMAL(12,2) DEFAULT 0,
    Status NVARCHAR(20) DEFAULT 'Unpaid' CHECK (Status IN ('Unpaid', 'Partial', 'Paid'))
);

CREATE TABLE Payments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceID INT NOT NULL FOREIGN KEY REFERENCES Invoices(InvoiceID),
    PaymentDate DATE DEFAULT GETDATE(),
    Amount DECIMAL(12,2) NOT NULL,
    PaymentMethod NVARCHAR(50)
);

CREATE TABLE Timesheets (
    TimesheetID INT IDENTITY(1,1) PRIMARY KEY,
    OrderItemID INT NOT NULL FOREIGN KEY REFERENCES OrderItems(OrderItemID),
    EmployeeID INT NOT NULL FOREIGN KEY REFERENCES Employees(EmployeeID),
    WorkDate DATE NOT NULL,
    HoursWorked DECIMAL(5,2) NOT NULL
);

CREATE INDEX IX_Orders_CustomerID ON Orders(CustomerID);
CREATE INDEX IX_Orders_StatusID ON Orders(StatusID);
CREATE INDEX IX_OrderItems_OrderID ON OrderItems(OrderID);
CREATE INDEX IX_OrderItems_ServiceID ON OrderItems(ServiceID);
CREATE INDEX IX_Invoices_OrderID ON Invoices(OrderID);
CREATE INDEX IX_Payments_InvoiceID ON Payments(InvoiceID);
CREATE INDEX IX_Timesheets_EmployeeID ON Timesheets(EmployeeID);
CREATE INDEX IX_Timesheets_OrderItemID ON Timesheets(OrderItemID);

CREATE VIEW vw_ActiveOrders
AS
SELECT 
    o.OrderID,
    c.Name AS CustomerName,
    o.OrderDate,
    o.RequiredDate,
    os.StatusName,
    o.TotalAmount,
    DATEDIFF(DAY, o.OrderDate, o.RequiredDate) AS DaysToComplete
FROM Orders o
JOIN Customers c ON o.CustomerID = c.CustomerID
JOIN OrderStatuses os ON o.StatusID = os.StatusID
WHERE os.StatusName != 'Completed' 
  AND os.StatusName != 'Cancelled';
GO

-- Функция расчета общей суммы заказа
CREATE FUNCTION fn_CalculateOrderTotal(@OrderID INT)
RETURNS DECIMAL(12,2)
AS
BEGIN
    DECLARE @Total DECIMAL(12,2);
    
    SELECT @Total = SUM(Amount)
    FROM OrderItems
    WHERE OrderID = @OrderID;
    
    RETURN ISNULL(@Total, 0);
END;
GO

-- Функция проверки статуса оплаты счета
CREATE FUNCTION fn_GetInvoiceStatus(@InvoiceID INT)
RETURNS NVARCHAR(20)
AS
BEGIN
    DECLARE @Status NVARCHAR(20);
    DECLARE @Total DECIMAL(12,2);
    DECLARE @Paid DECIMAL(12,2);
    
    SELECT @Total = TotalAmount, @Paid = PaidAmount
    FROM Invoices
    WHERE InvoiceID = @InvoiceID;
    
    IF @Paid >= @Total
        SET @Status = 'Paid';
    ELSE IF @Paid > 0
        SET @Status = 'Partial';
    ELSE
        SET @Status = 'Unpaid';
        
    RETURN @Status;
END;
GO

-- Процедура создания нового заказа
CREATE PROCEDURE sp_CreateOrder
    @CustomerID INT,
    @RequiredDate DATE,
    @StatusID INT,
    @CreatedBy INT,
    @Notes NVARCHAR(MAX) = NULL,
    @OrderID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO Orders (CustomerID, RequiredDate, StatusID, CreatedBy, Notes)
        VALUES (@CustomerID, @RequiredDate, @StatusID, @CreatedBy, @Notes);
        
        SET @OrderID = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Процедура добавления услуги в заказ
CREATE PROCEDURE sp_AddOrderItem
    @OrderID INT,
    @ServiceID INT,
    @Quantity DECIMAL(10,2),
    @UnitPrice DECIMAL(10,2) = NULL,
    @Discount DECIMAL(5,2) = 0
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        IF @UnitPrice IS NULL
        BEGIN
            SELECT @UnitPrice = DefaultPrice
            FROM Services
            WHERE ServiceID = @ServiceID;
        END
        
        INSERT INTO OrderItems (OrderID, ServiceID, Quantity, UnitPrice, Discount)
        VALUES (@OrderID, @ServiceID, @Quantity, @UnitPrice, @Discount);
        
        UPDATE Orders
        SET TotalAmount = dbo.fn_CalculateOrderTotal(@OrderID)
        WHERE OrderID = @OrderID;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Процедура регистрации платежа
CREATE PROCEDURE sp_RegisterPayment
    @InvoiceID INT,
    @Amount DECIMAL(12,2),
    @PaymentMethod NVARCHAR(50),
    @PaymentID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @Remaining DECIMAL(12,2);
        SELECT @Remaining = TotalAmount - PaidAmount
        FROM Invoices
        WHERE InvoiceID = @InvoiceID;
        
        IF @Amount > @Remaining
            THROW 50001, 'Payment amount exceeds remaining balance', 1;
        
        INSERT INTO Payments (InvoiceID, Amount, PaymentMethod)
        VALUES (@InvoiceID, @Amount, @PaymentMethod);
        
        SET @PaymentID = SCOPE_IDENTITY();
        
        UPDATE Invoices
        SET 
            PaidAmount = PaidAmount + @Amount,
            Status = dbo.fn_GetInvoiceStatus(InvoiceID)
        WHERE InvoiceID = @InvoiceID;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Процедура списания материалов
CREATE PROCEDURE sp_ConsumeMaterials
    @OrderID INT,
    @MaterialID INT,
    @QuantityUsed DECIMAL(10,2),
    @UnitCost DECIMAL(10,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @CurrentStock DECIMAL(10,2);
        SELECT @CurrentStock = CurrentStock
        FROM Materials
        WHERE MaterialID = @MaterialID;
        
        IF @CurrentStock < @QuantityUsed
            THROW 50002, 'Insufficient materials in stock', 1;
        
        IF @UnitCost IS NULL
        BEGIN
            SELECT @UnitCost = AVG(UnitCost)
            FROM OrderMaterials
            WHERE MaterialID = @MaterialID;
        END
        
        INSERT INTO OrderMaterials (OrderID, MaterialID, QuantityUsed, UnitCost)
        VALUES (@OrderID, @MaterialID, @QuantityUsed, @UnitCost);
        
        UPDATE Materials
        SET CurrentStock = CurrentStock - @QuantityUsed
        WHERE MaterialID = @MaterialID;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Процедура создания счета на заказ
CREATE PROCEDURE sp_CreateInvoice
    @OrderID INT,
    @DueDate DATE = NULL,
    @InvoiceID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @TotalAmount DECIMAL(12,2);
        
        SELECT @TotalAmount = TotalAmount
        FROM Orders
        WHERE OrderID = @OrderID;
        
        IF @DueDate IS NULL
            SET @DueDate = DATEADD(DAY, 14, GETDATE());
        
        INSERT INTO Invoices (OrderID, DueDate, TotalAmount)
        VALUES (@OrderID, @DueDate, @TotalAmount);
        
        SET @InvoiceID = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Процедура добавления отработанных часов
CREATE PROCEDURE sp_AddTimesheetEntry
    @OrderItemID INT,
    @EmployeeID INT,
    @WorkDate DATE,
    @HoursWorked DECIMAL(5,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @TotalHours DECIMAL(5,2);
        
        SELECT @TotalHours = SUM(HoursWorked)
        FROM Timesheets
        WHERE EmployeeID = @EmployeeID AND WorkDate = @WorkDate;
        
        IF @TotalHours + @HoursWorked > 24
            THROW 50003, 'Employee cannot work more than 24 hours per day', 1;
        
        INSERT INTO Timesheets (OrderItemID, EmployeeID, WorkDate, HoursWorked)
        VALUES (@OrderItemID, @EmployeeID, @WorkDate, @HoursWorked);
        
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Триггер для обновления TotalAmount при изменении OrderItems
CREATE OR ALTER TRIGGER tr_OrderItems_UpdateTotal
ON OrderItems
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @OrderID INT;
    
    DECLARE order_cursor CURSOR FOR
        SELECT DISTINCT OrderID FROM (
            SELECT OrderID FROM inserted
            UNION
            SELECT OrderID FROM deleted
        ) AS t
        WHERE OrderID IS NOT NULL;
    
    OPEN order_cursor;
    
    FETCH NEXT FROM order_cursor INTO @OrderID;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        UPDATE Orders
        SET TotalAmount = (
            SELECT ISNULL(SUM(Amount), 0)
            FROM OrderItems
            WHERE OrderID = @OrderID
        )
        WHERE OrderID = @OrderID;
        
        FETCH NEXT FROM order_cursor INTO @OrderID;
    END;
    
    CLOSE order_cursor;
    DEALLOCATE order_cursor;
END;
GO

-- Триггер для проверки дат
CREATE TRIGGER tr_Orders_CheckDates
ON Orders
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE RequiredDate < OrderDate
    )
    BEGIN
        THROW 50004, 'Required date cannot be earlier than order date', 1;
    END
END;
GO