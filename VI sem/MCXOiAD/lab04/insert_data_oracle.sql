-- Отключаем триггеры временно
ALTER TRIGGER tr_OrderItems_UpdateTotal DISABLE;
ALTER TRIGGER tr_Orders_CheckDates DISABLE;
/

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
COMMIT;
/

-- Сбрасываем последовательности (автоинкремент) для всех таблиц
BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE seq_customers'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE seq_employees'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE seq_orderstatuses'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE seq_services'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE seq_materials'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE seq_orders'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE seq_orderitems'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE seq_invoices'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE seq_payments'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE seq_timesheets'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE seq_categories'; EXCEPTION WHEN OTHERS THEN NULL; END;
/

CREATE SEQUENCE seq_customers START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE seq_employees START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE seq_orderstatuses START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE seq_services START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE seq_materials START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE seq_orders START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE seq_orderitems START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE seq_invoices START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE seq_payments START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE seq_timesheets START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE seq_categories START WITH 1 INCREMENT BY 1;
/

-- 1. Создаём категории услуг (простые, без иерархии)
INSERT INTO ServiceCategories (CategoryID, CategoryName) VALUES (seq_categories.NEXTVAL, 'Ремонт');
INSERT INTO ServiceCategories (CategoryID, CategoryName) VALUES (seq_categories.NEXTVAL, 'Диагностика');
INSERT INTO ServiceCategories (CategoryID, CategoryName) VALUES (seq_categories.NEXTVAL, 'Софт');
COMMIT;
/

-- 2. Customers
INSERT INTO Customers (CustomerID, Name, ContactPerson, Phone, Email, Address, RegistrationDate)
VALUES (seq_customers.NEXTVAL, 'ООО "Ромашка"', 'Иванов Иван', '+7(495)123-45-67', 'info@romashka.ru', 'ул. Ленина, д.1', DATE '2023-01-15');
INSERT INTO Customers (CustomerID, Name, ContactPerson, Phone, Email, Address, RegistrationDate)
VALUES (seq_customers.NEXTVAL, 'ИП Петров', 'Петров Петр', '+7(495)234-56-78', 'petrov@mail.ru', 'ул. Гагарина, д.5', DATE '2023-02-20');
INSERT INTO Customers (CustomerID, Name, ContactPerson, Phone, Email, Address, RegistrationDate)
VALUES (seq_customers.NEXTVAL, 'ЗАО "Техсервис"', 'Сидоров Сидор', '+7(495)345-67-89', 'sidorov@tehservice.ru', 'пр. Мира, д.10', DATE '2023-03-10');
INSERT INTO Customers (CustomerID, Name, ContactPerson, Phone, Email, Address, RegistrationDate)
VALUES (seq_customers.NEXTVAL, 'ООО "СтройИнвест"', 'Николаев Николай', '+7(495)456-78-90', 'nikolaev@stroyinvest.ru', 'ул. Строителей, д.15', DATE '2023-04-05');
INSERT INTO Customers (CustomerID, Name, ContactPerson, Phone, Email, Address, RegistrationDate)
VALUES (seq_customers.NEXTVAL, 'АО "ТоргМаш"', 'Алексеев Алексей', '+7(495)567-89-01', 'alekseev@torgmash.ru', 'ул. Машиностроителей, д.20', DATE '2023-05-12');
COMMIT;
/

-- 3. Employees
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, Email, HireDate)
VALUES (seq_employees.NEXTVAL, 'Смирнов Андрей', 'Менеджер', '+7(495)111-22-33', 'a.smirnov@company.ru', DATE '2022-01-10');
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, Email, HireDate)
VALUES (seq_employees.NEXTVAL, 'Кузнецова Елена', 'Бухгалтер', '+7(495)222-33-44', 'e.kuznetsova@company.ru', DATE '2022-02-15');
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, Email, HireDate)
VALUES (seq_employees.NEXTVAL, 'Попов Дмитрий', 'Мастер', '+7(495)333-44-55', 'd.popov@company.ru', DATE '2022-03-20');
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, Email, HireDate)
VALUES (seq_employees.NEXTVAL, 'Васильева Ольга', 'Мастер', '+7(495)444-55-66', 'o.vasilieva@company.ru', DATE '2022-04-25');
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, Email, HireDate)
VALUES (seq_employees.NEXTVAL, 'Михайлов Игорь', 'Техник', '+7(495)555-66-77', 'i.mihaylov@company.ru', DATE '2022-05-30');
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, Email, HireDate)
VALUES (seq_employees.NEXTVAL, 'Федорова Наталья', 'Менеджер', '+7(495)666-77-88', 'n.fedorova@company.ru', DATE '2022-06-05');
COMMIT;
/

