SET SERVEROUTPUT ON;

--1
DECLARE
    PROCEDURE GET_TEACHERS(PCODE TEACHER.PULPIT%TYPE) IS
    BEGIN
        DBMS_OUTPUT.PUT_LINE('Преподаватели кафедры ' || PCODE || ':');
        FOR teacher_rec IN (SELECT TEACHER_NAME FROM TEACHER WHERE PULPIT = PCODE) LOOP
            DBMS_OUTPUT.PUT_LINE(teacher_rec.TEACHER_NAME);
        END LOOP;
    END GET_TEACHERS;

BEGIN
    DBMS_OUTPUT.PUT_LINE('Демонстрация работы процедуры GET_TEACHERS:');
    GET_TEACHERS('ИСиТ');
    DBMS_OUTPUT.PUT_LINE('----------------------');
    GET_TEACHERS('ПОИТ');
END;

--2
DECLARE
    FUNCTION GET_NUM_TEACHERS(PCODE TEACHER.PULPIT%TYPE) RETURN NUMBER IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count FROM TEACHER WHERE PULPIT = PCODE;
        RETURN v_count;
    END GET_NUM_TEACHERS;

BEGIN
    DBMS_OUTPUT.PUT_LINE('Демонстрация работы функции GET_NUM_TEACHERS:');
    DBMS_OUTPUT.PUT_LINE('Количество преподавателей на кафедре ИСиТ: ' || GET_NUM_TEACHERS('ИСиТ'));
    DBMS_OUTPUT.PUT_LINE('Количество преподавателей на кафедре ПОИТ: ' || GET_NUM_TEACHERS('ПОИТ'));
    DBMS_OUTPUT.PUT_LINE('Количество преподавателей на кафедре ЛН: ' || GET_NUM_TEACHERS('ЛН'));
END;

--3
CREATE OR REPLACE PROCEDURE GET_TEACHERS(FCODE FACULTY.FACULTY%TYPE) IS
BEGIN
    DBMS_OUTPUT.PUT_LINE('Преподаватели факультета ' || FCODE || ':');
    FOR teacher_rec IN (
        SELECT t.TEACHER_NAME 
        FROM TEACHER t 
        JOIN PULPIT p ON t.PULPIT = p.PULPIT
        WHERE p.FACULTY = FCODE
    ) LOOP
        DBMS_OUTPUT.PUT_LINE(teacher_rec.TEACHER_NAME);
    END LOOP;
END;
/

BEGIN
    GET_TEACHERS('ФКН');
END;
/

CREATE OR REPLACE PROCEDURE GET_SUBJECTS(PCODE SUBJECT.PULPIT%TYPE) IS
BEGIN
    DBMS_OUTPUT.PUT_LINE('Дисциплины кафедры ' || PCODE || ':');
    FOR subject_rec IN (SELECT SUBJECT_NAME FROM SUBJECT WHERE PULPIT = PCODE) LOOP
        DBMS_OUTPUT.PUT_LINE(subject_rec.SUBJECT_NAME);
    END LOOP;
END;
/

BEGIN
    GET_SUBJECTS('ИСиТ');
    GET_SUBJECTS('ЛН');
END;
/

--4
CREATE OR REPLACE FUNCTION GET_NUM_TEACHERS(FCODE FACULTY.FACULTY%TYPE) RETURN NUMBER IS
    v_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_count 
    FROM TEACHER t 
    JOIN PULPIT p ON t.PULPIT = p.PULPIT
    WHERE p.FACULTY = FCODE;
    RETURN v_count;
END;
/

BEGIN
    DBMS_OUTPUT.PUT_LINE('Количество преподавателей на ФКН: ' || GET_NUM_TEACHERS('ФКН'));
    DBMS_OUTPUT.PUT_LINE('Количество преподавателей на ФЭН: ' || GET_NUM_TEACHERS('ФЭН'));
END;
/

CREATE OR REPLACE FUNCTION GET_NUM_SUBJECTS(PCODE SUBJECT.PULPIT%TYPE) RETURN NUMBER IS
    v_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_count FROM SUBJECT WHERE PULPIT = PCODE;
    RETURN v_count;
