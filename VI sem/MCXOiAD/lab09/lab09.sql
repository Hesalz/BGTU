SET SERVEROUTPUT ON;
SET LINESIZE 200;

BEGIN
    EXECUTE IMMEDIATE 'DROP TABLE t2 PURGE';
    EXECUTE IMMEDIATE 'DROP TABLE t1 PURGE';
    EXECUTE IMMEDIATE 'DROP TYPE k2_type FORCE';
    EXECUTE IMMEDIATE 'DROP TYPE k1_type FORCE';
    EXECUTE IMMEDIATE 'DROP TYPE t2_type FORCE';
    EXECUTE IMMEDIATE 'DROP TYPE t1_type FORCE';
EXCEPTION
    WHEN OTHERS THEN NULL;
END;
/

CREATE TYPE t2_type AS OBJECT (
    service_id   NUMBER(5),
    service_name VARCHAR2(100),
    price        NUMBER(10,2)
);
/

CREATE TYPE k2_type AS TABLE OF t2_type;
/

CREATE TYPE t1_type AS OBJECT (
    order_id      NUMBER(5),
    customer_name VARCHAR2(100),
    order_date    DATE,
    services      k2_type
);
/

CREATE TYPE k1_type AS TABLE OF t1_type;
/

CREATE TABLE t1 OF t1_type 
    NESTED TABLE services STORE AS t1_services_tab;

CREATE TABLE t2 OF t2_type;

INSERT INTO t2 VALUES (101, 'Консультация', 5000);
INSERT INTO t2 VALUES (102, 'Разработка ТЗ', 12000);
INSERT INTO t2 VALUES (103, 'Настройка сервера', 8000);
INSERT INTO t2 VALUES (104, 'Обучение персонала', 15000);
INSERT INTO t2 VALUES (105, 'Техподдержка мес.', 20000);

INSERT INTO t1 VALUES (1, 'ООО "Ромашка"', DATE '2025-03-10',
    k2_type(t2_type(101, 'Консультация', 5000), t2_type(102, 'Разработка ТЗ', 12000)));
INSERT INTO t1 VALUES (2, 'ИП Иванов', DATE '2025-03-12',
    k2_type(t2_type(103, 'Настройка сервера', 8000)));
INSERT INTO t1 VALUES (3, 'ЗАО "Вектор"', DATE '2025-03-15', k2_type());
INSERT INTO t1 VALUES (4, 'ООО "Глобус"', DATE '2025-03-18',
    k2_type(t2_type(101, 'Консультация', 5000), t2_type(105, 'Техподдержка мес.', 20000)));
INSERT INTO t1 VALUES (5, 'АО "Старт"', DATE '2025-03-20',
    k2_type(t2_type(102, 'Разработка ТЗ', 12000), t2_type(104, 'Обучение персонала', 15000)));

COMMIT;

--2
--2a
DECLARE
    K1 k1_type := k1_type();
    
    CURSOR c_orders IS
        SELECT order_id, customer_name, order_date, services
        FROM t1;
    
    v_order_id      t1.order_id%TYPE;
    v_customer_name t1.customer_name%TYPE;
    v_order_date    t1.order_date%TYPE;
    v_services      k2_type;
BEGIN
    OPEN c_orders;
    LOOP
        FETCH c_orders INTO v_order_id, v_customer_name, v_order_date, v_services;
        EXIT WHEN c_orders%NOTFOUND;
        K1.EXTEND;
        K1(K1.COUNT) := t1_type(v_order_id, v_customer_name, v_order_date, v_services);
    END LOOP;
    CLOSE c_orders;
    
    DBMS_OUTPUT.PUT_LINE('Коллекция K1 (заказы) создана. Всего элементов: ' || K1.COUNT);
    DBMS_OUTPUT.PUT_LINE('Тип коллекции K1: k1_type');
    DBMS_OUTPUT.PUT_LINE('Тип вложенной коллекции K2: k2_type (услуги)');
    
    FOR i IN 1..K1.COUNT LOOP
        DBMS_OUTPUT.PUT_LINE('  Заказ ' || i || ': ID=' || K1(i).order_id || 
                             ', Клиент=' || K1(i).customer_name ||
                             ', Услуг=' || CASE WHEN K1(i).services IS NOT NULL THEN K1(i).services.COUNT ELSE 0 END);
    END LOOP;
