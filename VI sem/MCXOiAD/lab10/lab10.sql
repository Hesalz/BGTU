USE master;
GO

EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
GO
EXEC sp_configure 'clr enabled', 1;
RECONFIGURE;
GO
EXEC sp_configure 'clr strict security', 0;
RECONFIGURE;
GO

ALTER DATABASE [lab10] SET TRUSTWORTHY ON;
GO

USE [lab10];
GO

DROP PROCEDURE IF EXISTS dbo.MoveFile;
GO

DROP TYPE IF EXISTS dbo.CurrencyExchange;
GO

DROP ASSEMBLY IF EXISTS lab10;
GO

CREATE ASSEMBLY lab10
FROM 'D:\BGTU\VI sem\MCXOiAD\lab10\lab10\bin\Debug\lab10.dll'
WITH PERMISSION_SET = EXTERNAL_ACCESS;
GO

CREATE TYPE dbo.CurrencyExchange
EXTERNAL NAME lab10.CurrencyExchange;
GO

CREATE PROCEDURE dbo.MoveFile
    @SourcePath NVARCHAR(4000),
    @DestinationPath NVARCHAR(4000),
    @Overwrite BIT = 1
AS EXTERNAL NAME lab10.FileOperations.MoveFile;
GO

--1
EXEC xp_cmdshell 'mkdir C:\CLR_Test';
EXEC xp_cmdshell 'mkdir C:\CLR_Test\Source';
EXEC xp_cmdshell 'mkdir C:\CLR_Test\Dest';
EXEC xp_cmdshell 'echo 123 > C:\CLR_Test\Source\file.txt';

EXEC xp_cmdshell 'dir C:\CLR_Test\Source\file.txt';

EXEC dbo.MoveFile 'C:\CLR_Test\Source\file.txt', 'C:\CLR_Test\Dest\file.txt', 1;

EXEC xp_cmdshell 'dir C:\CLR_Test\Dest\file.txt';
EXEC xp_cmdshell 'type C:\CLR_Test\Dest\file.txt';
GO

--2
DECLARE @usd CurrencyExchange;
SET @usd = CurrencyExchange::Parse('USD|92,50|2024-04-25');
SELECT @usd.GetCurrencyInfo() AS [Доллар];