using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Printing;

/// <summary>
/// Quản lý in và xem trước hóa đơn nhiệt khổ 80mm (vùng in thực tế ~72mm ≈ 283px) kèm Topping và Tiền thừa
/// </summary>
public class ReceiptPrinter
{
    #region Properties & Fields

    public int OrderId { get; set; }
    public string ReceiptCode { get; set; }
    public DateTime OrderDate { get; set; }
    public string CashierName { get; set; }
    public string TableNumber { get; set; }
    public List<CartItem> Items { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = "Tiền mặt";
    public decimal CustomerCash { get; set; } = 0m;
    public decimal ChangeAmount { get; set; } = 0m;

    private readonly CultureInfo _cultureVi = new("vi-VN");
    private const int PrintableWidth = 283; // 72mm ở tỉ lệ 100 DPI
    private const string StoreName = "POS FASTFOOD & CAFE";
    private const string StoreAddress = "123 Đường Công Nghệ, Q.1, TP.HCM";
    private const string StorePhone = "Hotline: 0901.234.567";

    #endregion

    #region Constructor

    public ReceiptPrinter(
        int orderId,
        string receiptCode,
        DateTime orderDate,
        string cashierName,
        string tableNumber,
        IEnumerable<CartItem> items,
        decimal subtotal,
        decimal tax,
        decimal discount,
        decimal total,
        string paymentMethod = "Tiền mặt",
        decimal customerCash = 0m,
        decimal changeAmount = 0m)
    {
        OrderId = orderId;
        ReceiptCode = receiptCode;
        OrderDate = orderDate;
        CashierName = cashierName;
        TableNumber = tableNumber;
        Items = items.ToList();
        Subtotal = subtotal;
        Tax = tax;
        Discount = discount;
        Total = total;
        PaymentMethod = paymentMethod;
        CustomerCash = customerCash;
        ChangeAmount = changeAmount;
    }

    #endregion

    #region Public Print Methods

    public void Print()
    {
        try
        {
            using var printDoc = CreatePrintDocument();
            printDoc.Print();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi in hóa đơn: {ex.Message}", "Lỗi in ấn", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void PrintPreview(IWin32Window? owner = null)
    {
        try
        {
            using var printDoc = CreatePrintDocument();
            using var previewDlg = new PrintPreviewDialog
            {
                Document = printDoc,
                Width = 460,
                Height = 680,
                StartPosition = FormStartPosition.CenterParent,
                Text = $"Xem trước hóa đơn - {ReceiptCode}"
            };

            if (owner != null)
                previewDlg.ShowDialog(owner);
            else
                previewDlg.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xem trước hóa đơn: {ex.Message}", "Lỗi in ấn", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    #endregion

    #region PrintDocument & PrintPage Configuration

    private PrintDocument CreatePrintDocument()
    {
        // Ước lượng chiều cao giấy
        int noteLinesCount = Items.Count(x => !string.IsNullOrWhiteSpace(x.Notes));
        int estimatedHeight = 180 + (Items.Count * 22) + (noteLinesCount * 16) + 180 + 90;

        var printDoc = new PrintDocument();
        printDoc.DefaultPageSettings.PaperSize = new PaperSize("CustomThermal80mm", PrintableWidth, Math.Max(estimatedHeight, 380));
        printDoc.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);
        printDoc.PrintPage += PrintDocument_PrintPage;

        return printDoc;
    }

    private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
    {
        var g = e.Graphics;
        if (g == null) return;

        g.SmoothingMode = SmoothingMode.HighQuality;

        using var fontTitle = new Font("Consolas", 11.5F, FontStyle.Bold);
        using var fontHeader = new Font("Consolas", 8F, FontStyle.Regular);
        using var fontBody = new Font("Consolas", 9F, FontStyle.Regular);
        using var fontBodyBold = new Font("Consolas", 9F, FontStyle.Bold);
        using var fontNotes = new Font("Consolas", 7.5F, FontStyle.Italic);
        using var fontTotal = new Font("Consolas", 10.5F, FontStyle.Bold);
        using var fontFooter = new Font("Consolas", 8F, FontStyle.Italic);
        using var brush = new SolidBrush(Color.Black);

        float y = 10;
        float width = PrintableWidth - 10; // Trừ lề 2 bên 5px

        // 1. Tên quán & Địa chỉ
        DrawCenter(g, StoreName, fontTitle, brush, y, width);
        y += 20;
        DrawCenter(g, StoreAddress, fontHeader, brush, y, width);
        y += 15;
        DrawCenter(g, StorePhone, fontHeader, brush, y, width);
        y += 20;

        // 2. Thông tin hóa đơn
        g.DrawString($"Số HĐ   : {ReceiptCode}", fontBody, brush, 5, y);
        y += 16;
        g.DrawString($"Thời gian: {OrderDate:dd/MM/yyyy HH:mm:ss}", fontBody, brush, 5, y);
        y += 16;
        g.DrawString($"Thu ngân : {CashierName}", fontBody, brush, 5, y);
        y += 16;
        g.DrawString($"Vị trí   : {TableNumber}", fontBody, brush, 5, y);
        y += 20;

        // 3. Dòng gạch ngang
        DrawLine(g, y, width);
        y += 8;

        // 4. Tiêu đề cột
        g.DrawString("Tên món", fontBodyBold, brush, 5, y);
        g.DrawString("SL", fontBodyBold, brush, 150, y);
        DrawRight(g, "Tiền", fontBodyBold, brush, y, width);
        y += 18;

        DrawLine(g, y, width);
        y += 8;

        // 5. Danh sách món
        foreach (var item in Items)
        {
            string shortName = FitString(g, item.ProductName, fontBody, 140);
            g.DrawString(shortName, fontBody, brush, 5, y);
            g.DrawString(item.Quantity.ToString(), fontBody, brush, 155, y);

            string lineTotalStr = item.LineTotal.ToString("#,##0", _cultureVi);
            DrawRight(g, lineTotalStr, fontBody, brush, y, width);
            y += 16;

            // In dòng ghi chú / Topping bên dưới nếu có
            if (!string.IsNullOrWhiteSpace(item.Notes))
            {
                string noteText = FitString(g, "  * " + item.Notes, fontNotes, width - 10);
                g.DrawString(noteText, fontNotes, brush, 5, y);
                y += 14;
            }
        }

        // 6. Dòng gạch ngang
        DrawLine(g, y, width);
        y += 8;

        // 7. Tạm tính, Thuế, Giảm giá, Tổng cộng thanh toán
        g.DrawString("Tiền hàng:", fontBody, brush, 5, y);
        DrawRight(g, Subtotal.ToString("#,##0 ₫", _cultureVi), fontBody, brush, y, width);
        y += 16;

        g.DrawString("Thuế VAT (5%):", fontBody, brush, 5, y);
        DrawRight(g, Tax.ToString("#,##0 ₫", _cultureVi), fontBody, brush, y, width);
        y += 16;

        if (Discount > 0)
        {
            g.DrawString("Giảm giá:", fontBody, brush, 5, y);
            DrawRight(g, "-" + Discount.ToString("#,##0 ₫", _cultureVi), fontBody, brush, y, width);
            y += 16;
        }

        DrawLine(g, y, width);
        y += 8;

        // Dòng tổng cộng in đậm
        g.DrawString("TỔNG CỘNG:", fontTotal, brush, 5, y);
        DrawRight(g, Total.ToString("#,##0 ₫", _cultureVi), fontTotal, brush, y, width);
        y += 22;

        // Thông tin thanh toán (Hình thức, Tiền đưa, Tiền thối)
        g.DrawString($"Hình thức: {PaymentMethod}", fontBody, brush, 5, y);
        y += 16;

        if (PaymentMethod == "Tiền mặt" && CustomerCash > 0)
        {
            g.DrawString("Tiền khách đưa:", fontBody, brush, 5, y);
            DrawRight(g, CustomerCash.ToString("#,##0 ₫", _cultureVi), fontBody, brush, y, width);
            y += 16;

            g.DrawString("Tiền thối lại:", fontBody, brush, 5, y);
            DrawRight(g, ChangeAmount.ToString("#,##0 ₫", _cultureVi), fontBody, brush, y, width);
            y += 16;
        }

        DrawLine(g, y, width);
        y += 10;

        // 8. Lời cảm ơn
        DrawCenter(g, "CẢM ƠN QUÝ KHÁCH!", fontBodyBold, brush, y, width);
        y += 16;
        DrawCenter(g, "Hẹn gặp lại quý khách lần sau!", fontFooter, brush, y, width);
        y += 15;
        DrawCenter(g, "Wifi: POS_Free | Pass: 88888888", fontHeader, brush, y, width);

        e.HasMorePages = false;
    }

    #endregion

    #region Helper Drawing Methods

    private static void DrawCenter(Graphics g, string text, Font font, Brush brush, float y, float width)
    {
        float textWidth = g.MeasureString(text, font).Width;
        float x = Math.Max(5, (width - textWidth) / 2 + 5);
        g.DrawString(text, font, brush, x, y);
    }

    private static void DrawRight(Graphics g, string text, Font font, Brush brush, float y, float width)
    {
        float textWidth = g.MeasureString(text, font).Width;
        float x = Math.Max(5, width - textWidth + 5);
        g.DrawString(text, font, brush, x, y);
    }

    private static void DrawLine(Graphics g, float y, float width)
    {
        using var pen = new Pen(Color.Gray, 1f) { DashStyle = DashStyle.Dash };
        g.DrawLine(pen, 5, y, width + 5, y);
    }

    private static string FitString(Graphics g, string text, Font font, float maxWidth)
    {
        if (g.MeasureString(text, font).Width <= maxWidth)
            return text;

        string current = text;
        while (current.Length > 0 && g.MeasureString(current + "..", font).Width > maxWidth)
        {
            current = current.Substring(0, current.Length - 1);
        }

        return current + "..";
    }

    #endregion
}