END;
/

BEGIN
    DBMS_OUTPUT.PUT_LINE('Количество дисциплин на кафедре ИСиТ: ' || GET_NUM_SUBJECTS('ИСиТ'));
    DBMS_OUTPUT.PUT_LINE('Количество дисциплин на кафедре ЛН: ' || GET_NUM_SUBJECTS('ЛН'));
END;
/

--5
CREATE OR REPLACE PACKAGE TEACHERS AS
    PROCEDURE GET_TEACHERS(FCODE FACULTY.FACULTY%TYPE);
    PROCEDURE GET_SUBJECTS(PCODE SUBJECT.PULPIT%TYPE);
    FUNCTION GET_NUM_TEACHERS(FCODE FACULTY.FACULTY%TYPE) RETURN NUMBER;
    FUNCTION GET_NUM_SUBJECTS(PCODE SUBJECT.PULPIT%TYPE) RETURN NUMBER;
END TEACHERS;
/

CREATE OR REPLACE PACKAGE BODY TEACHERS AS
    PROCEDURE GET_TEACHERS(FCODE FACULTY.FACULTY%TYPE) IS
    BEGIN
        DBMS_OUTPUT.PUT_LINE('Преподаватели факультета ' || FCODE || ':');
        FOR teacher_rec IN (
            SELECT t.TEACHER_NAME 
            FROM TEACHER t 
            JOIN PULPIT p ON t.PULPIT = p.PULPIT
            WHERE p.FACULTY = FCODE
        ) LOOP
            DBMS_OUTPUT.PUT_LINE(teacher_rec.TEACHER_NAME);
        END LOOP;
    END GET_TEACHERS;
    
    PROCEDURE GET_SUBJECTS(PCODE SUBJECT.PULPIT%TYPE) IS
    BEGIN
        DBMS_OUTPUT.PUT_LINE('Дисциплины кафедры ' || PCODE || ':');
        FOR subject_rec IN (SELECT SUBJECT_NAME FROM SUBJECT WHERE PULPIT = PCODE) LOOP
            DBMS_OUTPUT.PUT_LINE(subject_rec.SUBJECT_NAME);
        END LOOP;
    END GET_SUBJECTS;
    
    FUNCTION GET_NUM_TEACHERS(FCODE FACULTY.FACULTY%TYPE) RETURN NUMBER IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count 
        FROM TEACHER t 
        JOIN PULPIT p ON t.PULPIT = p.PULPIT
        WHERE p.FACULTY = FCODE;
        RETURN v_count;
    END GET_NUM_TEACHERS;
    
    FUNCTION GET_NUM_SUBJECTS(PCODE SUBJECT.PULPIT%TYPE) RETURN NUMBER IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count FROM SUBJECT WHERE PULPIT = PCODE;
        RETURN v_count;
    END GET_NUM_SUBJECTS;
END TEACHERS;
/

--6
BEGIN
    DBMS_OUTPUT.PUT_LINE('Демонстрация работы пакета TEACHERS:');
    DBMS_OUTPUT.PUT_LINE('1. Процедура GET_TEACHERS:');
    TEACHERS.GET_TEACHERS('ФКН');
    DBMS_OUTPUT.PUT_LINE('----------------------');
    
    DBMS_OUTPUT.PUT_LINE('2. Процедура GET_SUBJECTS:');
    TEACHERS.GET_SUBJECTS('ИСиТ');
    DBMS_OUTPUT.PUT_LINE('----------------------');
    
    DBMS_OUTPUT.PUT_LINE('3. Функция GET_NUM_TEACHERS:');
    DBMS_OUTPUT.PUT_LINE('Преподавателей на ФКН: ' || TEACHERS.GET_NUM_TEACHERS('ФКН'));
    DBMS_OUTPUT.PUT_LINE('----------------------');
    
    DBMS_OUTPUT.PUT_LINE('4. Функция GET_NUM_SUBJECTS:');
    DBMS_OUTPUT.PUT_LINE('Дисциплин на ИСиТ: ' || TEACHERS.GET_NUM_SUBJECTS('ИСиТ'));
END;
/
