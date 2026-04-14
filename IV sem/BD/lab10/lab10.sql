-- Создание таблицы FACULTY (Факультет)
CREATE TABLE FACULTY (
    FACULTY VARCHAR2(50) NOT NULL,
    FACULTY_NAME VARCHAR2(100) NOT NULL,
    CONSTRAINT PK_FACULTY PRIMARY KEY (FACULTY)
);

-- Создание таблицы PULPIT (Кафедра)
CREATE TABLE PULPIT (
    PULPIT VARCHAR2(50) NOT NULL,
    PULPIT_NAME VARCHAR2(100) NOT NULL,
    FACULTY VARCHAR2(50) NOT NULL,
    CONSTRAINT PK_PULPIT PRIMARY KEY (PULPIT),
    CONSTRAINT FK_PULPIT_FACULTY FOREIGN KEY (FACULTY) 
        REFERENCES FACULTY(FACULTY)
);

-- Создание таблицы TEACHER (Преподаватель)
CREATE TABLE TEACHER (
    TEACHER VARCHAR2(50) NOT NULL,
    TEACHER_NAME VARCHAR2(100) NOT NULL,
    PULPIT VARCHAR2(50) NOT NULL,
    CONSTRAINT PK_TEACHER PRIMARY KEY (TEACHER),
    CONSTRAINT FK_TEACHER_PULPIT FOREIGN KEY (PULPIT) 
        REFERENCES PULPIT(PULPIT)
);

-- Создание таблицы AUDITORIUM_TYPE (Тип аудитории)
CREATE TABLE AUDITORIUM_TYPE (
    AUDITORIUM_TYPE VARCHAR2(50) NOT NULL,
    AUDITORIUM_TYPENAME VARCHAR2(100) NOT NULL,
    CONSTRAINT PK_AUDITORIUM_TYPE PRIMARY KEY (AUDITORIUM_TYPE)
);

-- Создание таблицы AUDITORIUM (Аудитория)
CREATE TABLE AUDITORIUM (
    AUDITORIUM VARCHAR2(50) NOT NULL,
    AUDITORIUM_NAME VARCHAR2(100) NOT NULL,
    AUDITORIUM_CAPACITY NUMBER NOT NULL,
    AUDITORIUM_TYPE VARCHAR2(50) NOT NULL,
    CONSTRAINT PK_AUDITORIUM PRIMARY KEY (AUDITORIUM),
    CONSTRAINT FK_AUDITORIUM_TYPE FOREIGN KEY (AUDITORIUM_TYPE) 
        REFERENCES AUDITORIUM_TYPE(AUDITORIUM_TYPE)
);

-- Создание таблицы SUBJECT (Дисциплина)
CREATE TABLE SUBJECT (
    SUBJECT VARCHAR2(50) NOT NULL,
    SUBJECT_NAME VARCHAR2(100) NOT NULL,
    PULPIT VARCHAR2(50) NOT NULL,
    CONSTRAINT PK_SUBJECT PRIMARY KEY (SUBJECT),
    CONSTRAINT FK_SUBJECT_PULPIT FOREIGN KEY (PULPIT) 
        REFERENCES PULPIT(PULPIT)
);

-- Заполнение таблицы FACULTY
INSERT INTO FACULTY (FACULTY, FACULTY_NAME) VALUES ('ФКН', 'Факультет компьютерных наук');
INSERT INTO FACULTY (FACULTY, FACULTY_NAME) VALUES ('ФЭН', 'Факультет экономики');
INSERT INTO FACULTY (FACULTY, FACULTY_NAME) VALUES ('ФИЯ', 'Факультет иностранных языков');

-- Заполнение таблицы PULPIT
INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES ('ИСиТ', 'Информационные системы и технологии', 'ФКН');
INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES ('ПОИТ', 'Программное обеспечение информационных технологий', 'ФКН');
INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES ('ЭК', 'Экономика', 'ФЭН');
INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES ('БИ', 'Бухгалтерский учет', 'ФЭН');
INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES ('ЛН', 'Лингвистика', 'ФИЯ');

-- Заполнение таблицы TEACHER
INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT) VALUES ('Иванов', 'Иван Иванович Иванов', 'ИСиТ');
INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT) VALUES ('Петров', 'Петр Петрович Петров', 'ПОИТ');
INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT) VALUES ('Сидоров', 'Сидор Сидорович Сидоров', 'ЭК');
INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT) VALUES ('Кузнецова', 'Анна Васильевна Кузнецова', 'БИ');
INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT) VALUES ('Смирнова', 'Елена Александровна Смирнова', 'ЛН');

