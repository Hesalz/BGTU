SET SERVEROUTPUT ON SIZE 1000000;

-- 1
DECLARE
    v_teacher_name TEACHER.TEACHER_NAME%TYPE;
    v_pulpit TEACHER.PULPIT%TYPE;
BEGIN 
    SELECT TEACHER_NAME, PULPIT INTO v_teacher_name, v_pulpit
    FROM TEACHER
    WHERE TEACHER = 'Иванов';
    
    DBMS_OUTPUT.PUT_LINE('1. Точная выборка:');
    DBMS_OUTPUT.PUT_LINE('Преподаватель: ' || v_teacher_name || ', Кафедра: ' || v_pulpit);
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        DBMS_OUTPUT.PUT_LINE('Преподаватель не найден');
    WHEN TOO_MANY_ROWS THEN
        DBMS_OUTPUT.PUT_LINE('Найдено более одного преподавателя');
END;

-- 2
DECLARE
    v_teacher_name TEACHER.TEACHER_NAME%TYPE;
    v_pulpit TEACHER.PULPIT%TYPE;
BEGIN
    INSERT INTO TEACHER VALUES ('Петров', 'Петров Петр Петрович', 'ИСиТ');
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('2. Неточная выборка:');
    
    SELECT TEACHER_NAME, PULPIT INTO v_teacher_name, v_pulpit
    FROM TEACHER
    WHERE PULPIT = 'ИСиТ';
    
    DBMS_OUTPUT.PUT_LINE('Преподаватель: ' || v_teacher_name || ', Кафедра: ' || v_pulpit);
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Ошибка: ' || SQLERRM);
        DBMS_OUTPUT.PUT_LINE('Код ошибки: ' || SQLCODE);
END;

--3
INSERT INTO TEACHER VALUES ('Денисов', 'Денис Денисович Денисов', 'ИСиТ');
DECLARE
    v_teacher_name TEACHER.TEACHER_NAME%TYPE;
    v_pulpit TEACHER.PULPIT%TYPE;
BEGIN
    DBMS_OUTPUT.PUT_LINE('3. Обработка TOO_MANY_ROWS:');
    
    SELECT TEACHER_NAME, PULPIT INTO v_teacher_name, v_pulpit
    FROM TEACHER
    WHERE PULPIT = 'ИСиТ';
    
    DBMS_OUTPUT.PUT_LINE('Преподаватель: ' || v_teacher_name || ', Кафедра: ' || v_pulpit);
EXCEPTION
    WHEN TOO_MANY_ROWS THEN
        DBMS_OUTPUT.PUT_LINE('Ошибка: найдено более одного преподавателя на кафедре ИСиТ');
    WHEN NO_DATA_FOUND THEN
        DBMS_OUTPUT.PUT_LINE('Преподаватели кафедры ИСиТ не найдены');
END;

--4
DECLARE
    v_teacher_name TEACHER.TEACHER_NAME%TYPE;
BEGIN
    DBMS_OUTPUT.PUT_LINE('4. Обработка NO_DATA_FOUND:');
    
    SELECT TEACHER_NAME INTO v_teacher_name
    FROM TEACHER
    WHERE TEACHER = 'НЕСУЩ';
    
    DBMS_OUTPUT.PUT_LINE('Преподаватель: ' || v_teacher_name);
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        DBMS_OUTPUT.PUT_LINE('Преподаватель не найден');
        DBMS_OUTPUT.PUT_LINE('Атрибуты курсора:');
        DBMS_OUTPUT.PUT_LINE('SQL%ROWCOUNT = ' || SQL%ROWCOUNT);
        DBMS_OUTPUT.PUT_LINE('SQL%FOUND = ' || CASE WHEN SQL%FOUND THEN 'TRUE' ELSE 'FALSE' END);
        DBMS_OUTPUT.PUT_LINE('SQL%NOTFOUND = ' || CASE WHEN SQL%NOTFOUND THEN 'TRUE' ELSE 'FALSE' END);
        DBMS_OUTPUT.PUT_LINE('SQL%ISOPEN = ' || CASE WHEN SQL%ISOPEN THEN 'TRUE' ELSE 'FALSE' END);
