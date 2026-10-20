namespace MyWinFormsApp.Models;

/// <summary>
/// Đại diện cho thông tin tóm tắt đơn hàng đang tạm giữ (Pending)
/// </summary>
public class PendingOrderModel
{
    public int OrderID { get; set; }
    public string TableNumber { get; set; } = "Bàn 01";
    public DateTime OrderDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string CashierName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public string ItemsSummary { get; set; } = string.Empty;
}