-- Заполнение таблицы AUDITORIUM_TYPE
INSERT INTO AUDITORIUM_TYPE (AUDITORIUM_TYPE, AUDITORIUM_TYPENAME) VALUES ('ЛК', 'Лекционная');
INSERT INTO AUDITORIUM_TYPE (AUDITORIUM_TYPE, AUDITORIUM_TYPENAME) VALUES ('ЛБ', 'Лаборатория');
INSERT INTO AUDITORIUM_TYPE (AUDITORIUM_TYPE, AUDITORIUM_TYPENAME) VALUES ('СР', 'Семинарная');

-- Заполнение таблицы AUDITORIUM
INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY, AUDITORIUM_TYPE) VALUES ('101-1', 'Аудитория 101 корпус 1', 50, 'ЛК');
INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY, AUDITORIUM_TYPE) VALUES ('102-1', 'Аудитория 102 корпус 1', 30, 'СР');
INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY, AUDITORIUM_TYPE) VALUES ('201-1', 'Аудитория 201 корпус 1', 25, 'ЛБ');
INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY, AUDITORIUM_TYPE) VALUES ('301-2', 'Аудитория 301 корпус 2', 70, 'ЛК');
INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY, AUDITORIUM_TYPE) VALUES ('202-2', 'Аудитория 202 корпус 2', 20, 'ЛБ');

-- Заполнение таблицы SUBJECT
INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES ('БД', 'Базы данных', 'ИСиТ');
INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES ('ПО', 'Программное обеспечение', 'ПОИТ');
INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES ('ЭТ', 'Экономическая теория', 'ЭК');
INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES ('БУ', 'Бухгалтерский учет', 'БИ');
INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES ('ЛГ', 'Лингвистика', 'ЛН');

COMMIT;



 --drop table faculty; drop table pulpit; drop table teacher; drop table auditorium_type; drop table auditorium; drop table subject;
 
 SET SERVEROUTPUT ON;
--1
BEGIN
  NULL;
END;
/

--2
BEGIN
  DBMS_OUTPUT.PUT_LINE('Hello World!');
END;
/

--3
SELECT * FROM V$RESERVED_WORDS WHERE RESERVED = 'Y' AND LENGTH(KEYWORD) = 1;

--4
SELECT * FROM V$RESERVED_WORDS WHERE RESERVED = 'Y';

--5
DECLARE
  v_num1 NUMBER := 10;
  v_num2 NUMBER := 3;
  v_result NUMBER;
  v_fixed1 NUMBER(5,2) := 123.45;
  v_fixed2 NUMBER(5,-2) := 12345.67;
  v_exp NUMBER := 1.5E2;
  v_date1 DATE := SYSDATE;
  v_date2 DATE := TO_DATE('2023-12-31', 'YYYY-MM-DD');
  v_char CHAR(10) := 'ABC';
  v_varchar VARCHAR2(100) := 'Пример текста';
  v_bool1 BOOLEAN := TRUE;
  v_bool2 BOOLEAN := FALSE;
  v_bool3 BOOLEAN := NULL;
BEGIN
  v_result := v_num1 + v_num2;
  DBMS_OUTPUT.PUT_LINE('Сумма: ' || v_result);
  v_result := v_num1 - v_num2;
  DBMS_OUTPUT.PUT_LINE('Разность: ' || v_result);
  v_result := v_num1 * v_num2;
  DBMS_OUTPUT.PUT_LINE('Произведение: ' || v_result);
  v_result := v_num1 / v_num2;
  DBMS_OUTPUT.PUT_LINE('Деление: ' || v_result);
  v_result := MOD(v_num1, v_num2);
  DBMS_OUTPUT.PUT_LINE('Остаток: ' || v_result);
  DBMS_OUTPUT.PUT_LINE('Фикс1: ' || v_fixed1);
  DBMS_OUTPUT.PUT_LINE('Фикс2: ' || v_fixed2);
  DBMS_OUTPUT.PUT_LINE('Эксп: ' || v_exp);
  DBMS_OUTPUT.PUT_LINE('Дата1: ' || TO_CHAR(v_date1, 'DD.MM.YYYY'));
  DBMS_OUTPUT.PUT_LINE('Дата2: ' || TO_CHAR(v_date2, 'DD.MM.YYYY'));
  DBMS_OUTPUT.PUT_LINE('CHAR: ' || v_char);
  DBMS_OUTPUT.PUT_LINE('VARCHAR2: ' || v_varchar);
  IF v_bool1 THEN DBMS_OUTPUT.PUT_LINE('bool1=TRUE'); END IF;
  IF NOT v_bool2 THEN DBMS_OUTPUT.PUT_LINE('bool2=FALSE'); END IF;
  IF v_bool3 IS NULL THEN DBMS_OUTPUT.PUT_LINE('bool3=NULL'); END IF;
