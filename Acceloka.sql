CREATE DATABASE Acceloka
GO

USE Acceloka
GO

CREATE TABLE Category(
	Id INT NOT NULL
	CONSTRAINT PK_Category PRIMARY KEY IDENTITY,

	Name VARCHAR(255) NOT NULL,

	CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
	CreatedBy VARCHAR(255) NOT NULL DEFAULT 'SYSTEM',
	UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
	UpdatedBy VARCHAR(255) NOT NULL DEFAULT 'SYSTEM'
);

CREATE TABLE Ticket(
	Code VARCHAR(50) NOT NULL
	CONSTRAINT PK_Ticket PRIMARY KEY,

	Name varchar(255) not null,
	Price INT NOT NULL,
	Quota INT NOT NULL,
	Date DATETIME NOT NULL,

	CategoryId INT NOT NULL 
	CONSTRAINT FK_Ticket_Category FOREIGN KEY REFERENCES Category(Id),

	CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
	CreatedBy VARCHAR(255) NOT NULL DEFAULT 'SYSTEM',
	UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
	UpdatedBy VARCHAR(255) NOT NULL DEFAULT 'SYSTEM'
);

CREATE TABLE BookedTicket(
	Id INT NOT NULL
	CONSTRAINT PK_BookedTicket PRIMARY KEY IDENTITY,

	Date DATETIME NOT NULL,

	CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
	CreatedBy VARCHAR(255) NOT NULL DEFAULT 'SYSTEM',
	UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
	UpdatedBy VARCHAR(255) NOT NULL DEFAULT 'SYSTEM'
);

CREATE TABLE Detail(
	Id INT NOT NULL
	CONSTRAINT PK_Detail PRIMARY KEY IDENTITY,

	Quantity INT NOT NULL,

	TicketCode VARCHAR(50) NOT NULL 
	CONSTRAINT FK_Detail_Ticket FOREIGN KEY REFERENCES Ticket(Code),

	BookedTicketId INT NOT NULL
	CONSTRAINT FK_Detail_BookedTicket FOREIGN KEY REFERENCES BookedTicket(Id),

	CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
	CreatedBy VARCHAR(255) NOT NULL DEFAULT 'SYSTEM',
	UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
	UpdatedBy VARCHAR(255) NOT NULL DEFAULT 'SYSTEM'
);

INSERT INTO Category (Name) VALUES
('Transportasi Darat'),
('Transportasi Laut'),
('Cinema'),
('Hotel'),
('Konser Musik'),
('Seminar & Workshop'),
('Festival Budaya'),
('Olahraga'),
('Pameran & Expo');

INSERT INTO Ticket (Code, Name, Price, Quota, Date, CategoryId) VALUES
-- Transportasi Darat
('TD001', 'Bus Jawa-Sumatra', 500000, 80, '2025-05-10 12:00:00', 1),
('TD002', 'Kereta Eksekutif Jakarta-Surabaya', 700000, 90, '2025-06-15 08:00:00', 1),
('TD003', 'Bus Malang-Bandung', 600000, 85, '2025-07-20 14:30:00', 1),
('TD004', 'Bus Jakarta-Bali', 600000, 90, '2025-08-05 14:00:00', 1),

-- Transportasi Laut
('TL001', 'Kapal Feri Jawa-Sumatra', 250000, 70, '2025-05-20 13:00:00', 2),
('TL002', 'Pelni Jakarta-Makassar', 450000, 100, '2025-06-25 18:00:00', 2),
('TL003', 'Kapal Pesiar Bali-Komodo', 2000000, 60, '2025-07-30 09:00:00', 2),
('TL004', 'Speedboat Labuan Bajo-Komodo', 750000, 55, '2025-09-15 07:30:00', 2),

-- Cinema
('C001', 'Ironman CGV', 75000, 99, '2025-05-15 20:00:00', 3),
('C002', 'Batman IMAX', 100000, 95, '2025-06-18 21:30:00', 3),
('C003', 'Avatar 3 XXI', 95000, 90, '2025-07-22 19:45:00', 3),
('C004', 'John Wick IMAX', 110000, 98, '2025-08-15 19:00:00', 3),

-- Hotel
('H001', 'Ibis Hotel Jakarta 21-23', 1500000, 76, '2025-05-12 12:00:00', 4),
('H002', 'Grand Hyatt Bali', 2500000, 50, '2025-06-14 15:00:00', 4),
('H003', 'The Ritz-Carlton Jakarta', 3500000, 40, '2025-07-16 11:00:00', 4),
('H004', 'Aston Hotel Medan', 1800000, 60, '2025-09-08 14:00:00', 4),

-- Konser Musik
('M001', 'Festival Musik Nusantara', 300000, 1000, '2025-05-25 18:30:00', 5),
('M002', 'Rock Fest 2025', 350000, 1200, '2025-06-28 20:00:00', 5),
('M003', 'Konser Jazz Malam', 320000, 800, '2025-07-30 21:00:00', 5),

-- Seminar & Workshop
('S001', 'Seminar Bisnis Digital', 200000, 300, '2025-05-05 09:30:00', 6),
('S002', 'Workshop Coding Pemula', 250000, 200, '2025-06-07 10:00:00', 6),
('S003', 'Pelatihan UI/UX Design', 220000, 180, '2025-08-15 09:00:00', 6),

-- Festival Budaya
('F001', 'Festival Batik Nasional', 50000, 1000, '2025-07-20 10:00:00', 7),
('F002', 'Pawai Budaya Nusantara', 75000, 800, '2025-08-10 14:00:00', 7),
('F003', 'Pentas Tari Tradisional', 60000, 700, '2025-09-18 19:00:00', 7),

-- Olahraga
('O001', 'Final Sepak Bola', 600000, 500, '2025-05-10 17:00:00', 8),
('O002', 'Kejuaraan Bulu Tangkis', 450000, 400, '2025-06-12 16:30:00', 8),
('O003', 'Marathon Kota 2025', 300000, 700, '2025-07-18 06:00:00', 8),

-- Pameran & Expo
('P001', 'Pameran Teknologi 2025', 250000, 800, '2025-05-20 10:00:00', 9),
('P002', 'Pameran Startup Digital', 300000, 700, '2025-06-22 11:00:00', 9),
('P003', 'Expo Fashion & Beauty', 220000, 900, '2025-07-25 09:30:00', 9);