USE master
GO


-- Step 1
DROP TABLE IF EXISTS OnlineOrderDetail;
DROP TABLE IF EXISTS InStoreOrderDetail;
DROP TABLE IF EXISTS CartItem;
DROP TABLE IF EXISTS FeedbackProduct;
DROP TABLE IF EXISTS ProductVehicleType;
DROP TABLE IF EXISTS MotobikeProductImage;
DROP TABLE IF EXISTS MotobikeProductPrice;
GO

-- Step 2
DROP TABLE IF EXISTS OnlineOrder;
DROP TABLE IF EXISTS InStoreOrder;
DROP TABLE IF EXISTS Cart;
DROP TABLE IF EXISTS FeedbackStaff;
DROP TABLE IF EXISTS [Notification];
DROP TABLE IF EXISTS Bill;
DROP TABLE IF EXISTS RepairBooking;
GO

-- Step 3
DROP TABLE IF EXISTS MotobikeProduct;
DROP TABLE IF EXISTS RepairService;
GO

-- Step 4
DROP TABLE IF EXISTS Vehicle;
DROP TABLE IF EXISTS Discount;
DROP TABLE IF EXISTS Warranty;
DROP TABLE IF EXISTS Category;
GO

-- Step 5
DROP TABLE IF EXISTS CustomerAccount;
DROP TABLE IF EXISTS OwnerAccount;
DROP TABLE IF EXISTS StaffAccount;
DROP TABLE IF EXISTS TellerAccount;
GO

-- Step 6
DROP TABLE IF EXISTS Account;
GO

-- Step 7
DROP TABLE IF EXISTS AccountRole;
GO

DROP DATABASE IF EXISTS ARTHS_DB;
GO
CREATE DATABASE ARTHS_DB
GO

USE ARTHS_DB
GO

--Table role
DROP TABLE IF EXISTS AccountRole;
GO
CREATE TABLE AccountRole(
	Id uniqueidentifier primary key NOT NULL,
	RoleName nvarchar(50) NOT NULL
);
GO

INSERT INTO AccountRole (Id, RoleName)
VALUES 
(NEWID(), 'Customer'),
(NEWID(), 'Staff'),
(NEWID(), 'Owner'),
(NEWID(), 'Teller');
GO

--Table account
DROP TABLE IF EXISTS Account;
GO
CREATE TABLE Account(
	Id uniqueidentifier primary key NOT NULL,
	RoleId uniqueidentifier foreign key references AccountRole(Id) NOT NULL,
	PhoneNumber varchar(30) unique NOT NULL,
	PasswordHash varchar(255) NOT NULL,
	RefreshToken varchar(255),
	Status nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL default getdate()
);
GO

--Table owner
DROP TABLE IF EXISTS OwnerAccount;
GO
CREATE TABLE OwnerAccount(
	AccountId uniqueidentifier foreign key references Account(Id) NOT NULL,
	FullName nvarchar(255) NOT NULL,
	Gender nvarchar(10) NOT NULL, --"Nam", "Nữ", "Khác",
	Avatar varchar(max),
	primary key(AccountId)
);
GO

--Table staff
DROP TABLE IF EXISTS StaffAccount;
GO
CREATE TABLE StaffAccount(
	AccountId uniqueidentifier foreign key references Account(Id) NOT NULL,
	FullName nvarchar(255) NOT NULL,
	Gender nvarchar(10) NOT NULL, --"Nam", "Nữ", "Khác",
	Avatar varchar(max),
	primary key(AccountId)
);
GO

--Table teller
DROP TABLE IF EXISTS TellerAccount;
GO
CREATE TABLE TellerAccount(
	AccountId uniqueidentifier foreign key references Account(Id) NOT NULL,
	FullName nvarchar(255) NOT NULL,
	Gender nvarchar(10) NOT NULL, --"Nam", "Nữ", "Khác",
	Avatar varchar(max),
	primary key(AccountId)
);
GO

--Table customer
DROP TABLE IF EXISTS CustomerAccount;
GO
CREATE TABLE CustomerAccount(
	AccountId uniqueidentifier foreign key references Account(Id) NOT NULL,
	FullName nvarchar(255) NOT NULL,
	Gender nvarchar(10) NOT NULL, --"Nam", "Nữ", "Khác",
	Avatar varchar(max),
	Address nvarchar(255) NOT NULL,
	primary key(AccountId)
);
GO

--Table feedback staff
DROP TABLE IF EXISTS FeedbackStaff;
GO
CREATE TABLE FeedbackStaff(
	Id uniqueidentifier primary key NOT NULL,
	CustomerId uniqueidentifier foreign key references CustomerAccount(AccountId),
	StaffId uniqueidentifier foreign key references StaffAccount(AccountId) NOT NULL,
	Title nvarchar(255),
	Content nvarchar(max) NOT NULL,
	SendDate datetime NOT NULL default getdate()
);
GO