END;
/



--2b
DECLARE
    K1 k1_type := k1_type();
    
    CURSOR c_orders IS
        SELECT order_id, customer_name, order_date, services
        FROM t1;
    
    v_order_id      t1.order_id%TYPE;
    v_customer_name t1.customer_name%TYPE;
    v_order_date    t1.order_date%TYPE;
    v_services      k2_type;
    
    v_intersection_found BOOLEAN;
    v_common_services    VARCHAR2(200);
BEGIN
    OPEN c_orders;
    LOOP
        FETCH c_orders INTO v_order_id, v_customer_name, v_order_date, v_services;
        EXIT WHEN c_orders%NOTFOUND;
        K1.EXTEND;
        K1(K1.COUNT) := t1_type(v_order_id, v_customer_name, v_order_date, v_services);
    END LOOP;
    CLOSE c_orders;
    
    DBMS_OUTPUT.PUT_LINE('=== ПЕРЕСЕЧЕНИЕ КОЛЛЕКЦИЙ K2 МЕЖДУ РАЗНЫМИ ЗАКАЗАМИ ===');
    DBMS_OUTPUT.PUT_LINE('');
    
    FOR i IN 1..K1.COUNT LOOP
        FOR j IN i+1..K1.COUNT LOOP
            IF K1(i).services IS NOT NULL AND K1(j).services IS NOT NULL THEN
                
                v_intersection_found := FALSE;
                v_common_services := '';
                
                FOR s IN 1..K1(i).services.COUNT LOOP
                    FOR t IN 1..K1(j).services.COUNT LOOP
                        IF K1(i).services(s).service_id = K1(j).services(t).service_id THEN
                            v_intersection_found := TRUE;
                            v_common_services := v_common_services || 
                                                K1(i).services(s).service_id || ' ';
                        END IF;
                    END LOOP;
                END LOOP;
                
                IF v_intersection_found THEN
                    DBMS_OUTPUT.PUT_LINE('  ПЕРЕСЕЧЕНИЕ НАЙДЕНО:');
                    DBMS_OUTPUT.PUT_LINE('  Заказ ' || K1(i).order_id || ' (' || K1(i).customer_name || ')');
                    DBMS_OUTPUT.PUT_LINE('  Заказ ' || K1(j).order_id || ' (' || K1(j).customer_name || ')');
                    DBMS_OUTPUT.PUT_LINE('  Общие услуги (service_id): ' || v_common_services);
                    DBMS_OUTPUT.PUT_LINE('');
                END IF;
            END IF;
        END LOOP;
    END LOOP;
END;
/



--2c
DECLARE
    K1 k1_type := k1_type();
    
    CURSOR c_orders IS
        SELECT order_id, customer_name, order_date, services
        FROM t1;
    
    v_order_id      t1.order_id%TYPE;
    v_customer_name t1.customer_name%TYPE;
    v_order_date    t1.order_date%TYPE;
    v_services      k2_type;
    
    v_test_element t1_type := t1_type(99, 'Тестовый заказ', SYSDATE, k2_type());
    v_is_member BOOLEAN := FALSE;
BEGIN
    OPEN c_orders;
    LOOP
        FETCH c_orders INTO v_order_id, v_customer_name, v_order_date, v_services;
        EXIT WHEN c_orders%NOTFOUND;
        K1.EXTEND;
        K1(K1.COUNT) := t1_type(v_order_id, v_customer_name, v_order_date, v_services);
    END LOOP;
    CLOSE c_orders;
    
    DBMS_OUTPUT.PUT_LINE('Проверяемый элемент: order_id=' || v_test_element.order_id || 
                         ', customer=' || v_test_element.customer_name);
    
    FOR i IN 1..K1.COUNT LOOP
        IF K1(i).order_id = v_test_element.order_id THEN
            v_is_member := TRUE;
            EXIT;
        END IF;
    END LOOP;
    
    IF v_is_member THEN
        DBMS_OUTPUT.PUT_LINE('РЕЗУЛЬТАТ: Элемент ЯВЛЯЕТСЯ членом коллекции K1');
    ELSE
        DBMS_OUTPUT.PUT_LINE('РЕЗУЛЬТАТ: Элемент НЕ ЯВЛЯЕТСЯ членом коллекции K1');
    END IF;
