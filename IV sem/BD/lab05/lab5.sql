--1
SELECT tablespace_name FROM dba_tablespaces;

--2
CREATE TABLESPACE HBA_QDATA 
DATAFILE 'HBA_QDATA.dbf' SIZE 10M 
OFFLINE;

ALTER TABLESPACE HBA_QDATA ONLINE;

ALTER USER C##HBA_CORE QUOTA 2M ON HBA_QDATA;

CREATE TABLE HBA_T1 (
    id NUMBER PRIMARY KEY,
    data VARCHAR2(100)
) TABLESPACE HBA_QDATA;

INSERT INTO HBA_T1 (id, data) VALUES (1, 'Первая запись');
INSERT INTO HBA_T1 (id, data) VALUES (2, 'Вторая запись');
INSERT INTO HBA_T1 (id, data) VALUES (3, 'Третья запись');
COMMIT;

--3
SELECT segment_name, segment_type, tablespace_name
FROM dba_segments
WHERE tablespace_name = 'HBA_QDATA';

--4
SELECT segment_name, segment_type, tablespace_name
FROM dba_segments
WHERE tablespace_name = 'HBA_QDATA' AND segment_name = 'HBA_T1';

--5
SELECT segment_name, segment_type, tablespace_name
FROM dba_segments
WHERE tablespace_name = 'HBA_QDATA' AND segment_name <> 'HBA_T1';

--6
DROP TABLE HBA_T1;

--7
SELECT segment_name, segment_type, tablespace_name
FROM dba_segments
WHERE tablespace_name = 'HBA_QDATA';

SELECT segment_name, segment_type, tablespace_name
FROM dba_segments
WHERE tablespace_name = 'HBA_QDATA' AND segment_name = 'HBA_T1';

SELECT object_name, original_name, type, droptime FROM USER_RECYCLEBIN;

--8
FLASHBACK TABLE HBA_T1 TO BEFORE DROP;

--9
TRUNCATE TABLE HBA_T1;

BEGIN
    FOR i IN 1..10000 LOOP
        INSERT INTO HBA_T1 (id, data) VALUES (i, 'Строка №' || i);
    END LOOP;
    COMMIT;
END;
/

--10
SELECT segment_name, COUNT(*) AS extents_count, 
       SUM(blocks) AS total_blocks, 
       SUM(bytes) AS total_bytes
FROM dba_extents
WHERE segment_name = 'HBA_T1'
AND tablespace_name = 'HBA_QDATA'
GROUP BY segment_name;

--11
SELECT segment_name, segment_type, tablespace_name, extent_id, blocks, bytes
FROM dba_extents
ORDER BY tablespace_name, segment_name, extent_id;

--12
SELECT rowid, id, data FROM HBA_T1 FETCH FIRST 10 ROWS ONLY;

SELECT table_name, column_name
FROM user_tab_columns
WHERE column_name = 'ROWID';

--13
SELECT ORA_ROWSCN, id, data FROM HBA_T1 FETCH FIRST 10 ROWS ONLY;

SELECT table_name FROM user_tab_columns WHERE column_name = 'ORA_ROWSCN';

--16
DROP TABLESPACE HBA_QDATA INCLUDING CONTENTS AND DATAFILES;


