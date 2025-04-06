-- Tạo cơ sở dữ liệu HotelManagement
USE master;
GO

-- Kiểm tra và xóa cơ sở dữ liệu nếu đã tồn tại
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'HotelManagement')
BEGIN
    ALTER DATABASE HotelManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HotelManagement;
END
GO

-- Tạo cơ sở dữ liệu mới
CREATE DATABASE HotelManagement;
GO

USE HotelManagement;
GO

-- Tạo bảng Users
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('admin', 'employee', 'customer')),
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- Tạo bảng RoomTypes
CREATE TABLE RoomTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX),
    BasePrice DECIMAL(10, 2) NOT NULL,
    Capacity INT NOT NULL
);
GO

-- Tạo bảng Rooms
CREATE TABLE Rooms (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber NVARCHAR(10) NOT NULL UNIQUE,
    RoomTypeId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('available', 'occupied', 'maintenance', 'cleaning')),
    Floor INT NOT NULL,
    FOREIGN KEY (RoomTypeId) REFERENCES RoomTypes(Id)
);
GO

-- Tạo bảng Customers
CREATE TABLE Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100),
    PhoneNumber NVARCHAR(20) NOT NULL,
    IdNumber NVARCHAR(50),
    Nationality NVARCHAR(50),
    UserId INT,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

-- Tạo bảng Bookings
CREATE TABLE Bookings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    RoomId INT NOT NULL,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    Adults INT NOT NULL DEFAULT 1,
    Children INT NOT NULL DEFAULT 0,
    TotalAmount DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('pending', 'confirmed', 'checked_in', 'checked_out', 'cancelled')),
    PaymentStatus NVARCHAR(20) NOT NULL CHECK (PaymentStatus IN ('pending', 'paid', 'partially_paid', 'refunded')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    FOREIGN KEY (RoomId) REFERENCES Rooms(Id)
);
GO

-- Tạo bảng ServiceCategories
CREATE TABLE ServiceCategories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255)
);
GO

-- Tạo bảng Services
CREATE TABLE Services (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CategoryId INT NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(10, 2) NOT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (CategoryId) REFERENCES ServiceCategories(Id)
);
GO

-- Tạo bảng BookingServices
CREATE TABLE BookingServices (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL,
    ServiceId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    Price DECIMAL(10, 2) NOT NULL,
    ServiceDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (BookingId) REFERENCES Bookings(Id),
    FOREIGN KEY (ServiceId) REFERENCES Services(Id)
);
GO

-- Tạo bảng RoomAmenities
CREATE TABLE RoomAmenities (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoomTypeId INT NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    FOREIGN KEY (RoomTypeId) REFERENCES RoomTypes(Id)
);
GO

-- Tạo bảng RoomImages
CREATE TABLE RoomImages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoomTypeId INT NOT NULL,
    ImageUrl NVARCHAR(255) NOT NULL,
    IsPrimary BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (RoomTypeId) REFERENCES RoomTypes(Id)
);
GO

-- Tạo bảng Reviews
CREATE TABLE Reviews (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(MAX),
    ReviewDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
);
GO

-- Tạo bảng Invoices
CREATE TABLE Invoices (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL UNIQUE,
    InvoiceNumber NVARCHAR(50) NOT NULL UNIQUE,
    IssuedDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(10, 2) NOT NULL,
    Tax DECIMAL(10, 2) NOT NULL DEFAULT 0,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('draft', 'issued', 'paid', 'cancelled')),
    FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
);
GO

-- Tạo Stored Procedure để lấy báo cáo doanh thu theo tháng
CREATE PROCEDURE GetMonthlyRevenueReport
    @Year INT = NULL
AS
BEGIN
    IF @Year IS NULL
        SET @Year = YEAR(GETDATE());
        
    -- Tính doanh thu từ đặt phòng
    WITH BookingRevenue AS (
        SELECT 
            MONTH(b.CheckOutDate) AS Month,
            SUM(b.TotalAmount) AS Revenue,
            COUNT(DISTINCT b.Id) AS BookingsCount
        FROM Bookings b
        WHERE YEAR(b.CheckOutDate) = @Year
        AND b.Status = 'checked_out'
        GROUP BY MONTH(b.CheckOutDate)
    ),
    -- Tính doanh thu từ dịch vụ
    ServiceRevenue AS (
        SELECT 
            MONTH(b.CheckOutDate) AS Month,
            SUM(bs.Price * bs.Quantity) AS ServiceRevenue
        FROM BookingServices bs
        JOIN Bookings b ON bs.BookingId = b.Id
        WHERE YEAR(b.CheckOutDate) = @Year
        AND b.Status = 'checked_out'
        GROUP BY MONTH(b.CheckOutDate)
    )
    
    -- Kết hợp kết quả
    SELECT 
        br.Month,
        br.Revenue,
        ISNULL(sr.ServiceRevenue, 0) AS Services,
        br.BookingsCount
    FROM BookingRevenue br
    LEFT JOIN ServiceRevenue sr ON br.Month = sr.Month
    ORDER BY br.Month;
