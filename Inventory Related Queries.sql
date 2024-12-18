CREATE TABLE Products (
	Id int IDENTITY(1,1) PRIMARY KEY,
	Name nvarchar(255) NOT NULL,
	Description nvarchar(max) NOT NULL,
	Price decimal(18, 2) NOT NULL,
	Category VARCHAR(50) NOT NULL,
	StockQuantity int NOT NULL,
	IsDeleted bit NOT NULL,
	-- remove the Category later then add CategoryId
);

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY, -- Auto-incrementing primary key
    FullName NVARCHAR(100) NOT NULL,   -- FullName column with a maximum length of 100 characters
    Email NVARCHAR(100) NOT NULL,      -- Email column with a maximum length of 100 characters
    Password NVARCHAR(255) NOT NULL    -- Password column with a maximum length of 255 characters (for hashed passwords)
);

CREATE TABLE UserProfile (
    Id INT PRIMARY KEY IDENTITY(1,1), -- Auto-incrementing primary key
    FullName NVARCHAR(255) NOT NULL, -- Required
    Email NVARCHAR(255) NOT NULL, -- Required
    PhoneNumber NVARCHAR(50) NOT NULL, -- Required
    Address NVARCHAR(500) NOT NULL -- Required
);

CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1)
    CONSTRAINT PK_Categories PRIMARY KEY,
    CategoryName VARCHAR(50),
    IsDeleted BIT NOT NULL
    CONSTRAINT DF_IsDeleted DEFAULT 0
)



