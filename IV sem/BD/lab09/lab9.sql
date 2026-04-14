--1
ALTER SESSION SET CONTAINER = ORCLPDB;

CREATE USER HBA IDENTIFIED BY password
DEFAULT TABLESPACE USERS
TEMPORARY TABLESPACE TEMP
QUOTA UNLIMITED ON USERS;

GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE, CREATE CLUSTER, 
      CREATE VIEW, CREATE MATERIALIZED VIEW, CREATE DATABASE LINK,
      CREATE SYNONYM, CREATE PUBLIC SYNONYM TO HBA;
      
--2
CREATE GLOBAL TEMPORARY TABLE HBA_TEMP_TABLE (
    id NUMBER,
    name VARCHAR2(50)
) ON COMMIT PRESERVE ROWS;

INSERT INTO HBA_TEMP_TABLE VALUES (1, 'Временные данные');
COMMIT;

SELECT * FROM HBA_TEMP_TABLE;

--3
CREATE SEQUENCE HBA_S1
START WITH 1000
INCREMENT BY 10
NOMINVALUE
NOMAXVALUE
NOCYCLE
NOCACHE
NOORDER;

SELECT HBA_S1.NEXTVAL FROM DUAL;
SELECT HBA_S1.NEXTVAL FROM DUAL;
SELECT HBA_S1.CURRVAL FROM DUAL;

--4
SET SERVEROUTPUT ON SIZE 1000000;
SET LINESIZE 100;
SET PAGESIZE 100;

CREATE SEQUENCE HBA_S2
START WITH 10
INCREMENT BY 10
MAXVALUE 100
NOCYCLE;

BEGIN
  DBMS_OUTPUT.PUT_LINE('Получение всех значений последовательности HBA_S2:');
  FOR i IN 1..20 LOOP
    BEGIN
      DBMS_OUTPUT.PUT_LINE('Значение ' || i || ': ' || HBA_S2.NEXTVAL);
    EXCEPTION
      WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Ошибка на шаге ' || i || ': ' || SQLERRM);
        EXIT;
    END;
  END LOOP;
END;
/

BEGIN
  DBMS_OUTPUT.PUT_LINE('Попытка получить значение после достижения максимума:');
  DBMS_OUTPUT.PUT_LINE('Следующее значение: ' || HBA_S2.NEXTVAL);
EXCEPTION
  WHEN OTHERS THEN
    DBMS_OUTPUT.PUT_LINE('Ошибка: ' || SQLERRM);
END;
/

BEGIN
  DBMS_OUTPUT.PUT_LINE('Текущее значение после ошибки: ' || HBA_S2.CURRVAL);
EXCEPTION
  WHEN OTHERS THEN
    DBMS_OUTPUT.PUT_LINE('Ошибка при получении текущего значения: ' || SQLERRM);
END;
/

--5
CREATE SEQUENCE HBA_S3
START WITH 10
INCREMENT BY -10
MINVALUE -100
MAXVALUE 10
NOCYCLE
ORDER;

DECLARE
  v_value NUMBER;
  v_counter NUMBER := 1;
  v_max_attempts CONSTANT NUMBER := 20;
BEGIN
  DBMS_OUTPUT.PUT_LINE('====================================');
  DBMS_OUTPUT.PUT_LINE('Получение значений последовательности HBA_S3');
  DBMS_OUTPUT.PUT_LINE('====================================');
  
  WHILE v_counter <= v_max_attempts LOOP
    BEGIN
      v_value := HBA_S3.NEXTVAL;
      DBMS_OUTPUT.PUT_LINE('Значение '||v_counter||': '||v_value);
      v_counter := v_counter + 1;
    EXCEPTION
      WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('ОШИБКА: '||SQLERRM);
        EXIT;
    END;
  END LOOP;
  
  DBMS_OUTPUT.PUT_LINE('------------------------------------');
  DBMS_OUTPUT.PUT_LINE('Попытка получить значение после ошибки:');
  BEGIN
    v_value := HBA_S3.NEXTVAL;
    DBMS_OUTPUT.PUT_LINE('Следующее значение: '||v_value);
  EXCEPTION
    WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE('ОШИБКА: '||SQLERRM);
  END;
  
  DBMS_OUTPUT.PUT_LINE('------------------------------------');
  DBMS_OUTPUT.PUT_LINE('Текущее значение последовательности:');
  BEGIN
    DBMS_OUTPUT.PUT_LINE(HBA_S3.CURRVAL);
  EXCEPTION
    WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE('ОШИБКА: '||SQLERRM);
  END;
END;
/

--6
CREATE SEQUENCE HBA_S4
START WITH 10
INCREMENT BY 1
MINVALUE 10
MAXVALUE 15
CYCLE
CACHE 5
NOORDER;

SET SERVEROUTPUT ON;
DECLARE
  v_value NUMBER;
  v_counter NUMBER := 1;
  v_max_attempts CONSTANT NUMBER := 25;
BEGIN
  FOR v_counter IN 1..v_max_attempts LOOP
    v_value := HBA_S4.NEXTVAL;
    DBMS_OUTPUT.PUT_LINE('Шаг '||LPAD(v_counter,2)||': '||LPAD(v_value,2));
    
    FOR i IN 1..100000 LOOP
      NULL;
    END LOOP;
  END LOOP;
END;
/

--7
SELECT sequence_name
FROM all_sequences
WHERE sequence_owner = 'HBA';

