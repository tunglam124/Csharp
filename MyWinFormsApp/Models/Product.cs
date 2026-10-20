namespace MyWinFormsApp.Models;

/// <summary>
/// Đại diện cho bảng Products trong CSDL PosDB
/// </summary>
public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImagePath { get; set; }
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
