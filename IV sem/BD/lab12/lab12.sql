SET SERVEROUTPUT ON SIZE 1000000;

--1
ALTER TABLE TEACHER ADD (BIRTHDAY DATE, SALARY NUMBER);

UPDATE TEACHER SET BIRTHDAY = TO_DATE('15-06-1980', 'DD-MM-YYYY'), SALARY = 50000 WHERE TEACHER = 'Иванов';
UPDATE TEACHER SET BIRTHDAY = TO_DATE('22-11-1976', 'DD-MM-YYYY'), SALARY = 55000 WHERE TEACHER = 'Петров';
UPDATE TEACHER SET BIRTHDAY = TO_DATE('08-03-1982', 'DD-MM-YYYY'), SALARY = 60000 WHERE TEACHER = 'Сидоров';
UPDATE TEACHER SET BIRTHDAY = TO_DATE('30-09-1978', 'DD-MM-YYYY'), SALARY = 45000 WHERE TEACHER = 'Кузнецова';
UPDATE TEACHER SET BIRTHDAY = TO_DATE('15-07-1985', 'DD-MM-YYYY'), SALARY = 52000 WHERE TEACHER = 'Смирнова';
COMMIT;

--2
SELECT TEACHER_NAME AS "Фамилия И.О."
FROM TEACHER
WHERE TO_CHAR(BIRTHDAY, 'D') IN ('1', '2');

--3
CREATE VIEW NEXT_MONTH_BIRTHDAYS AS
SELECT TEACHER_NAME, TO_CHAR(BIRTHDAY, 'DD/MM/YYYY') AS BIRTHDAY_FORMATTED
FROM TEACHER
WHERE TO_CHAR(BIRTHDAY, 'MM') = TO_CHAR(ADD_MONTHS(SYSDATE, 1), 'MM');

SELECT * FROM next_month_birthdays;

--4
CREATE VIEW TEACHERS_BY_MONTH AS
SELECT TO_CHAR(BIRTHDAY, 'Month', 'NLS_DATE_LANGUAGE=RUSSIAN') AS MONTH_NAME, COUNT(*) AS TEACHER_COUNT
FROM TEACHER
GROUP BY TO_CHAR(BIRTHDAY, 'Month', 'NLS_DATE_LANGUAGE=RUSSIAN');

SELECT * FROM TEACHERS_BY_MONTH;

--5
DECLARE
    CURSOR jubilee_teachers IS
        SELECT TEACHER_NAME, EXTRACT(YEAR FROM SYSDATE) - EXTRACT(YEAR FROM BIRTHDAY) + 1 AS AGE
        FROM TEACHER
        WHERE MOD(EXTRACT(YEAR FROM SYSDATE) - EXTRACT(YEAR FROM BIRTHDAY) + 1, 10) = 0;
BEGIN
    FOR teacher IN jubilee_teachers LOOP
        DBMS_OUTPUT.PUT_LINE(teacher.TEACHER_NAME || ' - ' || teacher.AGE || ' лет');
    END LOOP;
END;

--6
DECLARE
    CURSOR avg_salary_by_department IS
        SELECT PULPIT, FLOOR(AVG(SALARY)) AS AVG_SALARY
        FROM TEACHER
        GROUP BY PULPIT;
BEGIN
    FOR dept IN avg_salary_by_department LOOP
        DBMS_OUTPUT.PUT_LINE(dept.PULPIT || ' - ' || dept.AVG_SALARY);
    END LOOP;
END;

--7
DECLARE
    numerator NUMBER := 10;
    denominator NUMBER := 0;
    result NUMBER;
BEGIN
    IF denominator = 0 THEN
        DBMS_OUTPUT.PUT_LINE('Ошибка: Деление на ноль не допускается');
    ELSE
        result := numerator / denominator;
        DBMS_OUTPUT.PUT_LINE('Результат: ' || result);
    END IF;
EXCEPTION
    WHEN ZERO_DIVIDE THEN
        DBMS_OUTPUT.PUT_LINE('Ошибка: Деление на ноль');
END;

--8
DECLARE
    teacher_name VARCHAR2(100);
    teacher_code TEACHER.TEACHER%TYPE := 'Несущ';
BEGIN
    SELECT TEACHER_NAME INTO teacher_name
    FROM TEACHER
    WHERE TEACHER = teacher_code;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        DBMS_OUTPUT.PUT_LINE('Преподаватель не найден!');
END;

--9
DECLARE
    my_exception EXCEPTION;
    PRAGMA EXCEPTION_INIT(my_exception, -20001);
BEGIN
    DECLARE
        nested_exception EXCEPTION;
        PRAGMA EXCEPTION_INIT(nested_exception, -20001);
    BEGIN
        RAISE nested_exception;
    EXCEPTION
        WHEN nested_exception THEN
            DBMS_OUTPUT.PUT_LINE('Исключение во вложенном блоке');
            RAISE;
    END;
EXCEPTION
    WHEN my_exception THEN
        DBMS_OUTPUT.PUT_LINE('Исключение в основном блоке');
END;

--10
DECLARE
    max_salary NUMBER;
BEGIN
    SELECT MAX(SALARY) INTO max_salary
    FROM TEACHER;
    DBMS_OUTPUT.PUT_LINE('Максимальная зарплата: ' || max_salary);
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        DBMS_OUTPUT.PUT_LINE('Данные не найдены');
END;
