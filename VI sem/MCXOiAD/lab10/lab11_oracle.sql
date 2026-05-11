CREATE TABLE Orders
(
    OrderId NUMBER PRIMARY KEY,
    CustomerName NVARCHAR2(100) NOT NULL,
    ProductName NVARCHAR2(100) NOT NULL,
    Quantity NUMBER NOT NULL,
    Price NUMBER(10,2) NOT NULL,
    OrderDate DATE NOT NULL,
    Status NVARCHAR2(20) DEFAULT 'Новый'
);

SELECT * FROM Orders;

DROP TABLE Orders;