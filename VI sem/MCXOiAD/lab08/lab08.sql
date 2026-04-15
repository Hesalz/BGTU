SET SERVEROUTPUT ON;

DROP VIEW v_order_details;
DROP VIEW v_services_obj;

DROP TABLE obj_orders CASCADE CONSTRAINTS;
DROP TABLE obj_services CASCADE CONSTRAINTS;
DROP TABLE rel_orders CASCADE CONSTRAINTS;
DROP TABLE rel_services CASCADE CONSTRAINTS;

DROP TYPE TOrder FORCE;
DROP TYPE TService FORCE;

--2
CREATE OR REPLACE TYPE TService AS OBJECT (
    service_id   NUMBER(6),
    service_name VARCHAR2(100),
    price        NUMBER(10,2),
    
    --a
    CONSTRUCTOR FUNCTION TService(service_name VARCHAR2, price NUMBER)
    RETURN SELF AS RESULT,
    
    --b
    MAP MEMBER FUNCTION get_price RETURN NUMBER,
    
    --c
    MEMBER FUNCTION get_price_with_vat(vat_rate NUMBER) RETURN NUMBER,
    
    --d
    MEMBER PROCEDURE apply_discount(discount_percent NUMBER)
);

CREATE OR REPLACE TYPE BODY TService AS
    CONSTRUCTOR FUNCTION TService(service_name VARCHAR2, price NUMBER)
    RETURN SELF AS RESULT IS
    BEGIN
        SELF.service_id := NULL;
        SELF.service_name := service_name;
        SELF.price := price;
        RETURN;
    END;
    
    MAP MEMBER FUNCTION get_price RETURN NUMBER IS
    BEGIN
        RETURN price;
    END;
    
    MEMBER FUNCTION get_price_with_vat(vat_rate NUMBER) RETURN NUMBER IS
    BEGIN
        RETURN price * (1 + vat_rate/100);
    END;

    MEMBER PROCEDURE apply_discount(discount_percent NUMBER) IS
    BEGIN
        price := price * (1 - discount_percent/100);
    END;
    
END;

CREATE OR REPLACE TYPE TOrder AS OBJECT (
    order_id     NUMBER(8),
    order_date   DATE,
    quantity     NUMBER(4),
    service_ref  REF TService,
    total_amount NUMBER(12,2),
    
    --a
    CONSTRUCTOR FUNCTION TOrder(order_date DATE, quantity NUMBER, service_ref REF TService)
    RETURN SELF AS RESULT,
    
    --b
    ORDER MEMBER FUNCTION compare_order(other TOrder) RETURN INTEGER,
    
    --c
    MEMBER FUNCTION calculate_total RETURN NUMBER,
    
    --d
    MEMBER PROCEDURE update_total
);

CREATE OR REPLACE TYPE BODY TOrder AS
    
    CONSTRUCTOR FUNCTION TOrder(order_date DATE, quantity NUMBER, service_ref REF TService)
    RETURN SELF AS RESULT IS
    BEGIN
        SELF.order_id := NULL;
        SELF.order_date := order_date;
        SELF.quantity := quantity;
        SELF.service_ref := service_ref;
        SELF.total_amount := NULL;
        RETURN;
    END;
    
    ORDER MEMBER FUNCTION compare_order(other TOrder) RETURN INTEGER IS
    BEGIN
        IF SELF.order_date < other.order_date THEN
            RETURN -1;
        ELSIF SELF.order_date > other.order_date THEN
            RETURN 1;
        ELSE
            RETURN 0;
        END IF;
    END;
    
    MEMBER FUNCTION calculate_total RETURN NUMBER IS
        service_price NUMBER;
    BEGIN
        SELECT DEREF(service_ref).price INTO service_price FROM dual;
        RETURN quantity * service_price;
    END;
    
    MEMBER PROCEDURE update_total IS
    BEGIN
        total_amount := SELF.calculate_total();
    END;
    
END;

--3
CREATE TABLE rel_services (
    service_id NUMBER(6) PRIMARY KEY,
    service_name VARCHAR2(100),
    price NUMBER(10,2)
);

CREATE TABLE rel_orders (
    order_id NUMBER(8) PRIMARY KEY,
    order_date DATE,
    quantity NUMBER(4),
    service_id NUMBER(6),
    total_amount NUMBER(12,2)
);

INSERT INTO rel_services VALUES (1, 'Консультация', 5000.00);
INSERT INTO rel_services VALUES (2, 'Настройка ПО', 12000.00);
INSERT INTO rel_services VALUES (3, 'Обучение', 8000.00);
INSERT INTO rel_services VALUES (4, 'Техподдержка', 3000.00);
INSERT INTO rel_services VALUES (5, 'Разработка', 25000.00);

INSERT INTO rel_orders VALUES (101, DATE '2025-01-10', 2, 1, NULL);
INSERT INTO rel_orders VALUES (102, DATE '2025-01-15', 1, 2, NULL);
INSERT INTO rel_orders VALUES (103, DATE '2025-02-01', 3, 3, NULL);
INSERT INTO rel_orders VALUES (104, DATE '2025-02-10', 5, 4, NULL);
INSERT INTO rel_orders VALUES (105, DATE '2025-02-15', 1, 5, NULL);
COMMIT;

