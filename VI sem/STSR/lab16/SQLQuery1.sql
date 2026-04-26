DROP TABLE IF EXISTS TEACHER;
DROP TABLE IF EXISTS SUBJECT;
DROP TABLE IF EXISTS PULPIT;
DROP TABLE IF EXISTS FACULTY;
GO

CREATE TABLE FACULTY (
    FACULTY VARCHAR(10) PRIMARY KEY,
    FACULTY_NAME VARCHAR(100) NOT NULL
);
GO

CREATE TABLE PULPIT (
    PULPIT VARCHAR(10) PRIMARY KEY,
    PULPIT_NAME VARCHAR(100) NOT NULL,
    FACULTY VARCHAR(10) NOT NULL,
    CONSTRAINT FK_PULPIT_FACULTY FOREIGN KEY (FACULTY) 
        REFERENCES FACULTY(FACULTY) ON DELETE CASCADE
);
GO

CREATE TABLE TEACHER (
    TEACHER VARCHAR(10) PRIMARY KEY,
    TEACHER_NAME VARCHAR(100) NOT NULL,
    PULPIT VARCHAR(10) NOT NULL,
    CONSTRAINT FK_TEACHER_PULPIT FOREIGN KEY (PULPIT) 
        REFERENCES PULPIT(PULPIT) ON DELETE CASCADE
);
GO

CREATE TABLE SUBJECT (
    SUBJECT VARCHAR(10) PRIMARY KEY,
    SUBJECT_NAME VARCHAR(100) NOT NULL,
    PULPIT VARCHAR(10) NOT NULL,
    CONSTRAINT FK_SUBJECT_PULPIT FOREIGN KEY (PULPIT) 
        REFERENCES PULPIT(PULPIT) ON DELETE CASCADE
);
GO

INSERT INTO FACULTY ( FACULTY,FACULTY_NAME) VALUES
('ИТ', N'Информационных технологий'),
('ИЭФ', N'Инженерно-экономический факультет'),
('ХТИТ', N'Химической технологии и техники'),
('ТОВ', N'Технологии органического вещества');   
GO

INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES
('ИСИТ', N'Информационных систем и технологий', 'ИТ'),
('ПИ',   N'Программной инженерии',             'ИТ'),
('ПОИТ', N'Программного обеспечения ИТ',       'ИТ'),     
('ЭУП',  N'Экономики и управления на предприятии', 'ИЭФ'),
('МЭП',  N'Менеджмента и экономики предприятия', 'ИЭФ'), 
('ОХ',   N'Органической химии',               'ХТИТ'),
('ТХ',   N'Технологии химических веществ',     'ХТИТ'), 
('ТОС',  N'Технологии органического синтеза',  'ТОВ');
GO

INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES
('БД',   N'Базы данных',                                  'ИСИТ'),
('ООП',  N'Объектно-ориентированное программирование',    'ПИ'),
('КЯП',  N'Конструирование языков программирования',      'ПОИТ'), 
('ВЭД',  N'Внешнеэкономическая деятельность',            'ЭУП'),
('МКТ',  N'Маркетинг',                                    'МЭП'),   
('ОХТ',  N'Органическая химия и технология',             'ОХ'),
('ХТП',  N'Химическая технология переработки',            'ТХ'),    
('ОСН',  N'Органический синтез',                          'ТОС');
GO

INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT) VALUES
('СМЛВ', N'Смелов Владимир Владиславович',      'ПИ'),
('БРВ',  N'Борисов Роман Викторович',           'ПОИТ'),   
('КЛНВ', N'Коваленко Наталья Владимировна',      'ИСИТ'),
('БЛНВ', N'Блинова Евгения Александровна',       'ИСИТ'),
('ЛДН',  N'Ледницкий Андрей Викентьевич',        'ЭУП'),
('ПТРВ', N'Петрова Анна Сергеевна',              'МЭП'),    
('ЖРСК', N'Жарский Иван Михайлович',            'ОХ'),
('ГРБВ', N'Грибова Елена Петровна',              'ТХ');   
GO

