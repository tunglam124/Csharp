using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace MyWinFormsApp.DataAccess;

/// <summary>
/// Quản lý chuỗi kết nối và cung cấp SqlConnection từ appsettings.json
/// </summary>
public static class DbHelper
{
    private static readonly string _connectionString;

    static DbHelper()
    {
        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _connectionString = config.GetConnectionString("PosDbConnection")
                ?? "Server=.\\SQLEXPRESS;Database=PosDB;Trusted_Connection=True;TrustServerCertificate=True;";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi nạp cấu hình appsettings.json: {ex.Message}", "Cảnh báo cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _connectionString = "Server=.\\SQLEXPRESS;Database=PosDB;Trusted_Connection=True;TrustServerCertificate=True;";
        }
    }

    /// <summary>
    /// Lấy chuỗi kết nối cơ sở dữ liệu
    /// </summary>
    public static string ConnectionString => _connectionString;

    /// <summary>
    /// Tạo một kết nối SqlConnection mới
    /// </summary>
    public static SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