CREATE TABLE obj_services OF TService (
    service_id PRIMARY KEY
);

CREATE TABLE obj_orders OF TOrder (
    order_id PRIMARY KEY,
    CONSTRAINT order_service_ref_fk FOREIGN KEY (service_ref) REFERENCES obj_services
);

INSERT INTO obj_services (service_id, service_name, price)
SELECT service_id, service_name, price FROM rel_services;

INSERT INTO obj_orders (order_id, order_date, quantity, service_ref, total_amount)
SELECT o.order_id, o.order_date, o.quantity, 
       (SELECT REF(serv) FROM obj_services serv WHERE serv.service_id = o.service_id),
       NULL
FROM rel_orders o;

DECLARE
    CURSOR cur IS SELECT o.order_id, o.service_ref, o.quantity 
                  FROM obj_orders o;
    v_service_price NUMBER;
    v_total NUMBER;
BEGIN
    FOR rec IN cur LOOP
        SELECT DEREF(rec.service_ref).price INTO v_service_price FROM dual;
        v_total := rec.quantity * v_service_price;
        
        UPDATE obj_orders o 
        SET o.total_amount = v_total
        WHERE o.order_id = rec.order_id;
    END LOOP;
    COMMIT;
    DBMS_OUTPUT.PUT_LINE('Total_amount обновлен для всех заказов');
END;

BEGIN
    FOR i IN 1..50 LOOP
        INSERT INTO obj_services (service_id, service_name, price)
        VALUES (100 + i, 'Услуга_' || i, ROUND(DBMS_RANDOM.VALUE(1000, 100000), 2));
    END LOOP;
    
    FOR i IN 1..100 LOOP
        INSERT INTO obj_orders (order_id, order_date, quantity, service_ref, total_amount)
        VALUES (
            1000 + i,
            SYSDATE - ROUND(DBMS_RANDOM.VALUE(1, 365)),
            ROUND(DBMS_RANDOM.VALUE(1, 10)),
            (SELECT REF(s) FROM obj_services s WHERE s.service_id = ROUND(DBMS_RANDOM.VALUE(1, 55))),
            NULL
        );
    END LOOP;
    COMMIT;
END;


DECLARE
    v_total NUMBER;
    v_price NUMBER;
BEGIN
    FOR rec IN (SELECT o.order_id, o.quantity, o.service_ref FROM obj_orders o WHERE o.total_amount IS NULL) LOOP
        SELECT DEREF(rec.service_ref).price INTO v_price FROM dual;
        v_total := rec.quantity * v_price;
        UPDATE obj_orders o SET o.total_amount = v_total WHERE o.order_id = rec.order_id;
    END LOOP;
    COMMIT;
    DBMS_OUTPUT.PUT_LINE('Total_amount обновлен для дополнительных заказов');
END;

BEGIN
    DBMS_STATS.GATHER_TABLE_STATS(USER, 'OBJ_SERVICES', 
        estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE,
        method_opt => 'FOR ALL COLUMNS SIZE AUTO',
        cascade => TRUE);
    
    DBMS_STATS.GATHER_TABLE_STATS(USER, 'OBJ_ORDERS',
        estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE,
        method_opt => 'FOR ALL COLUMNS SIZE AUTO',
        cascade => TRUE);
END;

SELECT * FROM obj_services WHERE service_id <= 5;

SELECT o.order_id, o.order_date, o.quantity, 
       DEREF(o.service_ref).service_name AS service_name,
       DEREF(o.service_ref).price AS price,
       o.total_amount
FROM obj_orders o WHERE o.order_id <= 105;





--4
CREATE OR REPLACE VIEW v_services_obj OF TService
WITH OBJECT OID (service_id) AS
    SELECT s.service_id, s.service_name, s.price
    FROM obj_services s;

CREATE OR REPLACE VIEW v_order_details AS
    SELECT o.order_id,
           o.order_date,
           o.quantity,
           DEREF(o.service_ref).service_name AS service_name,
           DEREF(o.service_ref).price AS unit_price,
           o.total_amount,
           DEREF(o.service_ref).get_price_with_vat(20) AS price_with_vat_20,
           o.quantity * DEREF(o.service_ref).get_price_with_vat(20) AS total_with_vat
    FROM obj_orders o;

SELECT * FROM v_services_obj WHERE service_id <= 5;
SELECT * FROM v_order_details WHERE order_id <= 105;




--5
CREATE INDEX idx_orders_order_date ON obj_orders(order_date);
CREATE INDEX idx_services_price ON obj_services(price);
CREATE INDEX idx_services_price_vat20 ON obj_services(price * 1.2);
CREATE INDEX idx_services_discount10 ON obj_services(price * 0.9);

SELECT index_name, index_type, uniqueness, status
FROM user_indexes 
WHERE table_name IN ('OBJ_SERVICES', 'OBJ_ORDERS')
ORDER BY table_name, index_name;