END;

select * from teacher
--5
BEGIN
    DBMS_OUTPUT.PUT_LINE('5. Нарушение целостности данных:');
    
    BEGIN
        INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT)
        VALUES ('Денисов', 'Дубликат преподавателя', 'ИСиТ');
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            DBMS_OUTPUT.PUT_LINE('Ошибка: преподаватель с таким кодом уже существует');
    END;
    
    BEGIN
        DELETE FROM PULPIT WHERE PULPIT = 'ИСиТ';
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -2292 THEN
                DBMS_OUTPUT.PUT_LINE('Ошибка: нельзя удалить кафедру, так как есть связанные преподаватели');
            END IF;
    END;
END;

--6
DECLARE
    CURSOR c_teachers IS
        SELECT TEACHER, TEACHER_NAME, PULPIT
        FROM TEACHER;
    
    v_teacher TEACHER.TEACHER%TYPE;
    v_teacher_name TEACHER.TEACHER_NAME%TYPE;
    v_pulpit TEACHER.PULPIT%TYPE;
BEGIN
    DBMS_OUTPUT.PUT_LINE('6. Явный курсор с LOOP:');
    
    OPEN c_teachers;
    LOOP
        FETCH c_teachers INTO v_teacher, v_teacher_name, v_pulpit;
        EXIT WHEN c_teachers%NOTFOUND;
        
        DBMS_OUTPUT.PUT_LINE(v_teacher || ' | ' || v_teacher_name || ' | ' || v_pulpit);
    END LOOP;
    CLOSE c_teachers;
END;

--7
DECLARE
    CURSOR c_subjects IS
        SELECT *
        FROM SUBJECT;
    
    r_subject SUBJECT%ROWTYPE;
BEGIN
    INSERT INTO SUBJECT VALUES ('Англ', 'Английский язык', 'ИСиТ');
    INSERT INTO SUBJECT VALUES ('ОАиП', 'Основы алгоритмизации и программирования', 'ИСиТ');
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('7. Явный курсор с WHILE:');
    
    OPEN c_subjects;
    FETCH c_subjects INTO r_subject;
    
    WHILE c_subjects%FOUND LOOP
        DBMS_OUTPUT.PUT_LINE(r_subject.SUBJECT || ' | ' || 
                           r_subject.SUBJECT_NAME || ' | ' || 
                           r_subject.PULPIT);
        
        FETCH c_subjects INTO r_subject;
    END LOOP;
    
    CLOSE c_subjects;
END;

--8
DECLARE
    CURSOR c_auditoriums(p_min_cap NUMBER, p_max_cap NUMBER) IS
        SELECT AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY
        FROM AUDITORIUM
        WHERE AUDITORIUM_CAPACITY BETWEEN p_min_cap AND p_max_cap
        ORDER BY AUDITORIUM_CAPACITY;
    
    PROCEDURE print_auditoriums_loop(p_min NUMBER, p_max NUMBER) IS
    BEGIN
        DBMS_OUTPUT.PUT_LINE('Аудитории с вместимостью ' || p_min || '-' || p_max || ' (LOOP):');
        
        FOR r IN c_auditoriums(p_min, p_max) LOOP
            DBMS_OUTPUT.PUT_LINE(r.AUDITORIUM || ' | ' || 
                               r.AUDITORIUM_NAME || ' | ' || 
                               r.AUDITORIUM_CAPACITY);
        END LOOP;
    END;
    
    PROCEDURE print_auditoriums_while(p_min NUMBER, p_max NUMBER) IS
        v_auditorium AUDITORIUM.AUDITORIUM%TYPE;
        v_name AUDITORIUM.AUDITORIUM_NAME%TYPE;
        v_capacity AUDITORIUM.AUDITORIUM_CAPACITY%TYPE;
    BEGIN
        DBMS_OUTPUT.PUT_LINE('Аудитории с вместимостью ' || p_min || '-' || p_max || ' (WHILE):');
        
        OPEN c_auditoriums(p_min, p_max);
        FETCH c_auditoriums INTO v_auditorium, v_name, v_capacity;
        
        WHILE c_auditoriums%FOUND LOOP
            DBMS_OUTPUT.PUT_LINE(v_auditorium || ' | ' || 
                               v_name || ' | ' || 
                               v_capacity);
            
            FETCH c_auditoriums INTO v_auditorium, v_name, v_capacity;
        END LOOP;
        
        CLOSE c_auditoriums;
    END;
    
    PROCEDURE print_auditoriums_for(p_min NUMBER, p_max NUMBER) IS
    BEGIN
        DBMS_OUTPUT.PUT_LINE('Аудитории с вместимостью ' || p_min || '-' || p_max || ' (FOR):');
        
        FOR r IN c_auditoriums(p_min, p_max) LOOP
            DBMS_OUTPUT.PUT_LINE(r.AUDITORIUM || ' | ' || 
                               r.AUDITORIUM_NAME || ' | ' || 
                               r.AUDITORIUM_CAPACITY);
        END LOOP;
    END;
    
