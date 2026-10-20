using System.Data;
using Microsoft.Data.SqlClient;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.DataAccess;

/// <summary>
/// Xử lý truy vấn và thao tác dữ liệu cho bảng Products (CRUD)
/// </summary>
public static class ProductDAL
{
    /// <summary>
    /// Lấy danh sách sản phẩm theo CategoryID (async/await, parameterized query)
    /// </summary>
    public static async Task<List<Product>> GetProductsByCategory(int categoryId)
    {
        var list = new List<Product>();
        const string query = @"
            SELECT ProductID, ProductName, Price, ImagePath, CategoryID 
            FROM Products 
            WHERE CategoryID = @CategoryID 
            ORDER BY ProductID DESC;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@CategoryID", SqlDbType.Int) { Value = categoryId });

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Product
                {
                    ProductID = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    ImagePath = reader.IsDBNull(3) ? null : reader.GetString(3),
                    CategoryID = reader.GetInt32(4)
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi truy vấn sản phẩm: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return list;
    }

    /// <summary>
    /// Lấy tất cả sản phẩm kèm tên danh mục cho màn hình quản lý món
    /// </summary>
    public static async Task<List<Product>> GetAllProductsAsync()
    {
        var list = new List<Product>();
        const string query = @"
            SELECT p.ProductID, p.ProductName, p.Price, p.ImagePath, p.CategoryID, c.CategoryName
            FROM Products p
            INNER JOIN Categories c ON p.CategoryID = c.CategoryID
            ORDER BY p.ProductID DESC;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Product
                {
                    ProductID = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    ImagePath = reader.IsDBNull(3) ? null : reader.GetString(3),
                    CategoryID = reader.GetInt32(4),
                    CategoryName = reader.GetString(5)
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh sách món: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return list;
    }

    /// <summary>
    /// Thêm sản phẩm mới vào CSDL
    /// </summary>
    public static async Task<int> AddProductAsync(Product product)
    {
        const string query = @"
            INSERT INTO Products (ProductName, Price, ImagePath, CategoryID)
            VALUES (@ProductName, @Price, @ImagePath, @CategoryID);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@ProductName", SqlDbType.NVarChar, 150) { Value = product.ProductName });
            cmd.Parameters.Add(new SqlParameter("@Price", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = product.Price });
            cmd.Parameters.Add(new SqlParameter("@ImagePath", SqlDbType.NVarChar, 500) { Value = (object?)product.ImagePath ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CategoryID", SqlDbType.Int) { Value = product.CategoryID });

            await conn.OpenAsync();
            object? result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : -1;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi thêm sản phẩm: {ex.Message}", "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return -1;
        }
    }

    /// <summary>
    /// Cập nhật thông tin sản phẩm đã có
    /// </summary>
    public static async Task<bool> UpdateProductAsync(Product product)
    {
        const string query = @"
            UPDATE Products 
            SET ProductName = @ProductName,
                Price = @Price,
                ImagePath = @ImagePath,
                CategoryID = @CategoryID
            WHERE ProductID = @ProductID;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int) { Value = product.ProductID });
            cmd.Parameters.Add(new SqlParameter("@ProductName", SqlDbType.NVarChar, 150) { Value = product.ProductName });
            cmd.Parameters.Add(new SqlParameter("@Price", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = product.Price });
            cmd.Parameters.Add(new SqlParameter("@ImagePath", SqlDbType.NVarChar, 500) { Value = (object?)product.ImagePath ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CategoryID", SqlDbType.Int) { Value = product.CategoryID });

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi cập nhật sản phẩm: {ex.Message}", "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>
    /// Xóa sản phẩm khỏi CSDL
    /// </summary>
    public static async Task<bool> DeleteProductAsync(int productId)
    {
        const string query = "DELETE FROM Products WHERE ProductID = @ProductID;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int) { Value = productId });

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        catch (SqlException sqlEx) when (sqlEx.Number == 547)
        {
            MessageBox.Show("Món ăn này đã có trong lịch sử đơn hàng, không thể xóa để bảo toàn dữ liệu kế toán!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xóa sản phẩm: {ex.Message}", "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>
    /// Lấy thông tin một sản phẩm theo ID
    /// </summary>
    public static async Task<Product?> GetProductByIdAsync(int productId)
    {
        const string query = "SELECT ProductID, ProductName, Price, ImagePath, CategoryID FROM Products WHERE ProductID = @ProductID;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int) { Value = productId });

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Product
                {
                    ProductID = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    ImagePath = reader.IsDBNull(3) ? null : reader.GetString(3),
                    CategoryID = reader.GetInt32(4)
                };
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi truy vấn sản phẩm #{productId}: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return null;
    }
}
