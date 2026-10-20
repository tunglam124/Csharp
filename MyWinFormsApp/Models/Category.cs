namespace MyWinFormsApp.Models;

/// <summary>
/// Đại diện cho bảng Categories trong CSDL PosDB
/// </summary>
public class Category
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