END;
/



--2d
DECLARE
    K1 k1_type := k1_type();
    
    CURSOR c_orders IS
        SELECT order_id, customer_name, order_date, services
        FROM t1;
    
    v_order_id      t1.order_id%TYPE;
    v_customer_name t1.customer_name%TYPE;
    v_order_date    t1.order_date%TYPE;
    v_services      k2_type;
    
    v_found BOOLEAN := FALSE;
BEGIN
    OPEN c_orders;
    LOOP
        FETCH c_orders INTO v_order_id, v_customer_name, v_order_date, v_services;
        EXIT WHEN c_orders%NOTFOUND;
        K1.EXTEND;
        K1(K1.COUNT) := t1_type(v_order_id, v_customer_name, v_order_date, v_services);
    END LOOP;
    CLOSE c_orders;
    
    DBMS_OUTPUT.PUT_LINE('Поиск пустых коллекций (заказов без услуг):');
    
    FOR i IN 1..K1.COUNT LOOP
        IF K1(i).services IS NULL OR K1(i).services.COUNT = 0 THEN
            DBMS_OUTPUT.PUT_LINE('  ЗАКАЗ ' || K1(i).order_id || ' (' || K1(i).customer_name || 
                                 ') - коллекция K2 ПУСТАЯ');
            v_found := TRUE;
        END IF;
    END LOOP;
    
    IF NOT v_found THEN
        DBMS_OUTPUT.PUT_LINE('  Пустых коллекций не найдено');
    END IF;
END;
/



--2е
DECLARE
    K1 k1_type := k1_type();
    
    CURSOR c_orders IS
        SELECT order_id, customer_name, order_date, services
        FROM t1;
    
    v_order_id      t1.order_id%TYPE;
    v_customer_name t1.customer_name%TYPE;
    v_order_date    t1.order_date%TYPE;
    v_services      k2_type;
    
    v_temp_services k2_type;
BEGIN
    OPEN c_orders;
    LOOP
        FETCH c_orders INTO v_order_id, v_customer_name, v_order_date, v_services;
        EXIT WHEN c_orders%NOTFOUND;
        K1.EXTEND;
        K1(K1.COUNT) := t1_type(v_order_id, v_customer_name, v_order_date, v_services);
    END LOOP;
    CLOSE c_orders;
    
    DBMS_OUTPUT.PUT_LINE('Обмен услугами (K2) между заказом 1 и заказом 2:');
    DBMS_OUTPUT.PUT_LINE('  ДО обмена:');
    DBMS_OUTPUT.PUT_LINE('    Заказ ' || K1(1).order_id || ' (' || K1(1).customer_name || 
                         ') - услуг: ' || CASE WHEN K1(1).services IS NOT NULL THEN K1(1).services.COUNT ELSE 0 END);
    DBMS_OUTPUT.PUT_LINE('    Заказ ' || K1(2).order_id || ' (' || K1(2).customer_name || 
                         ') - услуг: ' || CASE WHEN K1(2).services IS NOT NULL THEN K1(2).services.COUNT ELSE 0 END);
    
    v_temp_services := K1(1).services;
    K1(1).services := K1(2).services;
    K1(2).services := v_temp_services;
    
    DBMS_OUTPUT.PUT_LINE('  ПОСЛЕ обмена:');
    DBMS_OUTPUT.PUT_LINE('    Заказ ' || K1(1).order_id || ' (' || K1(1).customer_name || 
                         ') - услуг: ' || CASE WHEN K1(1).services IS NOT NULL THEN K1(1).services.COUNT ELSE 0 END);
    DBMS_OUTPUT.PUT_LINE('    Заказ ' || K1(2).order_id || ' (' || K1(2).customer_name || 
                         ') - услуг: ' || CASE WHEN K1(2).services IS NOT NULL THEN K1(2).services.COUNT ELSE 0 END);
    
    DBMS_OUTPUT.PUT_LINE('  Обмен успешно выполнен!');
