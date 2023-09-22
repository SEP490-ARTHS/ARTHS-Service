USE master
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

--Table account
DROP TABLE IF EXISTS Account;
GO
CREATE TABLE Account(
	Id uniqueidentifier primary key NOT NULL,
	RoleId uniqueidentifier foreign key references AccountRole(Id) NOT NULL,
	FullName nvarchar(255) NOT NULL,
	PhoneNumber varchar(30) unique NOT NULL,
	PasswordHash varchar(255) NOT NULL,
	Avatar varchar(max),
	Status nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL default getdate()
);
GO

--Table owner
DROP TABLE IF EXISTS OwnerAccount;
GO
CREATE TABLE OwnerAccount(
	Id uniqueidentifier primary key NOT NULL,
	AccountId uniqueidentifier unique foreign key references Account(Id) NOT NULL
);
GO

--Table staff
DROP TABLE IF EXISTS StaffAccount;
GO
CREATE TABLE StaffAccount(
	Id uniqueidentifier primary key NOT NULL,
	AccountId uniqueidentifier unique foreign key references Account(Id) NOT NULL
);
GO

--Table customer
DROP TABLE IF EXISTS CustomerAccount;
GO
CREATE TABLE CustomerAccount(
	Id uniqueidentifier primary key NOT NULL,
	AccountId uniqueidentifier unique foreign key references Account(Id) NOT NULL,
	Address nvarchar(255) NOT NULL
);
GO

--Table feedback staff
DROP TABLE IF EXISTS FeedbackStaff;
GO
CREATE TABLE FeedbackStaff(
	Id uniqueidentifier primary key NOT NULL,
	CustomerId uniqueidentifier foreign key references CustomerAccount(Id),
	StaffId uniqueidentifier foreign key references StaffAccount(Id) NOT NULL,
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
	Term nvarchar(max) NOT NULL
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
	ImageUrl varchar(max) NOT NULL,
	Description nvarchar(max) NOT NULL,
	Status nvarchar(100) NOT NULL,
	UpdateAt datetime,
	CreateAt datetime NOT NULL default getdate()
);
GO


--Table product
DROP TABLE IF EXISTS Product;
GO
CREATE TABLE Product(
	Id uniqueidentifier primary key NOT NULL,
	RepairServiceId uniqueidentifier foreign key references RepairService(Id),
	DiscountId uniqueidentifier foreign key references Discount(Id),
	WarrantyId uniqueidentifier foreign key references Warranty(Id),
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
DROP TABLE IF EXISTS ProductPrice;
GO
CREATE TABLE ProductPrice(
	Id uniqueidentifier primary key NOT NULL,
	ProductId uniqueidentifier foreign key references Product(Id) NOT NULL,
	DateApply datetime NOT NULL,
	PriceCurrent int NOT NULL
);
GO

--Table product image
DROP TABLE IF EXISTS ProductImage;
GO
CREATE TABLE ProductImage(
	Id uniqueidentifier primary key NOT NULL,
	ProductId uniqueidentifier foreign key references Product(Id) NOT NULL,
	Thumbnail bit NOT NULL default 0,
	ImageUrl varchar(max) NOT NULL
);
GO

--Table product category
DROP TABLE IF EXISTS ProductCategory;
GO
CREATE TABLE ProductCategory (
    ProductId uniqueidentifier foreign key references Product(Id) NOT NULL,
    CategoryId uniqueidentifier foreign key references Category(Id) NOT NULL,
	Primary key (ProductId, CategoryId)
);
GO

--Table product vehicle type
DROP TABLE IF EXISTS ProductVehicleType;
GO
CREATE TABLE ProductVehicleType (
    ProductId uniqueidentifier foreign key references Product(Id) NOT NULL,
    VehicleId uniqueidentifier foreign key references Vehicle(Id) NOT NULL,
	Primary key (ProductId, VehicleId)
);
GO


--Table feedback product
DROP TABLE IF EXISTS FeedbackProduct;
GO
CREATE TABLE FeedbackProduct(
	Id uniqueidentifier primary key NOT NULL,
	CustomerId uniqueidentifier foreign key references CustomerAccount(Id),
	ProductId uniqueidentifier foreign key references Product(Id) NOT NULL,
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
	CustomerId uniqueidentifier unique foreign key references CustomerAccount(Id) NOT NULL
);
GO

--Table cart Item
DROP TABLE IF EXISTS CartItem;
GO
CREATE TABLE CartItem (
	CartId uniqueidentifier foreign key references Cart(Id) NOT NULL,
	ProductId uniqueidentifier foreign key references Product(Id) NOT NULL,
	Quantity int NOT NULL,
	CreateAt datetime NOT NULL default getdate(),
	Primary key (CartId, ProductId)
);
GO


--Table product order
DROP TABLE IF EXISTS ProductOrder;
GO
CREATE TABLE ProductOrder(
	Id uniqueidentifier primary key NOT NULL,
	CustomerId uniqueidentifier foreign key references CustomerAccount(Id) NOT NULL,
	StaffId uniqueidentifier foreign key references StaffAccount(Id),
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
DROP TABLE IF EXISTS ProductOrderDetail;
GO
CREATE TABLE ProductOrderDetail(
	ProductOrderId uniqueidentifier foreign key references ProductOrder(Id) NOT NULL,
	ProductId uniqueidentifier foreign key references Product(Id) NOT NULL,
	Price int NOT NULL,
	Quantity int NOT NULL,
	SubTotalAmount int NOT NULL,
	CreateAt datetime NOT NULL default getdate(),
	Primary key (ProductOrderId, ProductId)
);
GO


--Table repair service order
DROP TABLE IF EXISTS RepairOrder;
GO
CREATE TABLE RepairOrder(
	Id uniqueidentifier primary key NOT NULL,
	StaffId uniqueidentifier foreign key references StaffAccount(Id) NOT NULL,
	CustomerName nvarchar(100),
	CustomerPhone varchar(30) NOT NULL,
	Status nvarchar(100) NOT NULL,
	TotalAmount int NOT NULL,
	OrderDate datetime NOT NULL default getdate()
);
GO

--Table repair order item
DROP TABLE IF EXISTS RepairOrderDetail;
GO
CREATE TABLE RepairOrderDetail(
	Id uniqueidentifier primary key NOT NULL,
	RepairOrderId uniqueidentifier foreign key references RepairOrder(Id) NOT NULL,
	RepairServiceId uniqueidentifier foreign key references RepairService(Id),
	ProductId uniqueidentifier foreign key references Product(Id),
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
	RepairOrderId uniqueidentifier unique foreign key references RepairOrder(Id) NOT NULL,
	PaymentMethod nvarchar(50) NOT NULL,
	BillDate datetime NOT NULL default getdate()
);
GO







