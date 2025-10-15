CREATE DATABASE LibraryManagementDb;
GO

USE LibraryManagementDb;
GO

CREATE TABLE Books (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Author NVARCHAR(150) NOT NULL,
    Category NVARCHAR(100) NULL,
    Isbn NVARCHAR(20) NULL,
    TotalCopies INT NOT NULL DEFAULT 0,
    AvailableCopies INT NOT NULL DEFAULT 0,
    CoverUrl NVARCHAR(500) NULL,
    Description NVARCHAR(MAX) NULL
);

CREATE TABLE ApplicationUsers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(50) NULL,
    Address NVARCHAR(255) NULL,
    Role INT NOT NULL,
    RegisteredAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE Loans (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BookId INT NOT NULL,
    ApplicationUserId INT NOT NULL,
    BorrowedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DueAt DATETIME2 NOT NULL,
    ReturnedAt DATETIME2 NULL,
    Status INT NOT NULL,
    CONSTRAINT FK_Loans_Books FOREIGN KEY (BookId) REFERENCES Books(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Loans_Users FOREIGN KEY (ApplicationUserId) REFERENCES ApplicationUsers(Id) ON DELETE NO ACTION
);

INSERT INTO Books (Title, Author, Category, Isbn, TotalCopies, AvailableCopies, CoverUrl, Description)
VALUES
('Clean Code', 'Robert C. Martin', 'Software Engineering', '9780132350884', 6, 5, 'https://images-na.ssl-images-amazon.com/images/I/41jEbK-jG+L.jpg', 'A handbook of agile software craftsmanship.'),
('The Pragmatic Programmer', 'Andrew Hunt, David Thomas', 'Software Engineering', '9780201616224', 4, 4, 'https://images-na.ssl-images-amazon.com/images/I/41uPjEenkFL._SX380_BO1,204,203,200_.jpg', 'Journey to mastery for modern programmers.'),
('Atomic Habits', 'James Clear', 'Self-help', '9780735211292', 8, 8, 'https://images-na.ssl-images-amazon.com/images/I/513Y5o-DYtL.jpg', 'Tiny changes lead to remarkable results.');

INSERT INTO ApplicationUsers (FullName, Email, PhoneNumber, Address, Role)
VALUES
('Lan Nguyen', 'lan.nguyen@example.com', '+84 912345678', '123 Nguyen Trai, Ha Noi', 0),
('Minh Tran', 'minh.tran@example.com', '+84 934567890', '45 Hai Ba Trung, Ha Noi', 0),
('Quang Le', 'quang.le@example.com', '+84 987654321', '78 Vo Thi Sau, Ho Chi Minh', 1),
('Thu Hoang', 'thu.hoang@example.com', '+84 923456789', '56 Dien Bien Phu, Da Nang', 1);

INSERT INTO Loans (BookId, ApplicationUserId, BorrowedAt, DueAt, ReturnedAt, Status)
VALUES
(1, 3, DATEADD(DAY, -3, SYSUTCDATETIME()), DATEADD(DAY, 11, SYSUTCDATETIME()), NULL, 0);
GO