BEGIN
    DBMS_OUTPUT.PUT_LINE('8. Курсор с параметрами и разными циклами:');
    
    print_auditoriums_loop(0, 20);  
    print_auditoriums_while(21, 30);  
    print_auditoriums_for(31, 60);    
    print_auditoriums_loop(61, 80); 
    print_auditoriums_while(81, 999); 
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Ошибка: ' || SQLERRM);
END;

--9
DECLARE
    TYPE t_teacher_cursor IS REF CURSOR;
    c_teachers t_teacher_cursor;
    
    v_teacher TEACHER.TEACHER%TYPE;
    v_teacher_name TEACHER.TEACHER_NAME%TYPE;
    v_pulpit TEACHER.PULPIT%TYPE;
BEGIN
    DBMS_OUTPUT.PUT_LINE('9. Курсорная переменная:');
    
    OPEN c_teachers FOR
        SELECT TEACHER, TEACHER_NAME, PULPIT
        FROM TEACHER
        WHERE PULPIT = 'ИСиТ';
    
    DBMS_OUTPUT.PUT_LINE('Преподаватели кафедры ИСиТ:');
    
    LOOP
        FETCH c_teachers INTO v_teacher, v_teacher_name, v_pulpit;
        EXIT WHEN c_teachers%NOTFOUND;
        
        DBMS_OUTPUT.PUT_LINE(v_teacher || ' | ' || v_teacher_name || ' | ' || v_pulpit);
    END LOOP;
    
    CLOSE c_teachers;
END;

--10
DECLARE
    CURSOR c_auditoriums IS
        SELECT AUDITORIUM, AUDITORIUM_CAPACITY
        FROM AUDITORIUM
        WHERE AUDITORIUM_CAPACITY BETWEEN 40 AND 80
        FOR UPDATE OF AUDITORIUM_CAPACITY;
BEGIN
    DBMS_OUTPUT.PUT_LINE('10. UPDATE CURRENT OF:');
    
    FOR r IN c_auditoriums LOOP
        DBMS_OUTPUT.PUT_LINE('Аудитория: ' || r.AUDITORIUM || 
                            ', Старая вместимость: ' || r.AUDITORIUM_CAPACITY);
        
        UPDATE AUDITORIUM
        SET AUDITORIUM_CAPACITY = AUDITORIUM_CAPACITY * 0.9
        WHERE CURRENT OF c_auditoriums;
        
        DBMS_OUTPUT.PUT_LINE('Новая вместимость: ' || (r.AUDITORIUM_CAPACITY * 0.9));
    END LOOP;
    
    COMMIT;
    DBMS_OUTPUT.PUT_LINE('Обновление завершено');
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        DBMS_OUTPUT.PUT_LINE('Ошибка: ' || SQLERRM);
END;