-- 4. OrderStatuses
INSERT INTO OrderStatuses (StatusID, StatusName, Description)
VALUES (seq_orderstatuses.NEXTVAL, 'Новый', 'Заказ только создан');
INSERT INTO OrderStatuses (StatusID, StatusName, Description)
VALUES (seq_orderstatuses.NEXTVAL, 'В работе', 'Заказ выполняется');
INSERT INTO OrderStatuses (StatusID, StatusName, Description)
VALUES (seq_orderstatuses.NEXTVAL, 'Завершен', 'Заказ выполнен');
INSERT INTO OrderStatuses (StatusID, StatusName, Description)
VALUES (seq_orderstatuses.NEXTVAL, 'Отменен', 'Заказ отменен клиентом');
INSERT INTO OrderStatuses (StatusID, StatusName, Description)
VALUES (seq_orderstatuses.NEXTVAL, 'Ожидание', 'Ожидает поступления материалов');
COMMIT;
/

-- 5. Services
INSERT INTO Services (ServiceID, ServiceName, Description, DefaultPrice, Unit, CategoryID)
VALUES (seq_services.NEXTVAL, 'Ремонт смартфона', 'Замена экрана, аккумулятора', 3000.00, 'шт', 
    (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = 'Ремонт'));
INSERT INTO Services (ServiceID, ServiceName, Description, DefaultPrice, Unit, CategoryID)
VALUES (seq_services.NEXTVAL, 'Ремонт ноутбука', 'Замена клавиатуры, матрицы', 3500.00, 'шт', 
    (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = 'Ремонт'));
INSERT INTO Services (ServiceID, ServiceName, Description, DefaultPrice, Unit, CategoryID)
VALUES (seq_services.NEXTVAL, 'Ремонт планшета', 'Замена стекла, аккумулятора', 2800.00, 'шт', 
    (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = 'Ремонт'));
INSERT INTO Services (ServiceID, ServiceName, Description, DefaultPrice, Unit, CategoryID)
VALUES (seq_services.NEXTVAL, 'Диагностика компьютера', 'Полная проверка оборудования', 1500.00, 'шт', 
    (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = 'Диагностика'));
INSERT INTO Services (ServiceID, ServiceName, Description, DefaultPrice, Unit, CategoryID)
VALUES (seq_services.NEXTVAL, 'Диагностика ноутбука', 'Выявление неисправностей', 1200.00, 'шт', 
    (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = 'Диагностика'));
INSERT INTO Services (ServiceID, ServiceName, Description, DefaultPrice, Unit, CategoryID)
VALUES (seq_services.NEXTVAL, 'Установка Windows', 'Установка ОС с настройкой', 2000.00, 'шт', 
    (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = 'Софт'));
INSERT INTO Services (ServiceID, ServiceName, Description, DefaultPrice, Unit, CategoryID)
VALUES (seq_services.NEXTVAL, 'Установка офисного ПО', 'Microsoft Office, антивирусы', 1000.00, 'шт', 
    (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = 'Софт'));
INSERT INTO Services (ServiceID, ServiceName, Description, DefaultPrice, Unit, CategoryID)
VALUES (seq_services.NEXTVAL, 'Настройка ПО', 'Установка драйверов, настройка', 800.00, 'шт', 
    (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = 'Софт'));
COMMIT;
/

-- 6. Materials
INSERT INTO Materials (MaterialID, MaterialName, Unit, CurrentStock)
VALUES (seq_materials.NEXTVAL, 'Экран для iPhone X', 'шт', 15);
INSERT INTO Materials (MaterialID, MaterialName, Unit, CurrentStock)
VALUES (seq_materials.NEXTVAL, 'Аккумулятор для ноутбука ASUS', 'шт', 8);
INSERT INTO Materials (MaterialID, MaterialName, Unit, CurrentStock)
VALUES (seq_materials.NEXTVAL, 'Термопаста Arctic', 'г', 200);
INSERT INTO Materials (MaterialID, MaterialName, Unit, CurrentStock)
VALUES (seq_materials.NEXTVAL, 'Винт M2x5', 'упак', 50);
INSERT INTO Materials (MaterialID, MaterialName, Unit, CurrentStock)
VALUES (seq_materials.NEXTVAL, 'Кабель SATA', 'шт', 30);
INSERT INTO Materials (MaterialID, MaterialName, Unit, CurrentStock)
VALUES (seq_materials.NEXTVAL, 'Мышь оптическая', 'шт', 20);
COMMIT;
/

