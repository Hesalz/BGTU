USE master;
CREATE DATABASE lab3
on primary
(name = N'lab3_mdf', filename = N'D:\����\III ���\��\lab3\lab3_mdf.mdf',
 size = 10240Kb, maxsize = UNLIMITED, filegrowth = 1024Kb)
, filegroup FG1
(name = N'lab3_gr1', filename = N'D:\����\III ���\��\lab3\lab3_gr1.ndf',
 size = 10240Kb, maxsize = 1Gb, filegrowth = 25%)
log on 
(name = N'lab3_log', filename = N'D:\����\III ���\��\lab3\lab3_log.ldf',
 size = 10240Kb, maxsize = 2048Gb, filegrowth = 10%)

 USE lab3
CREATE TABLE ����������
(��������� nchar(10) primary key,
 �������� nvarchar(50) not null,
 ����� nvarchar(50),
 ������� nvarchar(20)
)on FG1;
CREATE TABLE ������ 
(����� nchar(10) primary key,
 ��������_������ nvarchar(25) not null,
 ����������_�������_��_������ int not null,
 ���� int
)on FG1;
CREATE TABLE �������� 
(���_���������� nchar(10) not null foreign key references
									����������(���������),
 ������� nchar(10) not null foreign key references
							������(�����),
 ����������_����������_������� int not null,
 ����_������ date not null,
 ���������� nvarchar(50)
)on FG1;

ALTER TABLE ������ ADD �������� nvarchar(20); 
ALTER TABLE ������ DROP Column ��������;

INSERT into ���������� (���������, ��������, �����, �������)
	VALUES	('SUP001', '�������', '������, ��. ������, 10', '89031234567'),
			('SUP002', '���������', '�����-���������, ������� ��������, 15', '89047654321'),
			('SUP003', '������������', '������, ��. �����������, 5', '89055556677'),
			('SUP004', '�������������', '������������, ��. ����, 8', '89048887766');
INSERT into ������ 
	VALUES	('PRD001', '����� �8', 500, 5),
			('PRD002', '����� �10', 300, 3),
			('PRD003', '���� �6', 200, 12),
			('PRD004', '����� 4�50', 1000, 8);
INSERT into ��������
	VALUES	('SUP001', 'PRD001', 100, '2024-09-01', NULL),
			('SUP002', 'PRD002', 150, '2024-09-05', '������� ��������'),
			('SUP003', 'PRD003', 200, '2024-09-10', NULL),
			('SUP004', 'PRD004', 500, '2024-09-15', '������ �� �����');

SELECT * FROM ����������;
SELECT �����, ���� FROM ������;
SELECT count(*) AS [���������� �������] FROM ������;
SELECT ��������_������ [������� ������] FROM ������
	WHERE ���� < 6;
UPDATE �������� SET ���������� = NULL WHERE ������� = 'PRD002';
SELECT * FROM ��������;