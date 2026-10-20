namespace MyWinFormsApp.UserControls;

/// <summary>
/// Dữ liệu truyền đi khi người dùng click vào thẻ sản phẩm
/// </summary>
public class ProductClickedEventArgs : EventArgs
{
    public int Id { get; }
    public string ProductName { get; }
    public decimal Price { get; }

    public ProductClickedEventArgs(int id, string productName, decimal price)
    {
        Id = id;
        ProductName = productName;
        Price = price;
    }
}