END
GO

-- Tạo Stored Procedure để lấy báo cáo công suất phòng
CREATE PROCEDURE GetOccupancyReport
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    IF @StartDate IS NULL
        SET @StartDate = DATEADD(DAY, -30, GETDATE());
    
    IF @EndDate IS NULL
        SET @EndDate = GETDATE();
    
    -- Tạo bảng tạm chứa tất cả các ngày trong khoảng
    DECLARE @AllDates TABLE (Date DATE);
    
    DECLARE @CurrentDate DATE = @StartDate;
    WHILE @CurrentDate <= @EndDate
    BEGIN
        INSERT INTO @AllDates (Date) VALUES (@CurrentDate);
        SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
    END;
    
    -- Tính tổng số phòng
    DECLARE @TotalRooms INT;
    SELECT @TotalRooms = COUNT(*) FROM Rooms;
    
    -- Tính số phòng đã đặt theo ngày
    SELECT 
        ad.Date,
        COUNT(DISTINCT b.RoomId) AS OccupiedRooms,
        @TotalRooms AS TotalRooms,
        CAST(COUNT(DISTINCT b.RoomId) AS FLOAT) / @TotalRooms * 100 AS OccupancyRate
    FROM @AllDates ad
    LEFT JOIN Bookings b ON ad.Date BETWEEN b.CheckInDate AND DATEADD(DAY, -1, b.CheckOutDate)
                        AND b.Status IN ('confirmed', 'checked_in', 'checked_out')
    GROUP BY ad.Date
    ORDER BY ad.Date;
END
GO

-- Thêm dữ liệu mẫu

-- Thêm người dùng
INSERT INTO Users (Name, Email, Password, Role)
VALUES 
('Admin User', 'admin@hotel.com', 'hashed_password_here', 'admin'),
('Employee User', 'employee@hotel.com', 'hashed_password_here', 'employee'),
('John Doe', 'john@example.com', 'hashed_password_here', 'customer'),
('Jane Smith', 'jane@example.com', 'hashed_password_here', 'customer');
GO

-- Thêm loại phòng
INSERT INTO RoomTypes (Name, Description, BasePrice, Capacity)
VALUES 
('Standard', 'Comfortable room with essential amenities', 100.00, 2),
('Deluxe', 'Spacious room with premium amenities', 150.00, 2),
('Suite', 'Luxury suite with separate living area', 250.00, 4),
('Family', 'Large room suitable for families', 200.00, 5);
GO

-- Thêm phòng
INSERT INTO Rooms (RoomNumber, RoomTypeId, Status, Floor)
VALUES 
('101', 1, 'available', 1),
('102', 1, 'available', 1),
('201', 2, 'available', 2),
('202', 2, 'available', 2),
('301', 3, 'available', 3),
('302', 3, 'available', 3),
('401', 4, 'available', 4),
('402', 4, 'available', 4);
GO

-- Thêm tiện nghi phòng
INSERT INTO RoomAmenities (RoomTypeId, Name)
VALUES 
(1, 'Wi-Fi'),
(1, 'TV'),
(1, 'Air Conditioning'),
(2, 'Wi-Fi'),
(2, 'TV'),
(2, 'Mini Bar'),
(3, 'Wi-Fi'),
(3, 'Jacuzzi'),
(3, 'Separate Living Area'),
(4, 'Wi-Fi'),
(4, 'Extra Beds'),
(4, 'Microwave');
GO

-- Thêm khách hàng
INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber, IdNumber, Nationality, UserId)
VALUES 
('John', 'Doe', 'john@example.com', '0901234569', '123456789', 'USA', 3),
('Jane', 'Smith', 'jane@example.com', '0901234570', '987654321', 'UK', 4),
('Robert', 'Johnson', 'robert@example.com', '0901234571', '456789123', 'Canada', NULL),
('Maria', 'Garcia', 'maria@example.com', '0901234572', '789123456', 'Spain', NULL);
GO

-- Thêm danh mục dịch vụ
INSERT INTO ServiceCategories (Name, Description)
VALUES 
('Dining', 'Food and beverage services'),
('Wellness', 'Spa and fitness services'),
('Transportation', 'Airport transfers and car rentals'),
('Laundry', 'Cleaning and ironing services');
GO

