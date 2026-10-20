-- ==============================================================
-- Dự án: Hệ thống POS Nhà hàng / Cafe
-- Giai đoạn 7: Bổ sung danh mục Meals, Snacks & 10 món ăn đặc trưng
-- ==============================================================

USE PosDB;
GO

-- 1. Đảm bảo có danh mục Meals (Món chính) và Snacks (Ăn vặt)
IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Meals')
    INSERT INTO Categories (CategoryName) VALUES (N'Meals');

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Snacks')
    INSERT INTO Categories (CategoryName) VALUES (N'Snacks');
GO

-- 2. Lấy ID của danh mục Meals và Snacks
DECLARE @MealId INT = (SELECT TOP 1 CategoryID FROM Categories WHERE CategoryName = N'Meals');
DECLARE @SnackId INT = (SELECT TOP 1 CategoryID FROM Categories WHERE CategoryName = N'Snacks');

-- 3. Bổ sung 5 Món chính đặc trưng (Meals)
IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Cơm tấm sườn bì chả')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) 
    VALUES (N'Cơm tấm sườn bì chả', 45000, 'images/com_tam.png', @MealId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Phở bò tái lăn')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) 
    VALUES (N'Phở bò tái lăn', 55000, 'images/pho_bo.png', @MealId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Bún bò Huế đặc biệt')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) 
    VALUES (N'Bún bò Huế đặc biệt', 50000, 'images/bun_bo.png', @MealId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Bánh mì thịt nướng')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) 
    VALUES (N'Bánh mì thịt nướng', 30000, 'images/banh_mi.png', @MealId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Mì xào giòn hải sản')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) 
    VALUES (N'Mì xào giòn hải sản', 55000, 'images/mi_xao.png', @MealId);

-- 4. Bổ sung 5 Món ăn vặt đặc trưng (Snacks)
IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Bánh tráng trộn tôm thịt')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) 
    VALUES (N'Bánh tráng trộn tôm thịt', 25000, 'images/banh_trang.png', @SnackId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Cá viên chiên mắm tỏi')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) 
    VALUES (N'Cá viên chiên mắm tỏi', 35000, 'images/ca_vien.png', @SnackId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Nem chua rán Hà Nội')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) 
    VALUES (N'Nem chua rán Hà Nội', 30000, 'images/nem_chua.png', @SnackId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Gà popcorn lắc phô mai')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) 
    VALUES (N'Gà popcorn lắc phô mai', 40000, 'images/ga_popcorn.png', @SnackId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = N'Khoai tây lốc xoáy giòn')
    INSERT INTO Products (ProductName, Price, ImagePath, CategoryID) 
    VALUES (N'Khoai tây lốc xoáy giòn', 25000, 'images/khoai_tay_loc_xoay.png', @SnackId);
GO
