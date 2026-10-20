namespace MyWinFormsApp.Models;

/// <summary>
/// Đại diện cho tài khoản người dùng trong hệ thống (Admin / Cashier)
/// </summary>
public class User
{
    public int UserID { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "Cashier"; // "Admin" hoặc "Cashier"
    public bool IsActive { get; set; } = true;
}
