CREATE DATABASE lab11;
GO

USE lab11;
GO

CREATE TABLE Orders
(
    OrderId INT PRIMARY KEY,
    CustomerName NVARCHAR(100) NOT NULL,
    ProductName NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    OrderDate DATE NOT NULL,
    Status NVARCHAR(20) DEFAULT N'Новый'
);
GO

INSERT INTO Orders (OrderId, CustomerName, ProductName, Quantity, Price, OrderDate, Status)
VALUES
(1, N'Иванов Иван', N'Ноутбук Lenovo', 1, 75000.00, '2024-02-15', N'Доставлен'),
(2, N'Петрова Ольга', N'Смартфон Xiaomi', 2, 25000.00, '2024-02-16', N'В обработке'),
(3, N'Сидоров Алексей', N'Наушники Sony', 3, 5000.00, '2024-02-17', N'Новый');
GO

SELECT * FROM Orders;
GO