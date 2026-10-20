namespace MyWinFormsApp.Models;

/// <summary>
/// Quản lý phiên làm việc hiện tại của người dùng sau khi đăng nhập
/// </summary>
public static class UserSession
{
    public static User? CurrentUser { get; set; }

    public static bool IsLoggedIn => CurrentUser != null;

    public static bool IsAdmin => 
        CurrentUser != null && 
        string.Equals(CurrentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);

    public static string DisplayName => 
        CurrentUser != null ? $"{CurrentUser.FullName} ({CurrentUser.Role})" : "Chưa đăng nhập";

    public static void Logout()
    {
        CurrentUser = null;
    }
}