END;
/



--3
DECLARE
    K1 k1_type := k1_type();
    
    CURSOR c_orders IS
        SELECT order_id, customer_name, order_date, services
        FROM t1;
    
    v_order_id      t1.order_id%TYPE;
    v_customer_name t1.customer_name%TYPE;
    v_order_date    t1.order_date%TYPE;
    v_services      k2_type;
BEGIN
    OPEN c_orders;
    LOOP
        FETCH c_orders INTO v_order_id, v_customer_name, v_order_date, v_services;
        EXIT WHEN c_orders%NOTFOUND;
        K1.EXTEND;
        K1(K1.COUNT) := t1_type(v_order_id, v_customer_name, v_order_date, v_services);
    END LOOP;
    CLOSE c_orders;
    
    DBMS_OUTPUT.PUT_LINE('Преобразование K1 в реляционный вид (плоская таблица):');
    DBMS_OUTPUT.PUT_LINE('ORDER_ID | CUSTOMER_NAME | SERVICE_NAME | PRICE');
    DBMS_OUTPUT.PUT_LINE('--------|---------------|--------------|-------');
    
    FOR i IN 1..K1.COUNT LOOP
        IF K1(i).services IS NOT NULL AND K1(i).services.COUNT > 0 THEN
            FOR j IN 1..K1(i).services.COUNT LOOP
                DBMS_OUTPUT.PUT_LINE('   ' || K1(i).order_id || '      | ' || 
                                     RPAD(K1(i).customer_name, 13) || ' | ' ||
                                     RPAD(K1(i).services(j).service_name, 12) || ' | ' ||
                                     K1(i).services(j).price);
            END LOOP;
        ELSE
            DBMS_OUTPUT.PUT_LINE('   ' || K1(i).order_id || '      | ' || 
                                 RPAD(K1(i).customer_name, 13) || ' | (нет услуг)    | 0');
        END IF;
    END LOOP;
    
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '--- Также преобразование K1 к другому типу коллекции (массив строк):');
    DECLARE
        TYPE string_array IS TABLE OF VARCHAR2(200) INDEX BY PLS_INTEGER;
        v_converted string_array;
    BEGIN
        FOR i IN 1..K1.COUNT LOOP
            v_converted(i) := 'Заказ ' || K1(i).order_id || ': ' || K1(i).customer_name;
        END LOOP;
        
        FOR i IN 1..v_converted.COUNT LOOP
            DBMS_OUTPUT.PUT_LINE('  ' || v_converted(i));
        END LOOP;
    END;
END;
/



--4
DECLARE
    TYPE id_array IS TABLE OF t1.order_id%TYPE INDEX BY PLS_INTEGER;
    TYPE name_array IS TABLE OF t1.customer_name%TYPE INDEX BY PLS_INTEGER;
    TYPE date_array IS TABLE OF t1.order_date%TYPE INDEX BY PLS_INTEGER;
    TYPE service_array IS TABLE OF k2_type INDEX BY PLS_INTEGER;
    
    v_ids id_array;
    v_names name_array;
    v_dates date_array;
    v_services service_array;
    
    K1 k1_type := k1_type();
    
    CURSOR c_orders IS
        SELECT order_id, customer_name, order_date, services
        FROM t1;
        
    CURSOR c_k1 IS
        SELECT t1_type(order_id, customer_name, order_date, services)
        FROM t1;
