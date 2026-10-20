using System.Data;
using Microsoft.Data.SqlClient;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.DataAccess;

/// <summary>
/// Quản lý việc lưu, cập nhật và truy vấn dữ liệu đơn hàng (Orders, OrderDetails)
/// Sử dụng SqlTransaction để đảm bảo tính toàn vẹn dữ liệu
/// </summary>
public static class OrderRepository
{
    /// <summary>
    /// Lưu đơn hàng mới vào CSDL sử dụng SqlTransaction
    /// </summary>
    public static async Task<int> SaveOrderAsync(
        string status,
        IEnumerable<CartItem> cartItems,
        string? tableNumber,
        string? cashierName,
        decimal subtotal,
        decimal taxAmount,
        decimal discountAmount,
        decimal totalAmount,
        string? paymentMethod = null)
    {
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();

        await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync();

        try
        {
            const string insertOrderSql = @"
                INSERT INTO Orders 
                    (TableNumber, OrderDate, SubTotal, TaxAmount, DiscountAmount, TotalAmount, Status, CashierName, PaymentMethod)
                VALUES 
                    (@TableNumber, GETDATE(), @SubTotal, @TaxAmount, @DiscountAmount, @TotalAmount, @Status, @CashierName, @PaymentMethod);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int orderId;
            await using (var cmdOrder = new SqlCommand(insertOrderSql, conn, transaction))
            {
                cmdOrder.Parameters.Add(new SqlParameter("@TableNumber", SqlDbType.NVarChar, 50) { Value = (object?)tableNumber ?? DBNull.Value });
                cmdOrder.Parameters.Add(new SqlParameter("@SubTotal", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = subtotal });
                cmdOrder.Parameters.Add(new SqlParameter("@TaxAmount", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = taxAmount });
                cmdOrder.Parameters.Add(new SqlParameter("@DiscountAmount", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = discountAmount });
                cmdOrder.Parameters.Add(new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = totalAmount });
                cmdOrder.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = status });
                cmdOrder.Parameters.Add(new SqlParameter("@CashierName", SqlDbType.NVarChar, 100) { Value = (object?)cashierName ?? DBNull.Value });
                cmdOrder.Parameters.Add(new SqlParameter("@PaymentMethod", SqlDbType.NVarChar, 50) { Value = (object?)paymentMethod ?? DBNull.Value });

