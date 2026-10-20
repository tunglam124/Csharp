namespace MyWinFormsApp.Models;

/// <summary>
/// Đại diện cho bản ghi đơn hàng hiển thị trong báo cáo doanh thu
/// </summary>
public class OrderReportModel
{
    public int OrderID { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string CashierName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
