# C# POS Application (Point of Sale)

Hệ thống quản lý bán hàng (POS) xây dựng bằng C# Windows Forms và Microsoft SQL Server.

## Cấu trúc dự án

- **`Database/`**: Các file script SQL khởi tạo và cập nhật cơ sở dữ liệu (`PosDB_Init.sql`, `PosDB_Update_Menu.sql`).
- **`MyWinFormsApp/`**: Mã nguồn ứng dụng WinForms (.NET):
  - `Forms/`: Giao diện POS, quản lý sản phẩm, đơn hàng, người dùng, báo cáo doanh thu.
  - `DataAccess/`: Tầng truy xuất dữ liệu (DAL & Repository).
  - `Models/`: Các lớp dữ liệu thực thể.
  - `Printing/`: Xử lý in hóa đơn.
  - `UserControls/`: Các custom UI control (thẻ sản phẩm,...).

## Hướng dẫn cài đặt & khởi chạy

1. **Cơ sở dữ liệu**:
   - Sử dụng SQL Server / SQL Server Express.
   - Thực thi các script trong thư mục `Database/` để tạo cơ sở dữ liệu `PosDB`.
2. **Cấu hình chuỗi kết nối**:
   - Mở file [appsettings.json](file:///C:/Users/tungl/OneDrive/Desktop/C%23/MyWinFormsApp/appsettings.json) và kiểm tra connection string `PosDbConnection`.
3. **Mở và chạy dự án**:
   - Mở thư mục dự án hoặc file `.csproj` bằng Visual Studio hoặc VS Code / JetBrains Rider.
   - Build và chạy ứng dụng.
