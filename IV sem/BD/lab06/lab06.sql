--1
SHOW PARAMETER spfile;

--3
-- sqlplus / as sysdba    CREATE PFILE='D:\database-orac\database\HBA_PFILE.ORA' FROM SPFILE;

--5
-- sqlplus / as sysdba    CREATE SPFILE='D:\database-orac\database\HBA_SPFILE.ORA' 
-- sqlplus / as sysdba    FROM PFILE='D:\database-orac\database\HBA_PFILE.ORA';

--6
-- sqlplus / as sysdba    SHUTDOWN IMMEDIATE;
-- sqlplus / as sysdba    STARTUP;

--7
-- sqlplus / as sysdba    ALTER SYSTEM RESET db_block_size SCOPE=SPFILE;

--8
SELECT REGEXP_SUBSTR(name, '[^/]+$', 1, 1) AS controlfile_name
FROM v$controlfile;

--9
ALTER DATABASE BACKUP CONTROLFILE TO TRACE;

ALTER SYSTEM SET CONTROL_FILES = 'D:\database-orac\oradata\ORCLXE\CONTROLFILE\O1_MF_MWVXTRV9_.CTL',
'D:\database-orac\oradata\ORCLXE\CONTROLFILE\O1_MF_MWVXTRV9_copy.CTL' SCOPE = SPFILE;

--10
SELECT * FROM V$PASSWORDFILE_INFO;

--12
SELECT * FROM V$DIAG_INFO;

--14
SELECT * FROM v$diag_info WHERE name = 'Diag Trace';