--8
-- при возникновении ошибок просто пересоздайте каждый sequence
CREATE TABLE T1 (
  N1 NUMBER(20),
  N2 NUMBER(20),
  N3 NUMBER(20),
  N4 NUMBER(20)
)
CACHE
STORAGE (BUFFER_POOL KEEP);

BEGIN
  FOR i IN 1..7 LOOP
    INSERT INTO T1 (
      N1, N2, N3, N4
    ) VALUES (
      HBA_S1.NEXTVAL,
      HBA_S2.NEXTVAL,
      HBA_S3.NEXTVAL,
      HBA_S4.NEXTVAL
    );
  END LOOP;
  COMMIT;
END;
/

--9
CREATE CLUSTER ABC (
  X NUMBER(10),
  V VARCHAR2(12)
)
HASHKEYS 200;

--10
CREATE TABLE A (
  XA NUMBER(10),
  VA VARCHAR2(12),
  extra_col_a VARCHAR2(50)
)
CLUSTER ABC (XA, VA);

--11
CREATE TABLE B (
  XB NUMBER(10),
  VB VARCHAR2(12),
  extra_col_b VARCHAR2(50)
)
CLUSTER ABC (XB, VB);

--12
CREATE TABLE C (
  XC NUMBER(10),
  VC VARCHAR2(12),
  extra_col_c VARCHAR2(50)
)
CLUSTER ABC (XC, VC);

--13
SELECT table_name
FROM user_tables;

SELECT cluster_name
FROM user_clusters;

--14
CREATE SYNONYM C_ALIAS FOR C;
SELECT * FROM C_ALIAS;

--15
CREATE PUBLIC SYNONYM B_GLOBAL FOR HBA.B;
SELECT * FROM B_GLOBAL;

--16
CREATE TABLE A_MAIN (
  id NUMBER PRIMARY KEY,
  name VARCHAR2(50)
);

CREATE TABLE B_MAIN (
  id NUMBER PRIMARY KEY,
  a_id NUMBER,
  description VARCHAR2(100),
  CONSTRAINT fk_a FOREIGN KEY (a_id) REFERENCES A_MAIN(id)
);

INSERT INTO A_MAIN VALUES (1, 'Alpha');
INSERT INTO A_MAIN VALUES (2, 'Beta');

INSERT INTO B_MAIN VALUES (10, 1, 'Child of Alpha');
INSERT INTO B_MAIN VALUES (11, 2, 'Child of Beta');

COMMIT;

CREATE VIEW V1 AS
SELECT a.id AS A_ID, a.name, b.description
FROM A_MAIN a
INNER JOIN B_MAIN b ON a.id = b.a_id;

SELECT * FROM V1;

--17
CREATE MATERIALIZED VIEW MV_HBA
BUILD IMMEDIATE
REFRESH COMPLETE
START WITH SYSDATE
NEXT SYSDATE + INTERVAL '2' MINUTE
AS
SELECT a.id AS A_ID, a.name, b.description
FROM A_MAIN a
INNER JOIN B_MAIN b ON a.id = b.a_id;

SELECT * FROM MV_HBA;

--18
CREATE USER c##user2 IDENTIFIED BY password123;
GRANT CONNECT, RESOURCE TO c##user2;
GRANT CREATE DATABASE LINK TO c##user2;
ALTER USER c##user2 QUOTA UNLIMITED ON USERS;

--user2 connect
CREATE TABLE test_table (
    id NUMBER PRIMARY KEY,
    name VARCHAR2(100),
    created_date DATE DEFAULT SYSDATE,
    value NUMBER(10,2)
);
INSERT INTO test_table VALUES (1, 'Тест 1', SYSDATE, 100);
INSERT INTO test_table VALUES (2, 'Тест 2', SYSDATE, 200);
COMMIT;

-- обратно на основного пользователя

CREATE DATABASE LINK user1_to_user2
CONNECT TO c##user2 IDENTIFIED BY password123
USING 'orcl';

--19
SELECT * FROM test_table@user1_to_user2;

INSERT INTO test_table@user1_to_user2 (id, name, value)
VALUES (3, 'Новая запись', 150.50);
COMMIT;

SELECT * FROM test_table@user1_to_user2 ORDER BY id;

UPDATE test_table@user1_to_user2
SET value = value * 1.1
WHERE id = 1;
COMMIT;

SELECT * FROM test_table@user1_to_user2 WHERE id = 1;

DELETE FROM test_table@user1_to_user2
WHERE id = 2;
COMMIT;

SELECT * FROM test_table@user1_to_user2;

-- user2
CREATE OR REPLACE PROCEDURE update_all_values(p_factor NUMBER) IS
BEGIN
  UPDATE test_table SET value = value * p_factor;
  COMMIT;
END;
/

-- обратно на основу

BEGIN
  update_all_values@user1_to_user2(1.2);
  DBMS_OUTPUT.PUT_LINE('Процедура выполнена');
END;
/

SELECT * FROM test_table@user1_to_user2;

-- user2

CREATE OR REPLACE FUNCTION get_max_value RETURN NUMBER IS
  v_max NUMBER;
BEGIN
  SELECT MAX(value) INTO v_max FROM test_table;
  RETURN v_max;
END;
/

-- обратно основа
DECLARE
  v_max_val NUMBER;
BEGIN
  v_max_val := get_max_value@user1_to_user2();
  DBMS_OUTPUT.PUT_LINE('Максимальное значение: ' || v_max_val);
END;
/

DROP USER c##user2 CASCADE;