--Table notification
DROP TABLE IF EXISTS [Notification];
GO
CREATE TABLE [Notification](
	Id uniqueidentifier primary key NOT NULL,
	AccountId uniqueidentifier foreign key references Account(Id) NOT NULL,
	Title nvarchar(255) NOT NULL,
	Content nvarchar(255) NOT NULL,
	SendDate datetime NOT NULL default getdate()
);
GO


--Table category
DROP TABLE IF EXISTS Category;
GO
CREATE TABLE Category(
	Id uniqueidentifier primary key NOT NULL,
	CategoryName nvarchar(100) NOT NULL,
);
GO




--Table vehicle
DROP TABLE IF EXISTS Vehicle;
GO
CREATE TABLE Vehicle(
	Id uniqueidentifier primary key NOT NULL,
	VehicleName nvarchar(100) NOT NULL,
);
GO

--Table warranty (Bảo hành)
DROP TABLE IF EXISTS Warranty;
GO
CREATE TABLE Warranty(
	Id uniqueidentifier primary key NOT NULL,
	Duration int NOT NULL,		--số tháng bảo hành
	Description nvarchar(max) NOT NULL
);
GO

--Table discount
DROP TABLE IF EXISTS Discount;
GO
CREATE TABLE Discount(
	Id uniqueidentifier primary key NOT NULL,
	Title nvarchar(255) NOT NULL,
	DiscountAmount int NOT NULL,	-- phần trăm giảm giá
	StartDate datetime NOT NULL,
	EndDate datetime NOT NULL,
	ImageUrl varchar(max) NOT NULL,
	Description nvarchar(max) NOT NULL,
	Status nvarchar(100) NOT NULL
);
GO

--Table repair service
DROP TABLE IF EXISTS RepairService;
GO
CREATE TABLE RepairService(
	Id uniqueidentifier primary key NOT NULL,
	Name nvarchar(255) NOT NULL,
	Price int NOT NULL,
	Description nvarchar(max) NOT NULL,
	Status nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL default getdate()
);
GO


--Table product
DROP TABLE IF EXISTS MotobikeProduct;
GO
CREATE TABLE MotobikeProduct(
	Id uniqueidentifier primary key NOT NULL,
	RepairServiceId uniqueidentifier foreign key references RepairService(Id),
	DiscountId uniqueidentifier foreign key references Discount(Id),
	WarrantyId uniqueidentifier foreign key references Warranty(Id),
	CategoryId uniqueidentifier foreign key references Category(Id),
	Name nvarchar(255) NOT NULL,
	PriceCurrent int NOT NULL,
	Quantity int NOT NULL,
	Description nvarchar(max) NOT NULL,
	Status nvarchar(100) NOT NULL,
	UpdateAt datetime,
	CreateAt datetime NOT NULL default getdate()
);
GO

--Table product price
DROP TABLE IF EXISTS MotobikeProductPrice;
GO
CREATE TABLE MotobikeProductPrice(
	Id uniqueidentifier primary key NOT NULL,
	MotobikeProductId uniqueidentifier foreign key references MotobikeProduct(Id) NOT NULL,
	DateApply datetime NOT NULL,
	PriceCurrent int NOT NULL,
	CreateAt datetime NOT NULL default getdate()
);
GO

--Table product image
DROP TABLE IF EXISTS [Image];
GO
CREATE TABLE [Image](
	Id uniqueidentifier primary key NOT NULL,
	MotobikeProductId uniqueidentifier foreign key references MotobikeProduct(Id),
	RepairServiceId uniqueidentifier foreign key references RepairService(Id) ,
	Thumbnail bit NOT NULL default 0,
	ImageUrl varchar(max) NOT NULL
);
GO


--Table product vehicle type
DROP TABLE IF EXISTS ProductVehicleType;
GO
CREATE TABLE ProductVehicleType (
    MotobikeProductId uniqueidentifier foreign key references MotobikeProduct(Id) NOT NULL,
    VehicleId uniqueidentifier foreign key references Vehicle(Id) NOT NULL,
	Primary key (MotobikeProductId, VehicleId)
);
GO


--Table feedback product
DROP TABLE IF EXISTS FeedbackProduct;
GO
CREATE TABLE FeedbackProduct(
	Id uniqueidentifier primary key NOT NULL,
	CustomerId uniqueidentifier foreign key references CustomerAccount(AccountId),
	MotobikeProductId uniqueidentifier foreign key references MotobikeProduct(Id) NOT NULL,
	Title nvarchar(255),
	Rate int NOT NULL,
	Content nvarchar(max) NOT NULL,
	UpdateAt datetime,
	CreateAt datetime NOT NULL default getdate()
);
GO

