using Microsoft.Data.SqlClient;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.DataAccess;

/// <summary>
/// Xử lý truy vấn dữ liệu cho bảng Categories
/// </summary>
public static class CategoryDAL
{
    /// <summary>
    /// Lấy danh sách tất cả các danh mục món (async/await)
    /// </summary>
    public static async Task<List<Category>> GetCategories()
    {
        var list = new List<Category>();
        const string query = "SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryID;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Category
                {
                    CategoryID = reader.GetInt32(0),
                    CategoryName = reader.GetString(1)
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi truy vấn danh mục: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return list;
    }
}
