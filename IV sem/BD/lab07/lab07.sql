--1
SELECT SUM(value) / 1024 / 1024 AS "SGA Size (MB)"
FROM v$sga;

--2
SELECT name, bytes / 1024 / 1024 AS "Size (MB)"
FROM v$sgainfo
WHERE name IN ('Shared Pool Size', 'Buffer Cache Size', 'Large Pool Size', 'Java Pool Size', 'Streams Pool Size');

--3
SELECT name, bytes / 1024 / 1024 AS "Granule Size (MB)"
FROM v$sgainfo
WHERE name LIKE 'Granule Size';

--4
SELECT name, bytes / 1024 / 1024 AS "Free Memory (MB)"
FROM v$sgainfo
WHERE name = 'Free SGA Memory Available';

--5
SELECT name, value / 1024 / 1024 AS "Size (MB)"
FROM v$parameter
WHERE name IN ('sga_target', 'sga_max_size');

--6
SELECT NAME, BLOCK_SIZE, BUFFERS, (BLOCK_SIZE * BUFFERS) / 1024 / 1024 AS "Size (MB)"
FROM V$BUFFER_POOL;

--7
CREATE TABLESPACE keep_tbs 
DATAFILE 'D:\database-orac\oradata\ORCLXE\keep_tbs01.dbf' SIZE 100M;

CREATE TABLE keep_table (
    id NUMBER PRIMARY KEY, 
    data VARCHAR2(100)
) TABLESPACE keep_tbs STORAGE (BUFFER_POOL KEEP);

SELECT SEGMENT_NAME, SEGMENT_TYPE, TABLESPACE_NAME, BUFFER_POOL
FROM DBA_SEGMENTS
WHERE SEGMENT_NAME = 'KEEP_TABLE';

--8
CREATE TABLE default_table (
    id NUMBER PRIMARY KEY, 
    data VARCHAR2(100)
) STORAGE (BUFFER_POOL DEFAULT);

SELECT SEGMENT_NAME, SEGMENT_TYPE, TABLESPACE_NAME, BUFFER_POOL
FROM DBA_SEGMENTS
WHERE SEGMENT_NAME = 'DEFAULT_TABLE';

--9
SELECT NAME, VALUE / 1024 / 1024 AS "Size (MB)"
FROM V$SGA
WHERE NAME = 'Redo Buffers';

--10
SELECT NAME, VALUE / 1024 / 1024 AS "Large Pool Size (MB)"
FROM V$PARAMETER
WHERE NAME = 'large_pool_size';

--11
SELECT SERVER, COUNT(*) AS CONNECTIONS
FROM V$SESSION
GROUP BY SERVER;

--12
SELECT NAME, DESCRIPTION
FROM V$BGPROCESS
WHERE PADDR <> '00';

--13
SELECT PROGRAM
FROM V$PROCESS
WHERE BACKGROUND IS NULL;

--14
SELECT COUNT(*)
FROM V$PROCESS
WHERE PROGRAM LIKE 'DBW%';

--15
SELECT NAME, NETWORK_NAME, CON_ID
FROM V$SERVICES;

--16
SHOW PARAMETER dispatcher;
SHOW PARAMETER shared_server;

--17
-- D:\database-orac\homes\OraDB21Home2\network\admin

--19
-- lsnrctl

-- 20
-- lsnrctl services 888****************
