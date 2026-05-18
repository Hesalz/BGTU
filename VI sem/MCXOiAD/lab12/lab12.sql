create database Lab12;

create table popa (
id int primary key identity,
pervoe_polupopie nvarchar(100),
vtoroe_polupopie nvarchar(100)
)

insert into popa values
('большое','маленькое'),
('маленькое','большое');

select * from popa;

create login pupa_user
with password = '12345678';

create user pupa_user for login pupa_user;

alter role db_owner add member pupa_user;

