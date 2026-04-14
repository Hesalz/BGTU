SELECT P.Код_поставщика, PT.Поставщик, P.Артикул, P.Дата_заказа, PT.Название
		FROM Поставки P, Поставщики PT	
		WHERE P.Код_поставщика = PT.Поставщик 
	and 
		P.Артикул IN (SELECT Товар from Товары 
		WHERE (Название_детали LIKE 'Шуруп%'))

SELECT P.Код_поставщика, PT.Поставщик, P.Артикул, P.Дата_заказа, PT.Название
		FROM Поставки P
	INNER JOIN Поставщики PT ON P.Код_поставщика = PT.Поставщик
		WHERE P.Артикул IN (SELECT Товар FROM Товары 
		WHERE Название_детали LIKE 'Шуруп%'
);

SELECT P.Код_поставщика, PT.Поставщик, P.Артикул, P.Дата_заказа, PT.Название
		FROM Поставки P 
	INNER JOIN Поставщики PT 
		ON P.Код_поставщика = PT.Поставщик
	INNER JOIN Товары T 
		ON T.Товар = P.Артикул
		WHERE (Название_детали LIKE 'Шуруп%')

SELECT P.Артикул, P.Количество_заказанных_деталей
FROM Поставки P
WHERE P.Код_поставщика = (
    SELECT TOP(1) PP.Код_поставщика 
    FROM Поставки PP
    WHERE PP.Артикул = P.Артикул
    ORDER BY PP.Количество_заказанных_деталей DESC)

SELECT Название_детали from Товары 
	WHERE not exists (SELECT * FROM Поставки
						WHERE Поставки.Артикул = Товары.Товар)

SELECT top 1
	(SELECT avg(Количество_заказанных_деталей) FROM Поставки
		WHERE Код_поставщика LIKE 'SUP%') [Среднее SUP],
	(SELECT avg(Количество_заказанных_деталей) FROM Поставки
		WHERE Артикул LIKE 'PRD%') [PRD]
	FROM Поставки

SELECT Название_детали, Цена FROM Товары
	WHERE Цена >=all(SELECT Цена FROM Товары
						WHERE Название_детали LIKE 'Ш%')

SELECT Название_детали, Цена 
	FROM Товары
	WHERE Цена >any (SELECT Цена FROM Товары 
						WHERE Название_детали LIKE 'Диск%')
	
SELECT Название_детали, Цена
FROM Товары
WHERE Название_детали IN (
    SELECT TOP 4 Название_детали
    FROM (
        SELECT TOP 6 Название_детали
        FROM Товары
        ORDER BY Название_детали DESC
    ) AS Первые6
    ORDER BY Название_детали ASC
)
ORDER BY Название_детали DESC;


SELECT Название_детали, Цена
FROM Товары
ORDER BY Название_детали desc