--Table cart
DROP TABLE IF EXISTS Cart;
GO
CREATE TABLE Cart (
    Id uniqueidentifier primary key NOT NULL,
	CustomerId uniqueidentifier unique foreign key references CustomerAccount(AccountId) NOT NULL
);
GO

--Table cart Item
DROP TABLE IF EXISTS CartItem;
GO
CREATE TABLE CartItem (
	CartId uniqueidentifier foreign key references Cart(Id) NOT NULL,
	MotobikeProductId uniqueidentifier foreign key references MotobikeProduct(Id) NOT NULL,
	Quantity int NOT NULL,
	CreateAt datetime NOT NULL default getdate(),
	Primary key (CartId, MotobikeProductId)
);
GO


--Table product order
DROP TABLE IF EXISTS OnlineOrder;
GO
CREATE TABLE OnlineOrder(
	Id uniqueidentifier primary key NOT NULL,
	CustomerId uniqueidentifier foreign key references CustomerAccount(AccountId) NOT NULL,
	StaffId uniqueidentifier foreign key references StaffAccount(AccountId),
	PhoneNumber varchar(30) NOT NULL,
	Address nvarchar(255) NOT NULL,
	PaymentMethod nvarchar(50) NOT NULL,
	Status nvarchar(100) NOT NULL,
	TotalAmount int NOT NULL,
	CancellationReason nvarchar(max),
	CancellationDate datetime,
	OrderDate datetime NOT NULL default getdate()
);
GO

--Table product order detail
DROP TABLE IF EXISTS OnlineOrderDetail;
GO
CREATE TABLE OnlineOrderDetail(
	OnlineOrderId uniqueidentifier foreign key references OnlineOrder(Id) NOT NULL,
	MotobikeProductId uniqueidentifier foreign key references MotobikeProduct(Id) NOT NULL,
	Price int NOT NULL,
	Quantity int NOT NULL,
	SubTotalAmount int NOT NULL,
	CreateAt datetime NOT NULL default getdate(),
	Primary key (OnlineOrderId, MotobikeProductId)
);
GO


--Table repair service order
DROP TABLE IF EXISTS InStoreOrder;
GO
CREATE TABLE InStoreOrder(
	Id varchar(255) primary key NOT NULL,
	TellerId uniqueidentifier foreign key references TellerAccount(AccountId) NOT NULL,
	StaffId uniqueidentifier foreign key references StaffAccount(AccountId) NOT NULL,
	CustomerName nvarchar(100),
	CustomerPhone varchar(30) NOT NULL,
	LicensePlate varchar(50), -- Thêm cột biển số xe
	Status nvarchar(100) NOT NULL,
	TotalAmount int NOT NULL,
	OrderType nvarchar(100) NOT NULL, --Purchase or Repair
	OrderDate datetime NOT NULL default getdate()
);
GO


--Table repair order item
DROP TABLE IF EXISTS InStoreOrderDetail;
GO
CREATE TABLE InStoreOrderDetail(
	Id uniqueidentifier primary key NOT NULL,
	InStoreOrderId varchar(255) foreign key references InStoreOrder(Id) NOT NULL,
	RepairServiceId uniqueidentifier foreign key references RepairService(Id),
	MotobikeProductId uniqueidentifier foreign key references MotobikeProduct(Id),
	ProductQuantity int,
	ProductPrice int,
	ServicePrice int,
	WarrantyPeriod datetime NOT NULL,
	RepairCount int NOT NULL default 1,
	CreateAt datetime NOT NULL default getdate()
);
GO

--Table bill
DROP TABLE IF EXISTS Bill;
GO
CREATE TABLE Bill(
	Id uniqueidentifier primary key NOT NULL,
	InStoreOrderId varchar(255) unique foreign key references InStoreOrder(Id) NOT NULL,
	PaymentMethod nvarchar(50) NOT NULL,
	BillDate datetime NOT NULL default getdate()
);
GO

--Table booking
DROP TABLE IF EXISTS RepairBooking;
GO
CREATE TABLE RepairBooking(
	Id uniqueidentifier primary key NOT NULL,
	CustomerId uniqueidentifier foreign key references CustomerAccount(AccountId) NOT NULL,
	DateBook datetime NOT NULL,
	Description nvarchar(50) NOT NULL,
	CancellationReason nvarchar(max),
	CancellationDate datetime,
	Status nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL default getdate()
);
GO

