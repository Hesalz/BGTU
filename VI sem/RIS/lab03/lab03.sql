CREATE USER Hleb IDENTIFIED BY 1234
DEFAULT TABLESPACE USERS
QUOTA UNLIMITED ON USERS
TEMPORARY TABLESPACE TEMP;

GRANT CREATE SESSION TO Hleb;
GRANT CREATE TABLE TO Hleb;
GRANT UNLIMITED TABLESPACE TO Hleb;  
GRANT CONNECT TO Hleb;
GRANT CREATE DATABASE LINK TO Hleb;

-- drop user Hleb cascade; 

CREATE DATABASE LINK linkr
CONNECT TO Kolya IDENTIFIED BY "1234"
USING '(DESCRIPTION=
         (ADDRESS=(PROTOCOL=TCP)(HOST=172.20.10.3)(PORT=1521))
         (CONNECT_DATA=(SERVICE_NAME=ORCLPDB)))'; 

DROP DATABASE LINK linkr;
show con_name;

DROP TABLE local_table;
DROP TABLE remote_table CASCADE CONSTRAINTS PURGE;

CREATE TABLE local_table (
    id NUMBER PRIMARY KEY,
    data VARCHAR2(100),
    status VARCHAR2(20)
);

CREATE TABLE remote_table (
    id NUMBER PRIMARY KEY,
    data VARCHAR2(100),
    status VARCHAR2(20)
);


SELECT * FROM DUAL@linkr;

select * from local_table;
select * from remote_table;
select * from remote_table@linkr;

--6.1
DELETE FROM local_table;
COMMIT;
SET TRANSACTION NAME 'txn_insert_insert';
INSERT INTO local_table VALUES (200, 'Start', 'NEW');
INSERT INTO remote_table@linkr VALUES (200, 'Data from hleb', 'NEW');
COMMIT;

--6.2
begin
INSERT INTO local_table VALUES (5, 'Fifth row', 'PENDING');
UPDATE remote_table@linkr SET value='Updated by hleb', data='Y' WHERE id=200;
COMMIT;
end;

--6.3
begin
UPDATE local_table SET status='PROCESSED' WHERE id=200;
INSERT INTO remote_table@linkr VALUES (200, 'New remote row', 'N');
COMMIT;
end;

--7.1
DELETE FROM remote_table@linkr WHERE id=101;
COMMIT;

begin
-- Этот блок выдаст ошибку (дубликат ключа 100)
INSERT INTO local_table VALUES (101, 'Will be rolled back', 'ERROR');
INSERT INTO remote_table@linkr VALUES (200, 'DUPLICATE KEY!', 'X');
COMMIT;
end;



--
-- Подключаешься как SYS или SYSTEM
GRANT EXECUTE ON SYS.DBMS_LOCK TO Hleb;
SET SERVEROUTPUT ON;


DECLARE -- для 1-ого компа
   v_lock_handle VARCHAR2(128);
   v_result NUMBER;
BEGIN
   DBMS_OUTPUT.PUT_LINE('TXN B (удаленный комп) НАЧАЛО');
   DBMS_OUTPUT.PUT_LINE('Время: ' || TO_CHAR(SYSDATE, 'HH24:MI:SS'));
   
   -- Создаем именованную блокировку
   DBMS_LOCK.ALLOCATE_UNIQUE('MY_TEST_LOCK', v_lock_handle);
   
   -- Захватываем эксклюзивную блокировку
   v_result := DBMS_LOCK.REQUEST(
      lockhandle => v_lock_handle,
      lockmode => DBMS_LOCK.X_MODE,
      timeout => 0,
      release_on_commit => TRUE
   );
   
   IF v_result = 0 THEN
      -- Обновляем строку
      UPDATE remote_table
      SET data = 'BLOCKED BY KOLYA', status = 'Y' 
      WHERE id = 2;
      
      
      DBMS_OUTPUT.PUT_LINE('TXN B: Строка обновлена, блокировка DBMS_LOCK удерживается');
      DBMS_OUTPUT.PUT_LINE('TXN B: Ждем 15 секунд...');
      
      -- Держим блокировку 15 секунд
      DBMS_LOCK.SLEEP(15);
      
      -- Фиксируем изменения
      COMMIT;
      DBMS_OUTPUT.PUT_LINE('TXN B: COMMIT выполнен, блокировка снята');
   ELSE
      DBMS_OUTPUT.PUT_LINE('TXN B: ОШИБКА - не удалось получить блокировку');
   END IF;
   
   DBMS_OUTPUT.PUT_LINE('TXN B КОНЕЦ');
   DBMS_OUTPUT.PUT_LINE('Время: ' || TO_CHAR(SYSDATE, 'HH24:MI:SS'));
END;
/


------------


DECLARE --для 2-ого компа
   v_start DATE := SYSDATE;
BEGIN
   DBMS_OUTPUT.PUT_LINE('Время: ' || TO_CHAR(SYSDATE, 'HH24:MI:SS'));
   DBMS_OUTPUT.PUT_LINE('TXN A: Пытаемся обновить строку через dblink');
   
   UPDATE remote_table@linkr 
   SET data = 'UPDATED BY TXN A', status = 'X' 
   WHERE id = 2;
   
   DBMS_OUTPUT.PUT_LINE('TXN A: Обновление выполнено! Ожидание заняло '  
                        || ROUND((SYSDATE - v_start) * 86400) || ' секунд');
   
   UPDATE local_table SET status = 'UPDATED BY TXN A' WHERE id = 1;
   
   COMMIT;
   DBMS_OUTPUT.PUT_LINE('TXN A: COMMIT выполнен');
   DBMS_OUTPUT.PUT_LINE('Время: ' || TO_CHAR(SYSDATE, 'HH24:MI:SS'));
END;
/