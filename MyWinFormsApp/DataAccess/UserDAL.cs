using System.Data;
using Microsoft.Data.SqlClient;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.DataAccess;

/// <summary>
/// Quản lý dữ liệu tài khoản và xác thực người dùng trong bảng Users
/// </summary>
public static class UserDAL
{
    /// <summary>
    /// Đăng nhập: Kiểm tra username và password
    /// </summary>
    public static async Task<User?> AuthenticateAsync(string username, string password)
    {
        const string query = @"
            SELECT UserID, Username, Password, FullName, Role, IsActive 
            FROM Users 
            WHERE Username = @Username AND Password = @Password AND IsActive = 1;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username.Trim() });
            cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 100) { Value = password.Trim() });

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    FullName = reader.GetString(3),
                    Role = reader.GetString(4),
                    IsActive = reader.GetBoolean(5)
                };
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi kết nối khi đăng nhập: {ex.Message}", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return null;
    }

    /// <summary>
    /// Lấy danh sách toàn bộ người dùng
    /// </summary>
    public static async Task<List<User>> GetAllUsersAsync()
    {
        var list = new List<User>();
        const string query = "SELECT UserID, Username, Password, FullName, Role, IsActive FROM Users ORDER BY UserID;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new User
                {
                    UserID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    FullName = reader.GetString(3),
                    Role = reader.GetString(4),
                    IsActive = reader.GetBoolean(5)
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh sách người dùng: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return list;
    }

    /// <summary>
    /// Thêm người dùng mới
    /// </summary>
    public static async Task<bool> AddUserAsync(User user)
    {
        const string query = @"
            INSERT INTO Users (Username, Password, FullName, Role, IsActive)
            VALUES (@Username, @Password, @FullName, @Role, @IsActive);";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = user.Username.Trim() });
            cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 100) { Value = user.Password.Trim() });
            cmd.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = user.FullName.Trim() });
            cmd.Parameters.Add(new SqlParameter("@Role", SqlDbType.NVarChar, 20) { Value = user.Role });
            cmd.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = user.IsActive });

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi thêm người dùng: {ex.Message}", "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    public static async Task<bool> UpdateUserAsync(User user)
    {
        const string query = @"
            UPDATE Users 
            SET Password = @Password,
                FullName = @FullName,
                Role = @Role,
                IsActive = @IsActive
            WHERE UserID = @UserID;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = user.UserID });
            cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 100) { Value = user.Password.Trim() });
            cmd.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = user.FullName.Trim() });
            cmd.Parameters.Add(new SqlParameter("@Role", SqlDbType.NVarChar, 20) { Value = user.Role });
            cmd.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = user.IsActive });

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi cập nhật người dùng: {ex.Message}", "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>
    /// Xóa tài khoản người dùng
    /// </summary>
    public static async Task<bool> DeleteUserAsync(int userId)
    {
        const string query = "DELETE FROM Users WHERE UserID = @UserID;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userId });

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xóa người dùng: {ex.Message}", "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}
