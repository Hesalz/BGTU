DROP TABLE service_delivery PURGE;
DROP TABLE employees PURGE;
DROP TABLE service_types PURGE;

CREATE TABLE service_delivery (
    employee_id   NUMBER,
    service_type  VARCHAR2(50),
    delivery_date DATE,
    amount        NUMBER(15,2)
);

CREATE TABLE employees (
    employee_id   NUMBER PRIMARY KEY,
    employee_name VARCHAR2(100)
);

CREATE TABLE service_types (
    service_type  VARCHAR2(50) PRIMARY KEY,
    description   VARCHAR2(200)
);

INSERT INTO employees VALUES (1, 'Иванов Иван');
INSERT INTO employees VALUES (2, 'Петров Петр');
INSERT INTO employees VALUES (3, 'Сидорова Анна');

INSERT INTO service_types VALUES ('Консалтинг', 'Консультационные услуги');
INSERT INTO service_types VALUES ('Аудит', 'Аудиторские услуги');
INSERT INTO service_types VALUES ('Обучение', 'Обучающие курсы');

INSERT INTO service_delivery VALUES (1, 'Консалтинг', DATE '2024-01-15', 1000);
INSERT INTO service_delivery VALUES (1, 'Консалтинг', DATE '2024-02-10', 1050);
INSERT INTO service_delivery VALUES (1, 'Консалтинг', DATE '2024-03-05', 1100);
INSERT INTO service_delivery VALUES (1, 'Консалтинг', DATE '2024-04-20', 1080);
INSERT INTO service_delivery VALUES (1, 'Консалтинг', DATE '2024-05-12', 1150);
INSERT INTO service_delivery VALUES (1, 'Консалтинг', DATE '2024-06-18', 1200);
INSERT INTO service_delivery VALUES (1, 'Аудит', DATE '2024-01-22', 2000);
INSERT INTO service_delivery VALUES (1, 'Аудит', DATE '2024-02-14', 2100);
INSERT INTO service_delivery VALUES (1, 'Аудит', DATE '2024-03-10', 2050);
INSERT INTO service_delivery VALUES (1, 'Аудит', DATE '2024-04-05', 2200);
INSERT INTO service_delivery VALUES (2, 'Консалтинг', DATE '2024-01-30', 500);
INSERT INTO service_delivery VALUES (2, 'Консалтинг', DATE '2024-02-28', 520);
INSERT INTO service_delivery VALUES (2, 'Консалтинг', DATE '2024-03-25', 510);
INSERT INTO service_delivery VALUES (2, 'Консалтинг', DATE '2024-04-19', 550);
INSERT INTO service_delivery VALUES (3, 'Обучение', DATE '2024-01-11', 3000);
INSERT INTO service_delivery VALUES (3, 'Обучение', DATE '2024-02-09', 3100);
INSERT INTO service_delivery VALUES (3, 'Обучение', DATE '2024-03-15', 3200);
INSERT INTO service_delivery VALUES (3, 'Обучение', DATE '2024-04-21', 3150);
INSERT INTO service_delivery VALUES (3, 'Обучение', DATE '2024-05-17', 3300);
INSERT INTO service_delivery VALUES (3, 'Обучение', DATE '2024-06-13', 3400);

COMMIT;

-- 1
WITH base_data AS (
    SELECT 
        employee_id,
        service_type,
        EXTRACT(MONTH FROM delivery_date) AS mn,
        SUM(amount) AS monthly_amount
    FROM service_delivery
    WHERE EXTRACT(YEAR FROM delivery_date) = 2024
    GROUP BY employee_id, service_type, EXTRACT(MONTH FROM delivery_date)
),
last_month_value AS (
    SELECT 
        employee_id,
        service_type,
        MAX(monthly_amount) KEEP (DENSE_RANK LAST ORDER BY mn) AS last_amount
    FROM base_data
    GROUP BY employee_id, service_type
),
model_result AS (
    SELECT 
        employee_id,
        service_type,
        month_offset,
        forecast_amount
    FROM (
        SELECT 
            l.employee_id,
            l.service_type,
            l.last_amount,
            0 AS forecast_amount,
            0 AS month_offset
        FROM last_month_value l
    )
    MODEL
        PARTITION BY (employee_id, service_type)
        DIMENSION BY (0 AS month_offset)
        MEASURES (last_amount AS amount, 0 AS forecast_amount)
        RULES SEQUENTIAL ORDER (
            forecast_amount[1] = amount[0] * 1.05,
            forecast_amount[2] = forecast_amount[1] * 1.05,
            forecast_amount[3] = forecast_amount[2] * 1.05,
            forecast_amount[4] = forecast_amount[3] * 1.05,
            forecast_amount[5] = forecast_amount[4] * 1.05,
            forecast_amount[6] = forecast_amount[5] * 1.05,
            forecast_amount[7] = forecast_amount[6] * 1.05,
            forecast_amount[8] = forecast_amount[7] * 1.05,
            forecast_amount[9] = forecast_amount[8] * 1.05,
            forecast_amount[10] = forecast_amount[9] * 1.05,
            forecast_amount[11] = forecast_amount[10] * 1.05,
            forecast_amount[12] = forecast_amount[11] * 1.05
        )
)
SELECT 
    employee_id,
    service_type,
    month_offset AS forecast_month,
    ROUND(forecast_amount, 2) AS forecast_amount
FROM model_result
WHERE month_offset BETWEEN 0 AND 11
ORDER BY employee_id, service_type, forecast_month;

-- 2
WITH monthly_by_type AS (
    SELECT 
        service_type,
        TRUNC(delivery_date, 'MM') AS month_start,
        SUM(amount) AS total_amount
    FROM service_delivery
    GROUP BY service_type, TRUNC(delivery_date, 'MM')
)
SELECT 
    service_type,
    TO_CHAR(first_month, 'YYYY-MM') AS first_month,
    TO_CHAR(second_month, 'YYYY-MM') AS second_month,
    TO_CHAR(third_month, 'YYYY-MM') AS third_month,
    first_amount,
    second_amount,
    third_amount
FROM monthly_by_type
MATCH_RECOGNIZE (
    PARTITION BY service_type
    ORDER BY month_start
    MEASURES
        FIRST(up.month_start) AS first_month,
        FIRST(down.month_start) AS second_month,
        FIRST(up2.month_start) AS third_month,
        FIRST(up.total_amount) AS first_amount,
        FIRST(down.total_amount) AS second_amount,
        FIRST(up2.total_amount) AS third_amount
    PATTERN (up down up2)
    DEFINE
        up AS up.total_amount > PREV(up.total_amount),
        down AS down.total_amount < PREV(down.total_amount),
        up2 AS up2.total_amount > PREV(up2.total_amount)
)
ORDER BY service_type, first_month;