using System.Diagnostics;
using System.Globalization;
using System.Text;
using MyWinFormsApp.DataAccess;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Màn hình thống kê báo cáo doanh thu theo bộ lọc thời gian và xuất file Excel / CSV
/// </summary>
public class frmRevenueReport : Form
{
    #region Controls & Fields

    private RadioButton rdoToday = null!;
    private RadioButton rdoWeek = null!;
    private RadioButton rdoMonth = null!;
    private RadioButton rdoCustom = null!;
    private DateTimePicker dtpFrom = null!;
    private DateTimePicker dtpTo = null!;
    private Button btnFilter = null!;
    private Button btnExportExcel = null!;

    private Label lblTotalRevenue = null!;
    private Label lblTotalCompletedOrders = null!;
    private Label lblTotalDiscount = null!;

    private DataGridView dgvOrders = null!;
    private readonly CultureInfo _cultureVi = new("vi-VN");

    #endregion

    #region Constructor

    public frmRevenueReport()
    {
        InitializeUI();
    }

    #endregion

    #region UI Initialization

    private void InitializeUI()
    {
        Text = "Báo cáo thống kê doanh thu";
        Size = new Size(1040, 680);
        MinimumSize = new Size(980, 600);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        // 1. Header Panel
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(25, 135, 84), // Xanh lá chuyên nghiệp
            Padding = new Padding(16, 0, 0, 0)
        };
        var lblTitle = new Label
        {
            Text = "THỐNG KÊ & BÁO CÁO DOANH THU BÁN HÀNG",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlHeader.Controls.Add(lblTitle);

        // 2. Filter Panel
        var pnlFilter = new Panel
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = Color.FromArgb(248, 249, 250),
            Padding = new Padding(16, 12, 16, 12)
        };