-- 7. Базовые 6 заказов (июнь 2024)
INSERT INTO Orders (OrderID, CustomerID, OrderDate, RequiredDate, StatusID, CreatedBy, Notes)
VALUES (seq_orders.NEXTVAL, 
    (SELECT CustomerID FROM Customers WHERE Email = 'info@romashka.ru'), 
    DATE '2024-06-01', DATE '2024-06-05',
    (SELECT StatusID FROM OrderStatuses WHERE StatusName = 'Новый'),
    (SELECT EmployeeID FROM Employees WHERE Email = 'a.smirnov@company.ru'), 
    'Срочный ремонт');

INSERT INTO Orders (OrderID, CustomerID, OrderDate, RequiredDate, StatusID, CreatedBy, Notes)
VALUES (seq_orders.NEXTVAL, 
    (SELECT CustomerID FROM Customers WHERE Email = 'petrov@mail.ru'), 
    DATE '2024-06-02', DATE '2024-06-07',
    (SELECT StatusID FROM OrderStatuses WHERE StatusName = 'В работе'),
    (SELECT EmployeeID FROM Employees WHERE Email = 'n.fedorova@company.ru'), 
    'Диагностика и ремонт');

INSERT INTO Orders (OrderID, CustomerID, OrderDate, RequiredDate, StatusID, CreatedBy, Notes)
VALUES (seq_orders.NEXTVAL, 
    (SELECT CustomerID FROM Customers WHERE Email = 'sidorov@tehservice.ru'), 
    DATE '2024-06-03', DATE '2024-06-10',
    (SELECT StatusID FROM OrderStatuses WHERE StatusName = 'Завершен'),
    (SELECT EmployeeID FROM Employees WHERE Email = 'a.smirnov@company.ru'), 
    'Установка ПО');

INSERT INTO Orders (OrderID, CustomerID, OrderDate, RequiredDate, StatusID, CreatedBy, Notes)
VALUES (seq_orders.NEXTVAL, 
    (SELECT CustomerID FROM Customers WHERE Email = 'nikolaev@stroyinvest.ru'), 
    DATE '2024-06-04', DATE '2024-06-12',
    (SELECT StatusID FROM OrderStatuses WHERE StatusName = 'Ожидание'),
    (SELECT EmployeeID FROM Employees WHERE Email = 'd.popov@company.ru'), 
    'Ремонт принтера');

INSERT INTO Orders (OrderID, CustomerID, OrderDate, RequiredDate, StatusID, CreatedBy, Notes)
VALUES (seq_orders.NEXTVAL, 
    (SELECT CustomerID FROM Customers WHERE Email = 'alekseev@torgmash.ru'), 
    DATE '2024-06-05', DATE '2024-06-09',
    (SELECT StatusID FROM OrderStatuses WHERE StatusName = 'Новый'),
    (SELECT EmployeeID FROM Employees WHERE Email = 'i.mihaylov@company.ru'), 
    'Замена батареи');

INSERT INTO Orders (OrderID, CustomerID, OrderDate, RequiredDate, StatusID, CreatedBy, Notes)
VALUES (seq_orders.NEXTVAL, 
    (SELECT CustomerID FROM Customers WHERE Email = 'info@romashka.ru'), 
    DATE '2024-06-06', DATE '2024-06-11',
    (SELECT StatusID FROM OrderStatuses WHERE StatusName = 'В работе'),
    (SELECT EmployeeID FROM Employees WHERE Email = 'o.vasilieva@company.ru'), 
    'Повторный ремонт');
COMMIT;
/

-- 8. Генерация 200 дополнительных заказов с датами 2024–2026
DECLARE
    v_i NUMBER := 1;
    v_custID NUMBER;
    v_statusID NUMBER;
    v_empID NUMBER;
    v_orderDate DATE;
