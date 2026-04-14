--6
SELECT 
    'countries' AS table_name,
    DATA_TYPE AS spatial_data_type,
    COLUMN_NAME AS geometry_column
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'countries' 
  AND DATA_TYPE IN ('geometry', 'geography');

SELECT 
    'airports' AS table_name,
    DATA_TYPE AS spatial_data_type,
    COLUMN_NAME AS geometry_column
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'airports' 
  AND DATA_TYPE IN ('geometry', 'geography');

SELECT 
    'railroads' AS table_name,
    DATA_TYPE AS spatial_data_type,
    COLUMN_NAME AS geometry_column
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'railroads' 
  AND DATA_TYPE IN ('geometry', 'geography');

  --7
SELECT 'countries' AS table_name, 
       geom.STSrid AS srid,
       COUNT(*) AS record_count
FROM countries
WHERE geom IS NOT NULL
GROUP BY geom.STSrid;

SELECT 'airports' AS table_name, 
       geom.STSrid AS srid,
       COUNT(*) AS record_count
FROM airports
WHERE geom IS NOT NULL
GROUP BY geom.STSrid;

SELECT 'railroads' AS table_name, 
       geom.STSrid AS srid,
       COUNT(*) AS record_count
FROM railroads
WHERE geom IS NOT NULL
GROUP BY geom.STSrid;

--8
SELECT 
    'countries' AS table_name,
    COLUMN_NAME,
    DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'countries' 
  AND DATA_TYPE NOT IN ('geometry', 'geography')
ORDER BY ORDINAL_POSITION;

SELECT 
    'airports' AS table_name,
    COLUMN_NAME,
    DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'airports' 
  AND DATA_TYPE NOT IN ('geometry', 'geography')
ORDER BY ORDINAL_POSITION;

SELECT 
    'railroads' AS table_name,
    COLUMN_NAME,
    DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'railroads' 
  AND DATA_TYPE NOT IN ('geometry', 'geography')
ORDER BY ORDINAL_POSITION;

--9
SELECT 
    SOVEREIGNT AS country_name,
    geom.ToString() AS wkt_geometry
FROM countries
WHERE geom IS NOT NULL;

SELECT 
    name AS airport_name,
    geom.ToString() AS wkt_geometry
FROM airports
WHERE geom IS NOT NULL;

SELECT 
    fid AS railroad_id,
	continent AS railroad_continent,
    geom.ToString() AS wkt_geometry
FROM railroads
WHERE geom IS NOT NULL;

-- 10
--10.1
SELECT 
    c.name AS country_name,
    a.name AS airport_name,
    c.geom.STIntersection(a.geom.STBuffer(1000)) AS intersection_geometry
FROM countries c
CROSS JOIN airports a
WHERE c.geom.STIntersects(a.geom.STBuffer(1000)) = 1;

-- 10.2
DECLARE @russia_geom geometry;
SELECT @russia_geom = geom FROM countries WHERE name = 'Russia';

SELECT 
    'Объединенные железные дороги России' AS description,
    @russia_geom.STUnion(r.geom) AS union_geometry
FROM railroads r
WHERE r.geom.STIntersects(@russia_geom) = 1;

--10.3
SELECT 
    a.name AS airport_name,
    c.name AS country_name,
    c.geom.STContains(a.geom) AS is_inside
FROM airports a
JOIN countries c ON c.geom.STContains(a.geom) = 1;

--10.4
SELECT 
    name AS country_name,
    geom.Reduce(0.1) AS simplified_geometry,
    geom.STArea() AS original_area,
    geom.Reduce(0.1).STArea() AS simplified_area,
    geom.STNumPoints() AS original_points,
    geom.Reduce(0.1).STNumPoints() AS simplified_points
FROM countries
WHERE geom IS NOT NULL;

--10.5
SELECT 
    name AS airport_name,
    geom.STX AS x_coordinate,
    geom.STY AS y_coordinate,
    geom.ToString() AS wkt_point,
    geom.STAsText() AS wkt_geometry
FROM airports
WHERE geom IS NOT NULL;

