-- Сценарий В - изучение свойств транзакций
-- Запускаем два сценария параллельно, выполняем в двух окнах - по очереди по отметкам

USE TMP1_BSTU

--- 2
BEGIN TRAN  -- открываем параллельную транзакцию
DELETE FROM AUDITORIUM WHERE AUDITORIUM='128-1' -- удаляем строку из таблицы

--- 4
ROLLBACK TRAN -- откатываем транзакцию

--- 7
BEGIN TRAN  -- открываем параллельную транзакцию
DELETE FROM AUDITORIUM WHERE AUDITORIUM='128-1' -- удаляем строку из таблицы

--- 9
ROLLBACK TRAN -- откатываем транзакцию 

--- 12
BEGIN TRAN  -- открываем параллельную транзакцию
DELETE FROM AUDITORIUM WHERE AUDITORIUM = '128-1' -- удаляем строку из таблицы
COMMIT TRAN

--- 15
BEGIN TRAN  -- открываем параллельную транзакцию
DELETE FROM AUDITORIUM WHERE AUDITORIUM = '128-1' -- удаляем строку из таблицы, результат - ожидание

--- 17
COMMIT TRAN -- завершаем транзакцию

--- 19
BEGIN TRAN
INSERT INTO AUDITORIUM VALUES ('437-1','ЛК', 20, '437-1')  -- Строк обработано:1
COMMIT TRAN -- завершаем транзакцию

--- 22
BEGIN TRAN
INSERT INTO AUDITORIUM VALUES ('446-1','ЛК', 20, '446-1') -- ожидание

-- 24
COMMIT TRAN

--- 25
BEGIN TRAN 
INSERT INTO AUDITORIUM VALUES ('447-1','ЛК', 20, '447-1') -- выполняем вставку - Строк обработано:1

--27
COMMIT -- накатываем транзакцию В


 