-- Thêm dịch vụ
INSERT INTO Services (Name, CategoryId, Description, Price, IsAvailable)
VALUES 
('Breakfast Buffet', 1, 'Full breakfast buffet from 6:30 AM to 10:30 AM', 25.00, 1),
('Room Service', 1, '24-hour room service', 10.00, 1),
('Spa Treatment', 2, '60-minute massage', 80.00, 1),
('Airport Pickup', 3, 'One-way airport transfer', 40.00, 1),
('Laundry Service', 4, 'Same-day laundry service', 30.00, 1);
GO

-- Thêm đặt phòng
INSERT INTO Bookings (CustomerId, RoomId, CheckInDate, CheckOutDate, Adults, Children, TotalAmount, Status, PaymentStatus)
VALUES 
(1, 1, '2023-01-10', '2023-01-15', 2, 0, 500.00, 'checked_out', 'paid'),
(2, 3, '2023-02-15', '2023-02-20', 2, 0, 750.00, 'checked_out', 'paid'),
(3, 5, '2023-03-05', '2023-03-10', 2, 2, 1250.00, 'checked_out', 'paid'),
(4, 7, '2023-04-20', '2023-04-25', 2, 3, 1000.00, 'checked_out', 'paid'),
(1, 2, '2023-05-15', '2023-05-20', 1, 0, 500.00, 'checked_out', 'paid'),
(2, 4, '2023-06-10', '2023-06-15', 2, 0, 750.00, 'checked_out', 'paid'),
(3, 6, '2023-07-05', '2023-07-10', 2, 2, 1250.00, 'checked_out', 'paid'),
(4, 8, '2023-08-20', '2023-08-25', 2, 3, 1000.00, 'confirmed', 'paid');
GO

-- Thêm dịch vụ đặt phòng
INSERT INTO BookingServices (BookingId, ServiceId, Quantity, Price, ServiceDate)
VALUES 
(1, 1, 2, 25.00, '2023-01-11'),
(1, 3, 1, 80.00, '2023-01-12'),
(2, 1, 2, 25.00, '2023-02-16'),
(2, 2, 1, 10.00, '2023-02-17'),
(3, 1, 4, 25.00, '2023-03-06'),
(3, 4, 1, 40.00, '2023-03-05'),
(4, 1, 5, 25.00, '2023-04-21'),
(4, 5, 1, 30.00, '2023-04-23');
GO

-- Thêm hình ảnh phòng
INSERT INTO RoomImages (RoomTypeId, ImageUrl, IsPrimary)
VALUES 
(1, '/images/rooms/standard-1.jpg', 1),
(1, '/images/rooms/standard-2.jpg', 0),
(2, '/images/rooms/deluxe-1.jpg', 1),
(2, '/images/rooms/deluxe-2.jpg', 0),
(3, '/images/rooms/suite-1.jpg', 1),
(3, '/images/rooms/suite-2.jpg', 0),
(4, '/images/rooms/family-1.jpg', 1),
(4, '/images/rooms/family-2.jpg', 0);
GO

-- Thêm hóa đơn
INSERT INTO Invoices (BookingId, InvoiceNumber, IssuedDate, TotalAmount, Tax, Status)
VALUES 
(1, 'INV-202301-00001', '2023-01-15', 550.00, 50.00, 'paid'),
(2, 'INV-202302-00002', '2023-02-20', 825.00, 75.00, 'paid'),
(3, 'INV-202303-00003', '2023-03-10', 1375.00, 125.00, 'paid'),
(4, 'INV-202304-00004', '2023-04-25', 1100.00, 100.00, 'paid'),
(5, 'INV-202305-00005', '2023-05-20', 550.00, 50.00, 'paid'),
(6, 'INV-202306-00006', '2023-06-15', 825.00, 75.00, 'paid'),
(7, 'INV-202307-00007', '2023-07-10', 1375.00, 125.00, 'paid');
GO

-- Thêm đánh giá
INSERT INTO Reviews (BookingId, Rating, Comment, ReviewDate)
VALUES 
(1, 5, 'Excellent stay! The room was clean and comfortable.', '2023-01-16'),
(2, 4, 'Great experience overall. The room was nice.', '2023-02-21'),
(3, 5, 'Perfect for our family vacation.', '2023-03-11'),
(4, 3, 'Room was okay but noisy neighbors.', '2023-04-26'),
(5, 5, 'Another wonderful stay!', '2023-05-21');
GO

PRINT 'Database HotelManagement created successfully with sample data.';
GO