        rdoToday = new RadioButton { Text = "Hôm nay", Location = new Point(16, 20), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        rdoWeek = new RadioButton { Text = "7 ngày gần nhất", Location = new Point(110, 20), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        rdoMonth = new RadioButton { Text = "Tháng này", Location = new Point(235, 20), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        rdoCustom = new RadioButton { Text = "Tùy chọn:", Location = new Point(335, 20), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

        var lblFrom = new Label { Text = "Từ:", Location = new Point(420, 22), AutoSize = true };
        dtpFrom = new DateTimePicker { Location = new Point(448, 19), Width = 115, Format = DateTimePickerFormat.Short, Enabled = false };
        var lblTo = new Label { Text = "Đến:", Location = new Point(570, 22), AutoSize = true };
        dtpTo = new DateTimePicker { Location = new Point(605, 19), Width = 115, Format = DateTimePickerFormat.Short, Enabled = false };

        btnFilter = new Button
        {
            Text = "Xem báo cáo",
            Location = new Point(735, 17),
            Size = new Size(110, 32),
            BackColor = Color.FromArgb(13, 110, 253),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnFilter.FlatAppearance.BorderSize = 0;
        btnFilter.Click += async (s, e) => await LoadReportDataAsync();

        btnExportExcel = new Button
        {
            Text = "📥 Xuất Excel",
            Location = new Point(855, 17),
            Size = new Size(115, 32),
            BackColor = Color.FromArgb(25, 135, 84), // Màu xanh Excel
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnExportExcel.FlatAppearance.BorderSize = 0;
        btnExportExcel.Click += (s, e) => ExportToExcelCsv();

        rdoToday.CheckedChanged += RadioFilter_CheckedChanged;
        rdoWeek.CheckedChanged += RadioFilter_CheckedChanged;
        rdoMonth.CheckedChanged += RadioFilter_CheckedChanged;
        rdoCustom.CheckedChanged += RadioFilter_CheckedChanged;

        pnlFilter.Controls.AddRange(new Control[]
        {
            rdoToday, rdoWeek, rdoMonth, rdoCustom,
            lblFrom, dtpFrom, lblTo, dtpTo, btnFilter, btnExportExcel
        });

        // 3. Stat Cards Panel (3 Cards thống kê tổng quan)
        var pnlCards = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            BackColor = Color.White,
            Padding = new Padding(16, 8, 16, 8)
        };

        var cardRevenue = CreateStatCard("TỔNG DOANH THU", "0 ₫", Color.FromArgb(235, 247, 238), Color.FromArgb(25, 135, 84), out lblTotalRevenue);
        cardRevenue.Location = new Point(16, 8);
        cardRevenue.Size = new Size(300, 74);

        var cardOrders = CreateStatCard("ĐƠN HOÀN TẤT", "0 đơn", Color.FromArgb(238, 245, 255), Color.FromArgb(13, 110, 253), out lblTotalCompletedOrders);
        cardOrders.Location = new Point(330, 8);
        cardOrders.Size = new Size(300, 74);

        var cardDiscount = CreateStatCard("TỔNG GIẢM GIÁ", "0 ₫", Color.FromArgb(255, 248, 235), Color.FromArgb(217, 119, 6), out lblTotalDiscount);
        cardDiscount.Location = new Point(644, 8);
        cardDiscount.Size = new Size(300, 74);

        pnlCards.Controls.AddRange(new Control[] { cardRevenue, cardOrders, cardDiscount });

        // 4. DataGridView Panel
        var pnlGrid = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            BackColor = Color.White
        };

        dgvOrders = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            RowTemplate = { Height = 32 }
        };

        SetupGridColumns();
        pnlGrid.Controls.Add(dgvOrders);

        // Add to Form
        Controls.Add(pnlGrid);
        Controls.Add(pnlCards);
        Controls.Add(pnlFilter);
        Controls.Add(pnlHeader);

        Load += async (s, e) => await LoadReportDataAsync();
    }

    private Panel CreateStatCard(string title, string initialValue, Color bgColor, Color textColor, out Label valueLabel)
    {
        var pnl = new Panel
        {
            BackColor = bgColor,
            BorderStyle = BorderStyle.None,
            Padding = new Padding(12, 10, 12, 10)
        };

        var lblTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 8.25F, FontStyle.Bold),
            ForeColor = Color.FromArgb(108, 117, 125),
            Dock = DockStyle.Top,
            Height = 18
        };

        valueLabel = new Label
        {
            Text = initialValue,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = textColor,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        pnl.Controls.Add(valueLabel);
        pnl.Controls.Add(lblTitle);
        return pnl;
    }

    private void SetupGridColumns()
    {
        dgvOrders.AutoGenerateColumns = false;
        dgvOrders.Columns.Clear();

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(OrderReportModel.OrderID),
            HeaderText = "Mã HĐ",
            Width = 70,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(OrderReportModel.TableNumber),
            HeaderText = "Vị trí / Bàn",
            Width = 90,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(OrderReportModel.OrderDate),
            HeaderText = "Thời gian thanh toán",
            Width = 150,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Format = "dd/MM/yyyy HH:mm:ss" }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(OrderReportModel.CashierName),
            HeaderText = "Thu ngân",
            Width = 110,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(OrderReportModel.SubTotal),
            HeaderText = "Tiền hàng",
            Width = 110,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "#,##0 ₫", FormatProvider = _cultureVi }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(OrderReportModel.TaxAmount),
            HeaderText = "Thuế VAT",
            Width = 90,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "#,##0 ₫", FormatProvider = _cultureVi }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(OrderReportModel.DiscountAmount),
            HeaderText = "Giảm giá",
            Width = 95,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "#,##0 ₫", FormatProvider = _cultureVi }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(OrderReportModel.TotalAmount),
            HeaderText = "Tổng thanh toán",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Format = "#,##0 ₫", FormatProvider = _cultureVi }
        });
    }

    #endregion

    #region Filter Logic & Data Loading

    private void RadioFilter_CheckedChanged(object? sender, EventArgs e)
    {
        dtpFrom.Enabled = rdoCustom.Checked;
        dtpTo.Enabled = rdoCustom.Checked;
    }

    private (DateTime FromDate, DateTime ToDate) GetDateRange()
    {
        DateTime now = DateTime.Now;

        if (rdoToday.Checked)
        {
            var start = now.Date;
            var end = start.AddDays(1).AddTicks(-1);
            return (start, end);
        }
        else if (rdoWeek.Checked)
        {
            var start = now.Date.AddDays(-6);
            var end = now.Date.AddDays(1).AddTicks(-1);
            return (start, end);
        }
        else if (rdoMonth.Checked)
        {
            var start = new DateTime(now.Year, now.Month, 1);
            var end = start.AddMonths(1).AddTicks(-1);
            return (start, end);
        }
        else // rdoCustom
        {
            var start = dtpFrom.Value.Date;
            var end = dtpTo.Value.Date.AddDays(1).AddTicks(-1);
            return (start, end);
        }
    }

    private async Task LoadReportDataAsync()
    {
        try
        {
            var (fromDate, toDate) = GetDateRange();
            var orders = await OrderRepository.GetCompletedOrdersAsync(fromDate, toDate);

            // Gán dữ liệu lên lưới
            dgvOrders.DataSource = orders;

            // Tính toán tổng hợp cho 3 Card
            decimal totalRevenue = orders.Sum(x => x.TotalAmount);
            int totalCount = orders.Count;
            decimal totalDiscount = orders.Sum(x => x.DiscountAmount);

            lblTotalRevenue.Text = totalRevenue.ToString("#,##0 ₫", _cultureVi);
            lblTotalCompletedOrders.Text = $"{totalCount:N0} đơn";
            lblTotalDiscount.Text = totalDiscount.ToString("#,##0 ₫", _cultureVi);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải báo cáo: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    #endregion

    #region Export to Excel / CSV

    /// <summary>
    /// Xuất danh sách hóa đơn ra file CSV chuẩn UTF-8 (có BOM) để Microsoft Excel hiển thị tiếng Việt hoàn hảo
    /// </summary>
    private void ExportToExcelCsv()
    {
        if (dgvOrders.DataSource is not List<OrderReportModel> orders || orders.Count == 0)
        {
            MessageBox.Show("Hiện không có dữ liệu đơn hàng nào để xuất báo cáo!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var sfd = new SaveFileDialog
        {
            Filter = "File Excel CSV (*.csv)|*.csv|Tất cả tệp (*.*)|*.*",
            FileName = $"BaoCaoDoanhThu_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
            Title = "Lưu báo cáo doanh thu Excel / CSV"
        };

        if (sfd.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            var sb = new StringBuilder();

            // Header thông tin báo cáo
            sb.AppendLine("BÁO CÁO DOANH THU BÁN HÀNG");
            sb.AppendLine($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss},Người xuất: {UserSession.CurrentUser?.FullName ?? "Quản trị"}");

            var (fromD, toD) = GetDateRange();
            sb.AppendLine($"Thời gian: Từ {fromD:dd/MM/yyyy HH:mm} Đến {toD:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Tổng doanh thu: {orders.Sum(x => x.TotalAmount):N0} VNĐ,Tổng số đơn: {orders.Count},Tổng giảm giá: {orders.Sum(x => x.DiscountAmount):N0} VNĐ");
            sb.AppendLine();

            // Header bảng
            sb.AppendLine("Mã HĐ,Vị trí / Bàn,Thời gian thanh toán,Thu ngân,Tiền hàng (VNĐ),Thuế VAT (VNĐ),Giảm giá (VNĐ),Tổng thanh toán (VNĐ),Trạng thái");

            // Chi tiết từng dòng
            foreach (var o in orders)
            {
                sb.AppendLine($"\"{o.OrderID}\",\"{o.TableNumber}\",\"{o.OrderDate:dd/MM/yyyy HH:mm:ss}\",\"{o.CashierName}\",{o.SubTotal},{o.TaxAmount},{o.DiscountAmount},{o.TotalAmount},\"{o.Status}\"");
            }

            // Ghi file với UTF8 with BOM để Excel đọc không lỗi font tiếng Việt
            File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));

            var openConfirm = MessageBox.Show(
                $"Xuất báo cáo thành công ra file:\n{sfd.FileName}\n\nBạn có muốn mở file bằng Excel ngay không?",
                "Xuất file thành công",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (openConfirm == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi xuất file: {ex.Message}", "Lỗi xuất báo cáo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    #endregion
}