BEGIN
    DBMS_OUTPUT.PUT_LINE('========== BULK ОПЕРАЦИИ ==========');
    
    -- 2. BULK COLLECT - массовая загрузка в ассоциативные массивы
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '1. BULK COLLECT (массовая загрузка):');
    OPEN c_orders;
    FETCH c_orders BULK COLLECT INTO v_ids, v_names, v_dates, v_services;
    CLOSE c_orders;
    
    DBMS_OUTPUT.PUT_LINE('   Загружено ' || v_ids.COUNT || ' записей');
    FOR i IN 1..v_ids.COUNT LOOP
        DBMS_OUTPUT.PUT_LINE('     Заказ ' || v_ids(i) || ': ' || v_names(i));
    END LOOP;
    
    -- 3. BULK COLLECT с LIMIT (постраничная загрузка)
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '2. BULK COLLECT с LIMIT 2 (постранично):');
    OPEN c_orders;
    LOOP
        FETCH c_orders BULK COLLECT INTO v_ids, v_names, v_dates, v_services LIMIT 2;
        EXIT WHEN v_ids.COUNT = 0;
        DBMS_OUTPUT.PUT_LINE('   Страница: ' || v_ids.COUNT || ' записей');
        FOR i IN 1..v_ids.COUNT LOOP
            DBMS_OUTPUT.PUT_LINE('     Заказ ' || v_ids(i));
        END LOOP;
    END LOOP;
    CLOSE c_orders;
    
    -- 4. BULK COLLECT в коллекцию пользовательского типа K1
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '3. BULK COLLECT в коллекцию K1:');
    OPEN c_k1;
    FETCH c_k1 BULK COLLECT INTO K1;
    CLOSE c_k1;
    
    DBMS_OUTPUT.PUT_LINE('   Загружено ' || K1.COUNT || ' элементов в K1');
    FOR i IN 1..K1.COUNT LOOP
        DBMS_OUTPUT.PUT_LINE('     K1(' || i || '): заказ ' || K1(i).order_id);
    END LOOP;
    
    -- 5. FORALL с массовой вставкой
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '4. FORALL (массовая вставка):');
    DECLARE
        TYPE local_table_type IS TABLE OF VARCHAR2(100) INDEX BY PLS_INTEGER;
        v_data local_table_type;
        
        TYPE forall_id IS TABLE OF NUMBER INDEX BY PLS_INTEGER;
        TYPE forall_text IS TABLE OF VARCHAR2(100) INDEX BY PLS_INTEGER;
        v_forall_ids forall_id;
        v_forall_texts forall_text;
    BEGIN
        v_data(1) := 'Данные 1';
        v_data(2) := 'Данные 2';
        v_data(3) := 'Данные 3';
        
        FOR i IN 1..v_data.COUNT LOOP
            v_forall_ids(i) := i;
            v_forall_texts(i) := v_data(i);
        END LOOP;
        
        DBMS_OUTPUT.PUT_LINE('   Подготовлено ' || v_forall_ids.COUNT || ' записей для массовой вставки');
        DBMS_OUTPUT.PUT_LINE('   Синтаксис: FORALL i IN 1..collection.COUNT');
        DBMS_OUTPUT.PUT_LINE('              INSERT INTO table VALUES (collection1(i), collection2(i))');
        
        FOR i IN 1..v_forall_ids.COUNT LOOP
            DBMS_OUTPUT.PUT_LINE('     Вставка: ID=' || v_forall_ids(i) || ', text=' || v_forall_texts(i));
        END LOOP;
    END;
    
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '5. Вложенные коллекции (K2):');
    FOR i IN 1..K1.COUNT LOOP
        IF K1(i).services IS NOT NULL THEN
            DBMS_OUTPUT.PUT_LINE('     Заказ ' || K1(i).order_id || ': услуг=' || K1(i).services.COUNT);
        END IF;
    END LOOP;
    
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '========== BULK ОПЕРАЦИИ ВЫПОЛНЕНЫ ==========');
END;
/



SELECT '=== Таблица t1 (заказы) ===' AS info FROM DUAL;
SELECT order_id, customer_name, order_date FROM t1;

SELECT '=== Таблица t2 (услуги) ===' AS info FROM DUAL;
SELECT * FROM t2;