EXPLAIN PLAN FOR
    SELECT /*+ INDEX(o idx_orders_order_date) */
           o.order_id, o.order_date, o.total_amount 
    FROM obj_orders o 
    WHERE o.order_date > DATE '2025-01-20';
SELECT * FROM TABLE(DBMS_XPLAN.DISPLAY);


EXPLAIN PLAN FOR
    SELECT /*+ INDEX(s idx_services_price) */
           s.service_name, s.price 
    FROM obj_services s 
    WHERE s.price BETWEEN 5000 AND 20000;
SELECT * FROM TABLE(DBMS_XPLAN.DISPLAY);


EXPLAIN PLAN FOR
    SELECT /*+ INDEX(s idx_services_price_vat20) */
           service_name, price, price * 1.2 AS price_vat
    FROM obj_services s
    WHERE price * 1.2 > 10000;
SELECT * FROM TABLE(DBMS_XPLAN.DISPLAY);


EXPLAIN PLAN FOR
    SELECT /*+ INDEX(s idx_services_discount10) */
           service_name, price, price * 0.9 AS price_discount10
    FROM obj_services s
    WHERE price * 0.9 < 10000;
SELECT * FROM TABLE(DBMS_XPLAN.DISPLAY);


SELECT index_name, column_expression
FROM user_ind_expressions
WHERE table_name = 'OBJ_SERVICES';










--
-- MAP метод
SELECT '=== 1. MAP метод (сортировка по цене) ===' AS demonstration FROM dual;
SELECT service_name, price 
FROM obj_services 
WHERE service_id <= 10
ORDER BY price;

-- ORDER метод
BEGIN
    DECLARE
        ord1 TOrder;
        ord2 TOrder;
        ord3 TOrder;
        cmp12 INTEGER;
        cmp13 INTEGER;
    BEGIN
        SELECT VALUE(o) INTO ord1 FROM obj_orders o WHERE order_id = 101;
        SELECT VALUE(o) INTO ord2 FROM obj_orders o WHERE order_id = 102;
        SELECT VALUE(o) INTO ord3 FROM obj_orders o WHERE order_id = 103;
        
        cmp12 := ord1.compare_order(ord2);
        cmp13 := ord1.compare_order(ord3);
        
        DBMS_OUTPUT.PUT_LINE('=== 2. ORDER метод (сравнение заказов) ===');
        DBMS_OUTPUT.PUT_LINE('Заказ 101 (10.01.2025) vs Заказ 102 (15.01.2025): ' || cmp12);
        DBMS_OUTPUT.PUT_LINE('Заказ 101 (10.01.2025) vs Заказ 103 (01.02.2025): ' || cmp13);
        DBMS_OUTPUT.PUT_LINE('( -1 = раньше, 0 = одинаково, 1 = позже )');
    END;
END;

-- метод-функция get_price_with_vat
SELECT '=== 3. Метод-функция get_price_with_vat ===' AS demonstration FROM dual;
SELECT service_name, price, 
       price * 1.2 AS price_vat_20,
       price * 1.1 AS price_vat_10
FROM obj_services
WHERE service_id <= 5;

-- метод-процедура apply_discount
BEGIN
    DECLARE
        serv TService;
        old_price NUMBER;
    BEGIN
        SELECT VALUE(s) INTO serv FROM obj_services s WHERE service_id = 1;
        old_price := serv.price;
        
        DBMS_OUTPUT.PUT_LINE('=== 4. Метод-процедура apply_discount ===');
        DBMS_OUTPUT.PUT_LINE('Услуга: ' || serv.service_name);
        DBMS_OUTPUT.PUT_LINE('Старая цена: ' || old_price);
        
        serv.apply_discount(15); -- Скидка 15%
        DBMS_OUTPUT.PUT_LINE('Новая цена после скидки 15%: ' || serv.price);
        
        UPDATE obj_services s SET s.price = serv.price WHERE service_id = 1;
        COMMIT;
        DBMS_OUTPUT.PUT_LINE('Изменения сохранены в таблице');
    END;
END;


UPDATE obj_services SET price = 5000.00 WHERE service_id = 1;
COMMIT;
SELECT 'Цена услуги 1 восстановлена до 5000' AS status FROM dual;

-- метод-процедура update_total
BEGIN
    DECLARE
        ord TOrder;
        old_total NUMBER;
    BEGIN
        SELECT VALUE(o) INTO ord FROM obj_orders o WHERE order_id = 101;
        old_total := ord.total_amount;
        
        DBMS_OUTPUT.PUT_LINE('=== 5. Метод-процедура update_total ===');
        DBMS_OUTPUT.PUT_LINE('Заказ ID: ' || ord.order_id);
        DBMS_OUTPUT.PUT_LINE('Количество: ' || ord.quantity);
        DBMS_OUTPUT.PUT_LINE('Старая сумма: ' || NVL(old_total, 0));
        
        ord.update_total();
        DBMS_OUTPUT.PUT_LINE('Пересчитанная сумма: ' || ord.total_amount);
        
        UPDATE obj_orders o SET o.total_amount = ord.total_amount WHERE o.order_id = ord.order_id;
        COMMIT;
        DBMS_OUTPUT.PUT_LINE('Изменения сохранены в таблице');
    END;
END;