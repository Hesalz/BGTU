USE master;
CREATE DATABASE lab3
on primary
(name = N'lab3_mdf', filename = N'D:\БГТУ\III сем\БД\lab3\lab3_mdf.mdf',
 size = 10240Kb, maxsize = UNLIMITED, filegrowth = 1024Kb)
, filegroup FG1
(name = N'lab3_gr1', filename = N'D:\БГТУ\III сем\БД\lab3\lab3_gr1.ndf',
 size = 10240Kb, maxsize = 1Gb, filegrowth = 25%)
log on 
(name = N'lab3_log', filename = N'D:\БГТУ\III сем\БД\lab3\lab3_log.ldf',
 size = 10240Kb, maxsize = 2048Gb, filegrowth = 10%)

 USE lab3
CREATE TABLE Поставщики
(Поставщик nchar(10) primary key,
 Название nvarchar(50) not null,
 Адрес nvarchar(50),
 Телефон nvarchar(20)
)on FG1;
CREATE TABLE Товары 
(Товар nchar(10) primary key,
 Название_детали nvarchar(25) not null,
 Количество_деталей_на_складе int not null,
 Цена int
)on FG1;
CREATE TABLE Поставки 
(Код_поставщика nchar(10) not null foreign key references
									Поставщики(Поставщик),
 Артикул nchar(10) not null foreign key references
							Товары(Товар),
 Количество_заказанных_деталей int not null,
 Дата_заказа date not null,
 Примечание nvarchar(50)
)on FG1;

ALTER TABLE Товары ADD Материал nvarchar(20); 
ALTER TABLE Товары DROP Column Материал;

INSERT into Поставщики (Поставщик, Название, Адрес, Телефон)
	VALUES	('SUP001', 'ТехСнаб', 'Москва, ул. Ленина, 10', '89031234567'),
			('SUP002', 'АвтоПартс', 'Санкт-Петербург, Невский проспект, 15', '89047654321'),
			('SUP003', 'МегаКомплект', 'Казань, ул. Кремлевская, 5', '89055556677'),
			('SUP004', 'ТехИнструмент', 'Екатеринбург, ул. Мира, 8', '89048887766');
INSERT into Товары 
	VALUES	('PRD001', 'Гайка М8', 500, 5),
			('PRD002', 'Шайба М10', 300, 3),
			('PRD003', 'Болт М6', 200, 12),
			('PRD004', 'Шуруп 4х50', 1000, 8);
INSERT into Поставки
	VALUES	('SUP001', 'PRD001', 100, '2024-09-01', NULL),
			('SUP002', 'PRD002', 150, '2024-09-05', 'Обычная поставка'),
			('SUP003', 'PRD003', 200, '2024-09-10', NULL),
			('SUP004', 'PRD004', 500, '2024-09-15', 'Оплата по факту');

SELECT * FROM Поставщики;
SELECT Товар, Цена FROM Товары;
SELECT count(*) AS [Количество товаров] FROM Товары;
SELECT Название_детали [Дешевые товары] FROM Товары
	WHERE Цена < 6;
UPDATE Поставки SET Примечание = NULL WHERE Артикул = 'PRD002';
SELECT * FROM Поставки;