                object? result = await cmdOrder.ExecuteScalarAsync();
                orderId = result != null ? Convert.ToInt32(result) : -1;
            }

            if (orderId <= 0)
            {
                throw new Exception("Không thể khởi tạo mã đơn hàng mới.");
            }

            const string insertDetailSql = @"
                INSERT INTO OrderDetails 
                    (OrderID, ProductID, Quantity, UnitPrice, LineTotal, Notes)
                VALUES 
                    (@OrderID, @ProductID, @Quantity, @UnitPrice, @LineTotal, @Notes);";

            foreach (var item in cartItems)
            {
                await using var cmdDetail = new SqlCommand(insertDetailSql, conn, transaction);
                cmdDetail.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int) { Value = orderId });
                cmdDetail.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int) { Value = item.ProductId });
                cmdDetail.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int) { Value = item.Quantity });
                cmdDetail.Parameters.Add(new SqlParameter("@UnitPrice", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = item.UnitPrice });
                cmdDetail.Parameters.Add(new SqlParameter("@LineTotal", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = item.LineTotal });
                cmdDetail.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, 255) { Value = string.IsNullOrEmpty(item.Notes) ? (object)DBNull.Value : item.Notes });

                await cmdDetail.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return orderId;
        }
        catch (Exception ex)
        {
            try { await transaction.RollbackAsync(); } catch { }
            MessageBox.Show($"Lỗi khi lưu đơn hàng: {ex.Message}", "Lỗi giao dịch", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return -1;
        }
    }

    /// <summary>
    /// Cập nhật đơn hàng đã có (ví dụ khi mở lại đơn tạm giữ để gọi thêm món hoặc thanh toán hoàn tất)
    /// </summary>
    public static async Task<bool> UpdateOrderAsync(
        int orderId,
        string status,
        IEnumerable<CartItem> cartItems,
        string? tableNumber,
        string? cashierName,
        decimal subtotal,
        decimal taxAmount,
        decimal discountAmount,
        decimal totalAmount,
        string? paymentMethod = null)
    {
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();

        await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync();

        try
        {
            const string updateOrderSql = @"
                UPDATE Orders 
                SET TableNumber = @TableNumber,
                    SubTotal = @SubTotal,
                    TaxAmount = @TaxAmount,
                    DiscountAmount = @DiscountAmount,
                    TotalAmount = @TotalAmount,
                    Status = @Status,
                    CashierName = @CashierName,
                    PaymentMethod = @PaymentMethod
                WHERE OrderID = @OrderID;";

            await using (var cmd = new SqlCommand(updateOrderSql, conn, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int) { Value = orderId });
                cmd.Parameters.Add(new SqlParameter("@TableNumber", SqlDbType.NVarChar, 50) { Value = (object?)tableNumber ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@SubTotal", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = subtotal });
                cmd.Parameters.Add(new SqlParameter("@TaxAmount", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = taxAmount });
                cmd.Parameters.Add(new SqlParameter("@DiscountAmount", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = discountAmount });
                cmd.Parameters.Add(new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = totalAmount });
                cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = status });
                cmd.Parameters.Add(new SqlParameter("@CashierName", SqlDbType.NVarChar, 100) { Value = (object?)cashierName ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@PaymentMethod", SqlDbType.NVarChar, 50) { Value = (object?)paymentMethod ?? DBNull.Value });

                await cmd.ExecuteNonQueryAsync();
            }

            // Xóa chi tiết món cũ
            const string deleteDetailsSql = "DELETE FROM OrderDetails WHERE OrderID = @OrderID;";
            await using (var cmdDel = new SqlCommand(deleteDetailsSql, conn, transaction))
            {
                cmdDel.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int) { Value = orderId });
                await cmdDel.ExecuteNonQueryAsync();
            }

            // Ghi lại chi tiết món mới
            const string insertDetailSql = @"
                INSERT INTO OrderDetails 
                    (OrderID, ProductID, Quantity, UnitPrice, LineTotal, Notes)
                VALUES 
                    (@OrderID, @ProductID, @Quantity, @UnitPrice, @LineTotal, @Notes);";

            foreach (var item in cartItems)
            {
                await using var cmdDetail = new SqlCommand(insertDetailSql, conn, transaction);
                cmdDetail.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int) { Value = orderId });
                cmdDetail.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int) { Value = item.ProductId });
                cmdDetail.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int) { Value = item.Quantity });
                cmdDetail.Parameters.Add(new SqlParameter("@UnitPrice", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = item.UnitPrice });
                cmdDetail.Parameters.Add(new SqlParameter("@LineTotal", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = item.LineTotal });
                cmdDetail.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, 255) { Value = string.IsNullOrEmpty(item.Notes) ? (object)DBNull.Value : item.Notes });

                await cmdDetail.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            try { await transaction.RollbackAsync(); } catch { }
            MessageBox.Show($"Lỗi cập nhật đơn hàng: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>
    /// Lấy số lượng đơn hàng đang tạm giữ ('Pending')
    /// </summary>
    public static async Task<int> GetPendingOrderCountAsync()
    {
        const string query = "SELECT COUNT(1) FROM Orders WHERE Status = 'Pending';";
        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            await conn.OpenAsync();
            object? result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Lấy danh sách toàn bộ các đơn hàng đang ở trạng thái 'Pending' kèm số lượng món
    /// </summary>
    public static async Task<List<PendingOrderModel>> GetPendingOrdersAsync()
    {
        var list = new List<PendingOrderModel>();
        const string query = @"
            SELECT o.OrderID, o.TableNumber, o.OrderDate, o.SubTotal, o.TaxAmount, o.DiscountAmount, o.TotalAmount, o.CashierName,
                   ISNULL(COUNT(d.DetailID), 0) AS ItemCount
            FROM Orders o
            LEFT JOIN OrderDetails d ON o.OrderID = d.OrderID
            WHERE o.Status = 'Pending'
            GROUP BY o.OrderID, o.TableNumber, o.OrderDate, o.SubTotal, o.TaxAmount, o.DiscountAmount, o.TotalAmount, o.CashierName
            ORDER BY o.OrderID DESC;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new PendingOrderModel
                {
                    OrderID = reader.GetInt32(0),
                    TableNumber = reader.IsDBNull(1) ? "N/A" : reader.GetString(1),
                    OrderDate = reader.GetDateTime(2),
                    SubTotal = reader.GetDecimal(3),
                    TaxAmount = reader.GetDecimal(4),
                    DiscountAmount = reader.GetDecimal(5),
                    TotalAmount = reader.GetDecimal(6),
                    CashierName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    ItemCount = reader.GetInt32(8)
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi truy vấn đơn tạm giữ: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return list;
    }

    /// <summary>
    /// Lấy thông tin đơn hàng Pending theo số bàn
    /// </summary>
    public static async Task<PendingOrderModel?> GetPendingOrderByTableAsync(string tableNumber)
    {
        const string query = @"
            SELECT TOP 1 o.OrderID, o.TableNumber, o.OrderDate, o.SubTotal, o.TaxAmount, o.DiscountAmount, o.TotalAmount, o.CashierName,
                   ISNULL(COUNT(d.DetailID), 0) AS ItemCount
            FROM Orders o
            LEFT JOIN OrderDetails d ON o.OrderID = d.OrderID
            WHERE o.Status = 'Pending' AND o.TableNumber = @TableNumber
            GROUP BY o.OrderID, o.TableNumber, o.OrderDate, o.SubTotal, o.TaxAmount, o.DiscountAmount, o.TotalAmount, o.CashierName
            ORDER BY o.OrderID DESC;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@TableNumber", SqlDbType.NVarChar, 50) { Value = tableNumber });
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PendingOrderModel
                {
                    OrderID = reader.GetInt32(0),
                    TableNumber = reader.IsDBNull(1) ? "N/A" : reader.GetString(1),
                    OrderDate = reader.GetDateTime(2),
                    SubTotal = reader.GetDecimal(3),
                    TaxAmount = reader.GetDecimal(4),
                    DiscountAmount = reader.GetDecimal(5),
                    TotalAmount = reader.GetDecimal(6),
                    CashierName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    ItemCount = reader.GetInt32(8)
                };
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tìm đơn bàn {tableNumber}: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return null;
    }

    /// <summary>
    /// Lấy danh sách món chi tiết của một OrderID từ CSDL để nạp vào giỏ hàng
    /// </summary>
    public static async Task<List<CartItem>> GetOrderDetailsByOrderIdAsync(int orderId)
    {
        var list = new List<CartItem>();
        const string query = @"
            SELECT d.ProductID, p.ProductName, d.Quantity, d.UnitPrice, d.Notes, p.Price AS BasePrice
            FROM OrderDetails d
            INNER JOIN Products p ON d.ProductID = p.ProductID
            WHERE d.OrderID = @OrderID
            ORDER BY d.DetailID;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int) { Value = orderId });

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new CartItem
                {
                    ProductId = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    Quantity = reader.GetInt32(2),
                    UnitPrice = reader.GetDecimal(3),
                    Notes = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    BasePrice = reader.GetDecimal(5)
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi nạp chi tiết đơn hàng #{orderId}: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return list;
    }

    /// <summary>
    /// Hủy đơn hàng (Status = 'Canceled')
    /// </summary>
    public static async Task<bool> CancelOrderAsync(int orderId)
    {
        const string query = "UPDATE Orders SET Status = 'Canceled' WHERE OrderID = @OrderID;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int) { Value = orderId });

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi hủy đơn #{orderId}: {ex.Message}", "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>
    /// Chuyển bàn: Cập nhật số bàn của đơn hàng Pending sang bàn mới
    /// </summary>
    public static async Task<bool> MoveTableAsync(int orderId, string newTableNumber)
    {
        const string query = "UPDATE Orders SET TableNumber = @NewTable WHERE OrderID = @OrderID AND Status = 'Pending';";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int) { Value = orderId });
            cmd.Parameters.Add(new SqlParameter("@NewTable", SqlDbType.NVarChar, 50) { Value = newTableNumber });

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi chuyển bàn: {ex.Message}", "Lỗi chuyển bàn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>
    /// Gộp bàn: Gộp toàn bộ món từ sourceOrderId sang targetOrderId, tính lại tổng tiền và hủy sourceOrderId
    /// </summary>
    public static async Task<bool> MergeOrdersAsync(int sourceOrderId, int targetOrderId)
    {
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync();

        try
        {
            // Chuyển toàn bộ OrderDetails từ sourceOrderId sang targetOrderId
            const string updateDetailsSql = "UPDATE OrderDetails SET OrderID = @TargetOrderID WHERE OrderID = @SourceOrderID;";
            await using (var cmdDetails = new SqlCommand(updateDetailsSql, conn, transaction))
            {
                cmdDetails.Parameters.Add(new SqlParameter("@TargetOrderID", SqlDbType.Int) { Value = targetOrderId });
                cmdDetails.Parameters.Add(new SqlParameter("@SourceOrderID", SqlDbType.Int) { Value = sourceOrderId });
                await cmdDetails.ExecuteNonQueryAsync();
            }

            // Tính lại tổng tiền cho targetOrderId
            const string recalculateSql = @"
                DECLARE @SubTotal DECIMAL(18,2) = (SELECT ISNULL(SUM(LineTotal), 0) FROM OrderDetails WHERE OrderID = @TargetOrderID);
                DECLARE @Tax DECIMAL(18,2) = ROUND(@SubTotal * 0.05, 0);
                DECLARE @Discount DECIMAL(18,2) = (SELECT DiscountAmount FROM Orders WHERE OrderID = @TargetOrderID);
                DECLARE @Total DECIMAL(18,2) = @SubTotal + @Tax - @Discount;

                UPDATE Orders 
                SET SubTotal = @SubTotal,
                    TaxAmount = @Tax,
                    TotalAmount = CASE WHEN @Total > 0 THEN @Total ELSE 0 END
                WHERE OrderID = @TargetOrderID;

                UPDATE Orders SET Status = 'Canceled' WHERE OrderID = @SourceOrderID;";

            await using (var cmdRecalc = new SqlCommand(recalculateSql, conn, transaction))
            {
                cmdRecalc.Parameters.Add(new SqlParameter("@TargetOrderID", SqlDbType.Int) { Value = targetOrderId });
                cmdRecalc.Parameters.Add(new SqlParameter("@SourceOrderID", SqlDbType.Int) { Value = sourceOrderId });
                await cmdRecalc.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            try { await transaction.RollbackAsync(); } catch { }
            MessageBox.Show($"Lỗi gộp bàn: {ex.Message}", "Lỗi gộp bàn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>
    /// Lấy danh sách các đơn hàng hoàn tất ('Completed') trong khoảng thời gian để làm báo cáo doanh thu
    /// </summary>
    public static async Task<List<OrderReportModel>> GetCompletedOrdersAsync(DateTime fromDate, DateTime toDate)
    {
        var list = new List<OrderReportModel>();
        const string query = @"
            SELECT OrderID, TableNumber, OrderDate, SubTotal, TaxAmount, DiscountAmount, TotalAmount, CashierName, Status
            FROM Orders
            WHERE Status = 'Completed' 
              AND OrderDate >= @FromDate 
              AND OrderDate <= @ToDate
            ORDER BY OrderDate DESC;";

        try
        {
            await using var conn = DbHelper.CreateConnection();
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = fromDate });
            cmd.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime) { Value = toDate });

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new OrderReportModel
                {
                    OrderID = reader.GetInt32(0),
                    TableNumber = reader.IsDBNull(1) ? "N/A" : reader.GetString(1),
                    OrderDate = reader.GetDateTime(2),
                    SubTotal = reader.GetDecimal(3),
                    TaxAmount = reader.GetDecimal(4),
                    DiscountAmount = reader.GetDecimal(5),
                    TotalAmount = reader.GetDecimal(6),
                    CashierName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    Status = reader.GetString(8)
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi truy vấn báo cáo doanh thu: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return list;
    }
}
