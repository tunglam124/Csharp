-- ==============================================================
-- Dự án: Hệ thống POS Nhà hàng / Cafe
-- Giai đoạn 1: Khởi tạo Database PosDB và dữ liệu mẫu
-- ==============================================================

-- 1. Tạo Database nếu chưa tồn tại
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'PosDB')
BEGIN
    CREATE DATABASE PosDB;
END
GO

USE PosDB;
GO

-- 2. Bảng Danh mục (Categories)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Categories')
BEGIN
    CREATE TABLE Categories (
        CategoryID INT IDENTITY(1,1) NOT NULL,
        CategoryName NVARCHAR(100) NOT NULL,
        CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (CategoryID)
    );
END
GO

-- 3. Bảng Sản phẩm (Products)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Products')
BEGIN
    CREATE TABLE Products (
        ProductID INT IDENTITY(1,1) NOT NULL,
        ProductName NVARCHAR(150) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        ImagePath NVARCHAR(500) NULL,
        CategoryID INT NOT NULL,
        CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (ProductID),
        CONSTRAINT CK_Products_Price CHECK (Price >= 0),
        -- Không cho phép xóa danh mục nếu còn sản phẩm thuộc danh mục đó
        CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID) 
            REFERENCES Categories(CategoryID) ON DELETE NO ACTION
    );
END
GO

-- 4. Bảng Đơn hàng (Orders)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Orders')
BEGIN
    CREATE TABLE Orders (
        OrderID INT IDENTITY(1,1) NOT NULL,
        TableNumber NVARCHAR(50) NULL, -- Cho phép chuỗi như 'Bàn 01', 'Mang về'
        OrderDate DATETIME NOT NULL CONSTRAINT DF_Orders_OrderDate DEFAULT GETDATE(),
        SubTotal DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_SubTotal DEFAULT 0,
        TaxAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_TaxAmount DEFAULT 0,
        DiscountAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_DiscountAmount DEFAULT 0,
        TotalAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_TotalAmount DEFAULT 0,
        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Orders_Status DEFAULT 'Pending',
        CashierName NVARCHAR(100) NULL,
        CONSTRAINT PK_Orders PRIMARY KEY CLUSTERED (OrderID),
        CONSTRAINT CK_Orders_Status CHECK (Status IN ('Pending', 'Completed', 'Canceled')),
        CONSTRAINT CK_Orders_TotalAmount CHECK (TotalAmount >= 0)
    );
END
GO

-- 5. Bảng Chi tiết đơn hàng (OrderDetails)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'OrderDetails')
BEGIN
    CREATE TABLE OrderDetails (
        DetailID INT IDENTITY(1,1) NOT NULL,
        OrderID INT NOT NULL,
        ProductID INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        LineTotal DECIMAL(18,2) NOT NULL,
        Notes NVARCHAR(255) NULL, -- Ví dụ: 'Ít đường', 'Không hành'
        CONSTRAINT PK_OrderDetails PRIMARY KEY CLUSTERED (DetailID),
        CONSTRAINT CK_OrderDetails_Quantity CHECK (Quantity > 0),
        CONSTRAINT CK_OrderDetails_UnitPrice CHECK (UnitPrice >= 0),
        -- Khi xóa đơn hàng (hủy/dọn dẹp), xóa toàn bộ chi tiết liên kết
        CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderID) 
            REFERENCES Orders(OrderID) ON DELETE CASCADE,
        -- Không cho phép xóa sản phẩm nếu đã phát sinh chi tiết đơn hàng
        CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductID) 
            REFERENCES Products(ProductID) ON DELETE NO ACTION
    );
END
GO

-- 6. Tạo Index tối ưu hóa truy vấn
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Products_CategoryID' AND object_id = OBJECT_ID('Products'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Products_CategoryID 
    ON Products(CategoryID);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_OrderDetails_OrderID' AND object_id = OBJECT_ID('OrderDetails'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_OrderDetails_OrderID 
    ON OrderDetails(OrderID);
END
GO

-- 7. Chèn dữ liệu mẫu (Chạy an toàn không trùng lặp)
-- Chèn 3 Danh mục: Drinks, Meals, Sides
IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Drinks')
    INSERT INTO Categories (CategoryName) VALUES (N'Drinks');

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Meals')
    INSERT INTO Categories (CategoryName) VALUES (N'Meals');

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Sides')
    INSERT INTO Categories (CategoryName) VALUES (N'Sides');
GO

-- Chèn 10 Sản phẩm mẫu
DECLARE @DrinkId INT = (SELECT CategoryID FROM Categories WHERE CategoryName = N'Drinks');
DECLARE @MealId INT = (SELECT CategoryID FROM Categories WHERE CategoryName = N'Meals');
DECLARE @SideId INT = (SELECT CategoryID FROM Categories WHERE CategoryName = N'Sides');

-- 4 Món nước
IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Cà phê đen')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) VALUES (N'Cà phê đen', 25000, 'images/ca_phe_den.png', @DrinkId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Cà phê sữa')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) VALUES (N'Cà phê sữa', 29000, 'images/ca_phe_sua.png', @DrinkId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Trà đào cam sả')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) VALUES (N'Trà đào cam sả', 39000, 'images/tra_dao.png', @DrinkId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Coca Cola')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) VALUES (N'Coca Cola', 18000, 'images/coca.png', @DrinkId);

-- 4 Món chính
IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Burger bò phô mai')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) VALUES (N'Burger bò phô mai', 55000, 'images/burger_bo.png', @MealId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Gà rán sốt cay')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) VALUES (N'Gà rán sốt cay', 45000, 'images/ga_ran.png', @MealId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Cơm gà xối mỡ')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) VALUES (N'Cơm gà xối mỡ', 50000, 'images/com_ga.png', @MealId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Mì Ý sốt bò bằm')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) VALUES (N'Mì Ý sốt bò bằm', 59000, 'images/mi_y.png', @MealId);

-- 2 Món ăn kèm
IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Khoai tây chiên')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) VALUES (N'Khoai tây chiên', 30000, 'images/khoai_tay.png', @SideId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Phô mai que (3 chiếc)')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) VALUES (N'Phô mai que (3 chiếc)', 35000, 'images/pho_mai_que.png', @SideId);
GO
