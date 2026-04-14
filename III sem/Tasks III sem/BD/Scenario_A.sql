-- Сценарий А - изучение свойств транзакций
-- Запускаем два сценария параллельно, выполняем в двух окнах - по очереди по отметкам


USE TMP1_BSTU
----- Покажем, что уровень изолированности READ UNCOMMITTED допускает неподтвержденное чтение

-- 1
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
BEGIN TRAN
SELECT COUNT(*) FROM AUDITORIUM -- запускаем транзакцию, Результат: 17
 
--  3
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 16, налицо неподтвержденное чтение
 
-- 5
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 17, после отката транзакции В
COMMIT TRAN

----- Покажем, что уровень изолированности READ COMMITTED не допускает неподтвержденное чтение

-- 6
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
BEGIN TRAN
SELECT COUNT(*) FROM AUDITORIUM -- запускаем транзакцию, Результат: 17
 
--  8
SELECT COUNT(*) FROM AUDITORIUM -- Результат: ожидание, неподтвержденного чтения нет
 
-- 10
SELECT COUNT(*) FROM AUDITORIUM -- сразу после отката транзакции В Результат: 17, 
COMMIT TRAN

----- Покажем, что уровень изолированности READ COMMITTED  допускает неповторяющееся чтение

-- 11
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
BEGIN TRAN
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 17
 
-- 13
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 16 
-- пока вторая транзакция удаляла запись, данные дважды прочитались по-разному.
COMMIT TRAN

----- Покажем, что уровень изолированности REPEATABLE READ не допускает неповторяющееся чтение
INSERT INTO AUDITORIUM VALUES ('128-1', 'ЛК', 60, '128-1'); -- вернем запись
-- 14
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ
BEGIN TRAN
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 17
 
-- 16
COMMIT TRAN -- сразу после фиксации транзакции А в окне В 
--- Строк обработано:1 - прошло выполнение оператора удаления

----- Покажем, что уровень изолированности REPEATABLE READ  допускает проблему фантомных записей
INSERT AUDITORIUM VALUES ('128-1', 'ЛК', 60, '128-1'); -- вернем запись
-- 18
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ 
BEGIN TRAN
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 17
 
-- 20
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 18 
--- в рамках одной транзакции А два результата
COMMIT TRAN

----- Покажем, что уровень изолированности SERIALIZABLE  не допускает проблему фантомных записей
-- 21
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
BEGIN TRAN
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 18

--  23
COMMIT TRAN -- после выполнения этой команды в сценарии В - Строк обработано:1

-- Установим ALLOW_SNAPSHOT_ISOLATION
USE master
GO
ALTER DATABASE B_BSTU SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
USE B_BSTU
GO

----- Покажем, что уровень изолированности SNAPSHOT не блокирует строки таблицы и при этом обеспечивает изолированность
-- 24
SET TRANSACTION ISOLATION LEVEL SNAPSHOT
BEGIN TRAN
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 19

-- 26
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 19 - результат прежний

-- 28
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 19 - а результат все равно прежний

-- 29
COMMIT -- накатываем транзакцию А
SELECT COUNT(*) FROM AUDITORIUM -- Результат: 20 - изменился


