CREATE DATABASE Cinema;
GO

USE Cinema;
GO

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    MiddleName NVARCHAR(50),
    Email NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsAdmin BIT DEFAULT 0,
    Language NVARCHAR(10) DEFAULT 'ru',
    Theme NVARCHAR(10) NOT NULL DEFAULT 'Dark'
);
GO

CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(50) NOT NULL
);
GO

CREATE TABLE Cinemas (
    CinemaId INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(200) NOT NULL,
    HallCount INT NOT NULL,
    Capacity INT NOT NULL,
    Rating FLOAT NOT NULL,
    CategoryId INT NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);
GO

CREATE TABLE CinemaImages (
    ImageId INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    ImageData VARBINARY(MAX) NOT NULL,
    FileName NVARCHAR(255),
    ContentType NVARCHAR(50),
    CinemaId INT NOT NULL,
    FOREIGN KEY (CinemaId) REFERENCES Cinemas(CinemaId)
);
GO

    INSERT INTO Categories (Name) VALUES ('Мультиплекс');
    INSERT INTO Categories (Name) VALUES ('Арт-хаус');
    INSERT INTO Categories (Name) VALUES ('IMAX');
    INSERT INTO Categories (Name) VALUES ('Детский');
    INSERT INTO Categories (Name) VALUES ('Премиум');