BEGIN
    WHILE v_i <= 200 LOOP
        SELECT CustomerID INTO v_custID FROM (
            SELECT CustomerID FROM Customers ORDER BY DBMS_RANDOM.VALUE
        ) WHERE ROWNUM = 1;
        
        SELECT StatusID INTO v_statusID FROM (
            SELECT StatusID FROM OrderStatuses ORDER BY DBMS_RANDOM.VALUE
        ) WHERE ROWNUM = 1;
        
        SELECT EmployeeID INTO v_empID FROM (
            SELECT EmployeeID FROM Employees ORDER BY DBMS_RANDOM.VALUE
        ) WHERE ROWNUM = 1;
        
        v_orderDate := DATE '2024-01-01' + TRUNC(DBMS_RANDOM.VALUE(0, 1095));
        
        INSERT INTO Orders (OrderID, CustomerID, OrderDate, RequiredDate, StatusID, CreatedBy, Notes)
        VALUES (seq_orders.NEXTVAL, v_custID, v_orderDate, 
                v_orderDate + TRUNC(DBMS_RANDOM.VALUE(3, 15)), 
                v_statusID, v_empID, 'BULK_ORDER_' || v_i);
        
        v_i := v_i + 1;
    END LOOP;
    COMMIT;
END;
/

-- 9. Вставка OrderItems с распределением по весам
DECLARE
    v_repair_id NUMBER;
    v_diag_id NUMBER;
    v_soft_id NUMBER;
BEGIN
    SELECT CategoryID INTO v_repair_id FROM ServiceCategories WHERE CategoryName = 'Ремонт';
    SELECT CategoryID INTO v_diag_id FROM ServiceCategories WHERE CategoryName = 'Диагностика';
    SELECT CategoryID INTO v_soft_id FROM ServiceCategories WHERE CategoryName = 'Софт';
    
    FOR rec IN (SELECT OrderID FROM Orders) LOOP
        FOR j IN 1..TRUNC(DBMS_RANDOM.VALUE(1, 4)) LOOP
            DECLARE
                v_cat_id NUMBER;
                v_rand NUMBER := TRUNC(DBMS_RANDOM.VALUE(0, 10));
            BEGIN
                IF v_rand < 5 THEN
                    v_cat_id := v_repair_id;
                ELSIF v_rand < 8 THEN
                    v_cat_id := v_diag_id;
                ELSE
                    v_cat_id := v_soft_id;
                END IF;
                
                INSERT INTO OrderItems (OrderItemID, OrderID, ServiceID, Quantity, UnitPrice, Discount)
                SELECT seq_orderitems.NEXTVAL, rec.OrderID, ServiceID, 
                       TRUNC(DBMS_RANDOM.VALUE(1, 4)), DefaultPrice,
                       CASE TRUNC(DBMS_RANDOM.VALUE(0, 4))
                           WHEN 0 THEN 0
                           WHEN 1 THEN 5
                           WHEN 2 THEN 10
                           ELSE 15
                       END
                FROM (
                    SELECT ServiceID, DefaultPrice FROM Services 
                    WHERE CategoryID = v_cat_id 
                    ORDER BY DBMS_RANDOM.VALUE
                ) WHERE ROWNUM = 1;
            END;
        END LOOP;
    END LOOP;
    COMMIT;
END;
/

-- 10. Вставка OrderMaterials (уникальные пары)
DECLARE
    v_orders SYS.ODCINUMBERLIST;
    v_materials SYS.ODCINUMBERLIST;
    v_count NUMBER := 0;
BEGIN
    SELECT OrderID BULK COLLECT INTO v_orders FROM (
        SELECT OrderID FROM Orders ORDER BY DBMS_RANDOM.VALUE
    ) WHERE ROWNUM <= 50;
    
    SELECT MaterialID BULK COLLECT INTO v_materials FROM Materials;
    
    FOR i IN 1..v_orders.COUNT LOOP
        FOR j IN 1..v_materials.COUNT LOOP
            IF v_count >= 250 THEN EXIT; END IF;
            
            BEGIN
                INSERT INTO OrderMaterials (OrderID, MaterialID, QuantityUsed, UnitCost)
                VALUES (v_orders(i), v_materials(j), 
                        TRUNC(DBMS_RANDOM.VALUE(1, 6)), 
                        TRUNC(DBMS_RANDOM.VALUE(50, 2000)));
                v_count := v_count + 1;
            EXCEPTION
                WHEN DUP_VAL_ON_INDEX THEN
                    NULL;
            END;
        END LOOP;
    END LOOP;
    COMMIT;
END;
/

-- 11. Вставка Invoices для всех заказов
DECLARE
    v_total NUMBER;
BEGIN
    FOR rec IN (SELECT OrderID, OrderDate FROM Orders) LOOP
        SELECT NVL(SUM(Amount), 0) INTO v_total 
        FROM OrderItems WHERE OrderID = rec.OrderID;
        
        INSERT INTO Invoices (InvoiceID, OrderID, InvoiceDate, DueDate, TotalAmount, PaidAmount)
        VALUES (seq_invoices.NEXTVAL, rec.OrderID, rec.OrderDate, 
                rec.OrderDate + 14, v_total,
                CASE TRUNC(DBMS_RANDOM.VALUE(0, 3))
                    WHEN 0 THEN 0
                    WHEN 1 THEN v_total / 2
                    ELSE v_total
                END);
    END LOOP;
    COMMIT;
