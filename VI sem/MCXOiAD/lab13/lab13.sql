USE master;
GO
DROP DATABASE IF EXISTS OrderServiceDW;
GO
CREATE DATABASE OrderServiceDW;
GO
USE OrderServiceDW;
GO

CREATE TABLE DimCustomer (CustomerID INT PRIMARY KEY, CustomerName NVARCHAR(100), City NVARCHAR(50), Segment NVARCHAR(20), RegistrationDate DATE);
CREATE TABLE DimService (ServiceID INT PRIMARY KEY, ServiceName NVARCHAR(100), Category NVARCHAR(50), Subcategory NVARCHAR(50), Price DECIMAL(10,2), Cost DECIMAL(10,2));
CREATE TABLE DimDate (DateID INT PRIMARY KEY, FullDate DATE, Year INT, Quarter INT, QuarterName NVARCHAR(10), Month INT, MonthName NVARCHAR(20), Week INT, DayOfWeek NVARCHAR(20));
CREATE TABLE DimStatus (StatusID INT PRIMARY KEY, StatusName NVARCHAR(30), StatusGroup NVARCHAR(20));
CREATE TABLE FactOrders (OrderID INT PRIMARY KEY, CustomerID INT, ServiceID INT, DateID INT, StatusID INT, Quantity INT, UnitPrice DECIMAL(10,2), Discount DECIMAL(5,2), TotalAmount DECIMAL(10,2));

INSERT INTO DimCustomer VALUES (1, N'ООО "Ромашка"', N'Москва', N'B2B', '2024-01-15'),(2, N'ИП Смирнов', N'СПб', N'B2C', '2024-02-20'),(3, N'ЗАО "Лидер"', N'Казань', N'VIP', '2023-11-10');
INSERT INTO DimService VALUES (101, N'Консультация', N'Консалтинг', N'Юр', 3000, 1500),(102, N'Разработка', N'IT', N'Веб', 50000, 25000),(103, N'SEO', N'Маркетинг', N'Digital', 15000, 8000);
INSERT INTO DimStatus VALUES (1, N'Новый', N'Активные'),(2, N'В работе', N'Активные'),(3, N'Завершен', N'Завершенные');
INSERT INTO DimDate VALUES (20250101, '2025-01-01', 2025, 1, N'Q1', 1, N'Январь', 1, N'Пн'),(20250115, '2025-01-15', 2025, 1, N'Q1', 1, N'Январь', 3, N'Ср'),(20250210, '2025-02-10', 2025, 1, N'Q1', 2, N'Февраль', 6, N'Пн');
INSERT INTO FactOrders VALUES (1001, 1, 101, 20250101, 3, 2, 3000, 0, 6000),(1002, 2, 102, 20250101, 3, 1, 50000, 0, 50000),(1003, 1, 103, 20250115, 2, 3, 15000, 0, 45000),(1004, 3, 101, 20250210, 3, 5, 3000, 10, 13500);

ALTER TABLE FactOrders ADD FOREIGN KEY (CustomerID) REFERENCES DimCustomer(CustomerID);
ALTER TABLE FactOrders ADD FOREIGN KEY (ServiceID) REFERENCES DimService(ServiceID);
ALTER TABLE FactOrders ADD FOREIGN KEY (DateID) REFERENCES DimDate(DateID);
ALTER TABLE FactOrders ADD FOREIGN KEY (StatusID) REFERENCES DimStatus(StatusID);



USE master;
GO

CREATE LOGIN cube_user WITH PASSWORD = 'Qwerty123!';
GO

USE OrderServiceDW;
GO

CREATE USER cube_user FOR LOGIN cube_user;
GO

ALTER ROLE db_datareader ADD MEMBER cube_user;
GO
ALTER ROLE db_datawriter ADD MEMBER cube_user;
GO
ALTER ROLE db_owner ADD MEMBER cube_user;
GO