END;
/

--6
DECLARE
  c_varchar CONSTANT VARCHAR2(100) := 'Константа VARCHAR2';
  c_char CONSTANT CHAR(100) := 'CHAR';
  c_number CONSTANT NUMBER := 3.14159;
BEGIN
  DBMS_OUTPUT.PUT_LINE(c_varchar);
  DBMS_OUTPUT.PUT_LINE(c_char);
  DBMS_OUTPUT.PUT_LINE('Число Пи: ' || c_number);
  DBMS_OUTPUT.PUT_LINE('Пи^2: ' || (c_number * c_number));
  DBMS_OUTPUT.PUT_LINE('Длина: ' || LENGTH(c_varchar));
END;
/

--7
DECLARE
  v_teacher_id TEACHER.TEACHER%TYPE;
BEGIN
  BEGIN
    SELECT TEACHER INTO v_teacher_id FROM TEACHER WHERE ROWNUM = 1;
    DBMS_OUTPUT.PUT_LINE('ID преподавателя: ' || v_teacher_id);
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      DBMS_OUTPUT.PUT_LINE('Таблица TEACHER пуста или не существует');
  END;
END;
/

--8
DECLARE
  v_teacher_row TEACHER%ROWTYPE;
BEGIN
  BEGIN
    SELECT * INTO v_teacher_row FROM TEACHER WHERE ROWNUM = 1;
    DBMS_OUTPUT.PUT_LINE('Преподаватель: ' || v_teacher_row.TEACHER_NAME || 
                        ', Кафедра: ' || v_teacher_row.PULPIT);
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      DBMS_OUTPUT.PUT_LINE('Таблица TEACHER пуста или не существует');
  END;
END;
/

--9
DECLARE
  v_num NUMBER := 10;
BEGIN
  IF v_num > 0 THEN 
    DBMS_OUTPUT.PUT_LINE('Положительное'); 
  END IF;
  
  IF v_num MOD 2 = 0 THEN 
    DBMS_OUTPUT.PUT_LINE('Четное'); 
  ELSE 
    DBMS_OUTPUT.PUT_LINE('Нечетное'); 
  END IF;
  
  IF v_num < 0 THEN 
    DBMS_OUTPUT.PUT_LINE('Отрицательное');
  ELSIF v_num = 0 THEN 
    DBMS_OUTPUT.PUT_LINE('Ноль');
  ELSE 
    DBMS_OUTPUT.PUT_LINE('Положительное'); 
  END IF;
END;
/

--10
DECLARE
  v_grade CHAR(1) := 'B';
  v_result VARCHAR2(100);
BEGIN
  v_result := CASE v_grade
                WHEN 'A' THEN 'Отлично'
                WHEN 'B' THEN 'Хорошо'
                WHEN 'C' THEN 'Удовлетворительно'
                ELSE 'Неудовлетворительно' 
              END;
  DBMS_OUTPUT.PUT_LINE('Оценка: ' || v_result);
  
  v_result := CASE
                WHEN v_grade = 'A' THEN '5'
                WHEN v_grade = 'B' THEN '4'
                WHEN v_grade = 'C' THEN '3'
                ELSE '2' 
              END;
  DBMS_OUTPUT.PUT_LINE('Балл: ' || v_result);
END;
/

--11
DECLARE
  v_counter NUMBER := 1;
BEGIN
  LOOP
    DBMS_OUTPUT.PUT_LINE('Итерация: ' || v_counter);
    v_counter := v_counter + 1;
    EXIT WHEN v_counter > 5;
  END LOOP;
END;
/

--12
DECLARE
  v_counter NUMBER := 1;
BEGIN
  WHILE v_counter <= 5 LOOP
    DBMS_OUTPUT.PUT_LINE('Итерация: ' || v_counter);
    v_counter := v_counter + 1;
  END LOOP;
END;
/

--13
BEGIN
  FOR i IN 1..5 LOOP
    DBMS_OUTPUT.PUT_LINE('Итерация: ' || i);
  END LOOP;
  
  FOR i IN REVERSE 1..5 LOOP
    DBMS_OUTPUT.PUT_LINE('Обратно: ' || i);
  END LOOP;
END;
/