END;
/

-- 12. Вставка Payments
DECLARE
    CURSOR c_invoices IS 
        SELECT InvoiceID, PaidAmount, InvoiceDate FROM Invoices 
        WHERE PaidAmount > 0;
    v_payment_count NUMBER;
BEGIN
    FOR rec IN c_invoices LOOP
        v_payment_count := TRUNC(DBMS_RANDOM.VALUE(1, 3));
        
        FOR j IN 1..v_payment_count LOOP
            INSERT INTO Payments (PaymentID, InvoiceID, PaymentDate, Amount, PaymentMethod)
            VALUES (seq_payments.NEXTVAL, rec.InvoiceID, 
                    rec.InvoiceDate + TRUNC(DBMS_RANDOM.VALUE(0, 10)),
                    rec.PaidAmount / v_payment_count,
                    CASE TRUNC(DBMS_RANDOM.VALUE(0, 4))
                        WHEN 0 THEN 'Наличные'
                        WHEN 1 THEN 'Банковская карта'
                        WHEN 2 THEN 'Безналичный расчет'
                        ELSE 'Электронные деньги'
                    END);
        END LOOP;
    END LOOP;
    COMMIT;
END;
/

-- 13. Вставка Timesheets
DECLARE
    v_empID NUMBER;
BEGIN
    FOR rec IN (SELECT oi.OrderItemID, o.OrderDate FROM OrderItems oi JOIN Orders o ON oi.OrderID = o.OrderID) LOOP
        SELECT EmployeeID INTO v_empID FROM (
            SELECT EmployeeID FROM Employees ORDER BY DBMS_RANDOM.VALUE
        ) WHERE ROWNUM = 1;
        
        INSERT INTO Timesheets (TimesheetID, OrderItemID, EmployeeID, WorkDate, HoursWorked)
        VALUES (seq_timesheets.NEXTVAL, rec.OrderItemID, v_empID,
                rec.OrderDate + TRUNC(DBMS_RANDOM.VALUE(0, 30)),
                TRUNC(DBMS_RANDOM.VALUE(1, 9)));
    END LOOP;
    COMMIT;
END;
/

-- 14. Обновление TotalAmount в Orders
DECLARE
    v_total NUMBER;
BEGIN
    FOR rec IN (SELECT OrderID FROM Orders) LOOP
        SELECT NVL(SUM(Amount), 0) INTO v_total 
        FROM OrderItems WHERE OrderID = rec.OrderID;
        
        UPDATE Orders SET TotalAmount = v_total WHERE OrderID = rec.OrderID;
    END LOOP;
    COMMIT;
END;
/

-- 15. Включаем триггеры обратно
ALTER TRIGGER tr_OrderItems_UpdateTotal ENABLE;
ALTER TRIGGER tr_Orders_CheckDates ENABLE;
/

-- 16. Проверка количества записей (исправленный синтаксис)
SELECT 'Customers' AS TableName, (SELECT COUNT(*) FROM Customers) AS RowsCount FROM DUAL
UNION ALL
SELECT 'Employees', (SELECT COUNT(*) FROM Employees) FROM DUAL
UNION ALL
SELECT 'OrderStatuses', (SELECT COUNT(*) FROM OrderStatuses) FROM DUAL
UNION ALL
SELECT 'Services', (SELECT COUNT(*) FROM Services) FROM DUAL
UNION ALL
SELECT 'Materials', (SELECT COUNT(*) FROM Materials) FROM DUAL
UNION ALL
SELECT 'Orders', (SELECT COUNT(*) FROM Orders) FROM DUAL
UNION ALL
SELECT 'OrderItems', (SELECT COUNT(*) FROM OrderItems) FROM DUAL
UNION ALL
SELECT 'OrderMaterials', (SELECT COUNT(*) FROM OrderMaterials) FROM DUAL
UNION ALL
SELECT 'Invoices', (SELECT COUNT(*) FROM Invoices) FROM DUAL
UNION ALL
SELECT 'Payments', (SELECT COUNT(*) FROM Payments) FROM DUAL
UNION ALL
SELECT 'Timesheets', (SELECT COUNT(*) FROM Timesheets) FROM DUAL;
/

COMMIT;
/