--10.6
SELECT 
    'countries' AS table_name,
    name AS object_name,
    geom.STDimension() AS dimension,
    CASE geom.STDimension()
        WHEN 0 THEN 'Точка/Мультиточка'
        WHEN 1 THEN 'Линия/Мультилиния'
        WHEN 2 THEN 'Полигон/Мультиполигон'
        ELSE 'Пустой или коллекция'
    END AS dimension_type
FROM countries;

SELECT 
    'airports' AS table_name,
    name AS object_name,
    geom.STDimension() AS dimension,
    CASE geom.STDimension()
        WHEN 0 THEN 'Точка/Мультиточка'
        WHEN 1 THEN 'Линия/Мультилиния'
        WHEN 2 THEN 'Полигон/Мультиполигон'
        ELSE 'Пустой или коллекция'
    END AS dimension_type
FROM airports;

SELECT 
    'railroads' AS table_name,
    fid AS object_id,
    geom.STDimension() AS dimension,
    CASE geom.STDimension()
        WHEN 0 THEN 'Точка/Мультиточка'
        WHEN 1 THEN 'Линия/Мультилиния'
        WHEN 2 THEN 'Полигон/Мультиполигон'
        ELSE 'Пустой или коллекция'
    END AS dimension_type
FROM railroads;

--10.7
SELECT 
    name AS country_name,
    geom.STArea() AS area_sq_degrees,
    geom.STArea() * 111319.9 * 111319.9 AS approximate_area_sq_meters
FROM countries
ORDER BY area_sq_degrees DESC;

SELECT 
    fid AS railroad_id,
    geom.STLength() AS length_degrees,
    geom.STLength() * 111319.9 AS approximate_length_meters
FROM railroads
ORDER BY length_degrees DESC;

--10.8
SELECT 
    a1.name AS airport1,
    a2.name AS airport2,
    a1.geom.STDistance(a2.geom) AS distance_degrees,
    a1.geom.STDistance(a2.geom) * 111319.9 AS distance_meters
FROM airports a1
CROSS JOIN airports a2
WHERE a1.name < a2.name
ORDER BY distance_meters;

--11
--1
DECLARE @point geometry = geometry::STGeomFromText('POINT(37.6176 55.7558)', 4326);
INSERT INTO airports (name, geom)
VALUES ('Test Point', @point);

--2
DECLARE @line geometry = geometry::STGeomFromText('LINESTRING(37.5 55.7, 37.7 55.8, 37.9 55.75)', 4326);
INSERT INTO railroads (rwdb_rr_id, featurecla, mult_track, electric, geom)
VALUES (999999, 'Test Line', 0, 0, @line);

--3
DECLARE @polygon geometry = geometry::STGeomFromText('POLYGON((37.5 55.7, 37.7 55.8, 37.9 55.75, 37.8 55.6, 37.5 55.7))', 4326);
INSERT INTO countries (name, geom)
VALUES ('Test Polygon', @polygon);

--12
--1
DECLARE @point geometry = geometry::STGeomFromText('POINT(37.6176 55.7558)', 4326);
SELECT 
    'Точка' AS object_type,
    c.name AS country_name
FROM countries c
WHERE c.geom.STContains(@point) = 1;

--2
DECLARE @line geometry = geometry::STGeomFromText('LINESTRING(37.5 55.7, 37.7 55.8, 37.9 55.75)', 4326);
SELECT 
    'Линия' AS object_type,
    c.name AS country_name
FROM countries c
WHERE c.geom.STIntersects(@line) = 1;

--3
DECLARE @polygon geometry = geometry::STGeomFromText('POLYGON((37.5 55.7, 37.7 55.8, 37.9 55.75, 37.8 55.6, 37.5 55.7))', 4326);
SELECT 
    'Аэропорт' AS object_type,
    a.name AS object_name
FROM airports a
WHERE @polygon.STContains(a.geom) = 1;

SELECT 
    'Железная дорога' AS object_type,
    r.rwdb_rr_id AS object_id
FROM railroads r
WHERE @polygon.STIntersects(r.geom) = 1;