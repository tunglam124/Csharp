using System.Globalization;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Hộp thoại thanh toán đa phương thức: Tiền mặt (tự động tính tiền thối) và Chuyển khoản (sinh mã VietQR động)
/// </summary>
public class frmPayment : Form
{
    private readonly decimal _totalAmount;
    private readonly string _tableNumber;
    private readonly int _orderId;
    private readonly string _cashierName;
    private readonly CultureInfo _cultureVi = new("vi-VN");

    // Output results
    public string PaymentMethod { get; private set; } = "Tiền mặt";
    public decimal CustomerCash { get; private set; } = 0m;
    public decimal ChangeAmount { get; private set; } = 0m;

    // Controls - Cash Tab
    private TextBox txtCustomerCash = null!;
    private Label lblChangeResult = null!;
    private Label lblChangeTitle = null!;
    private Button btnConfirmCash = null!;

    // Controls - VietQR Tab
    private PictureBox picQr = null!;
    private Label lblQrStatus = null!;
    private Label lblQrBank = null!;
    private Label lblQrAccount = null!;
    private Label lblQrName = null!;
    private Label lblQrAmount = null!;
    private Label lblQrContent = null!;
    private Button btnConfirmTransfer = null!;

    private TabControl tabPayment = null!;
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(10) };

    public frmPayment(decimal totalAmount, string tableNumber, int orderId, string cashierName)
    {
        _totalAmount = totalAmount;
        _tableNumber = tableNumber;
        _orderId = orderId;
        _cashierName = cashierName;

        InitializeUI();
    }

    private void InitializeUI()
    {
        Text = "Thanh toán đơn hàng";
        Size = new Size(680, 620);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        // Header Panel: Hiển thị Tổng tiền to rõ
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 85,
            BackColor = Color.FromArgb(13, 110, 253),
            Padding = new Padding(20, 12, 20, 12)
        };

        var lblOrderInfo = new Label
        {
            Text = $"BÀN: {_tableNumber.ToUpper()}  |  THU NGÂN: {_cashierName.ToUpper()}",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(220, 235, 255),
            Dock = DockStyle.Top,
            Height = 20
        };

        var lblTotalTitle = new Label
        {
            Text = "TỔNG TIỀN THANH TOÁN:",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 36),
            AutoSize = true
        };

        var lblTotalValue = new Label
        {
            Text = _totalAmount.ToString("#,##0 ₫", _cultureVi),
            Font = new Font("Segoe UI", 20F, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 235, 59), // Màu vàng rực rỡ nổi bật
            Location = new Point(210, 28),
            AutoSize = true
        };

        pnlHeader.Controls.AddRange(new Control[] { lblOrderInfo, lblTotalTitle, lblTotalValue });

        // TabControl: 2 Phương thức thanh toán
        tabPayment = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Padding = new Point(16, 8)
        };

        var tabCash = new TabPage("💵 Tiền mặt (Cash)");
        tabCash.BackColor = Color.White;
        InitializeCashTab(tabCash);

        var tabTransfer = new TabPage("📱 Chuyển khoản (VietQR / MoMo)");
        tabTransfer.BackColor = Color.White;
        InitializeTransferTab(tabTransfer);

        tabPayment.TabPages.Add(tabCash);
        tabPayment.TabPages.Add(tabTransfer);

        // Footer Cancel Button
        var pnlFooter = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            BackColor = Color.FromArgb(248, 249, 250),
            Padding = new Padding(16, 8, 16, 8)
        };
        var btnCancel = new Button
        {
            Text = "Đóng / Hủy bỏ",
            Dock = DockStyle.Left,
            Width = 120,
            BackColor = Color.FromArgb(240, 242, 245),
            ForeColor = Color.FromArgb(33, 37, 41),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.Cancel
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        pnlFooter.Controls.Add(btnCancel);

        Controls.Add(tabPayment);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);

        CancelButton = btnCancel;

        tabPayment.SelectedIndexChanged += async (s, e) =>
        {
            if (tabPayment.SelectedTab == tabTransfer)
            {
                await LoadVietQrAsync();
            }
        };

        // Mặc định nạp tiền mặt = tổng tiền
        CustomerCash = _totalAmount;
        txtCustomerCash.Text = CustomerCash.ToString("#,##0", _cultureVi);
        UpdateCashCalculation();
    }

    #region Tab Tiền Mặt

    private void InitializeCashTab(TabPage tab)
    {
        var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = Color.White };

        int y = 16;
        var lblInputTitle = new Label { Text = "TIỀN KHÁCH ĐƯA (VNĐ):", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(33, 37, 41) };
        y += 26;

        txtCustomerCash = new TextBox
        {
            Location = new Point(20, y),
            Width = 320,
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(13, 110, 253),
            TextAlign = HorizontalAlignment.Right
        };
        txtCustomerCash.TextChanged += TxtCustomerCash_TextChanged;
        txtCustomerCash.KeyPress += TxtCustomerCash_KeyPress;

        var btnExact = new Button
        {
            Text = "ĐỦ TIỀN",
            Location = new Point(350, y),
            Size = new Size(110, 36),
            BackColor = Color.FromArgb(231, 241, 255),
            ForeColor = Color.FromArgb(13, 110, 253),
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnExact.FlatAppearance.BorderSize = 0;
        btnExact.Click += (s, e) =>
        {
            CustomerCash = _totalAmount;
            txtCustomerCash.Text = CustomerCash.ToString("#,##0", _cultureVi);
        };

        y += 48;

        // Các nút gợi ý mệnh giá tiền nhanh
        var lblSuggest = new Label { Text = "Gợi ý tiền chẵn:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(108, 117, 125) };
        y += 22;

        var pnlQuickButtons = new FlowLayoutPanel
        {
            Location = new Point(20, y),
            Size = new Size(600, 75),
            BackColor = Color.Transparent
        };

        decimal[] denominations = { 20000m, 50000m, 100000m, 200000m, 500000m };
        foreach (var den in denominations)
        {
            var btn = new Button
            {
                Text = den.ToString("#,##0k", _cultureVi).Replace(",000k", "k"),
                Size = new Size(88, 32),
                Margin = new Padding(0, 0, 8, 6),
                BackColor = Color.FromArgb(240, 242, 245),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) =>
            {
                CustomerCash = den;
                txtCustomerCash.Text = CustomerCash.ToString("#,##0", _cultureVi);
            };
            pnlQuickButtons.Controls.Add(btn);
        }

        // Nút cộng dồn nhanh
        decimal[] addOptions = { 10000m, 20000m, 50000m, 100000m };
        foreach (var opt in addOptions)
        {
            var btn = new Button
            {
                Text = $"+{opt / 1000:N0}k",
                Size = new Size(88, 32),
                Margin = new Padding(0, 0, 8, 6),
                BackColor = Color.FromArgb(255, 243, 205),
                ForeColor = Color.FromArgb(102, 77, 3),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) =>
            {
                CustomerCash += opt;
                txtCustomerCash.Text = CustomerCash.ToString("#,##0", _cultureVi);
            };
            pnlQuickButtons.Controls.Add(btn);
        }

        y += 85;

        // Stat Card hiển thị tiền thừa
        var pnlChangeCard = new Panel
        {
            Location = new Point(20, y),
            Size = new Size(590, 80),
            BackColor = Color.FromArgb(235, 247, 238),
            BorderStyle = BorderStyle.None,
            Padding = new Padding(16, 12, 16, 12)
        };
        lblChangeTitle = new Label
        {
            Text = "TIỀN THỪA TRẢ KHÁCH (TIỀN THỐI):",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 135, 84),
            Dock = DockStyle.Top,
            Height = 20
        };
        lblChangeResult = new Label
        {
            Text = "0 ₫",
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 135, 84),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlChangeCard.Controls.Add(lblChangeResult);
        pnlChangeCard.Controls.Add(lblChangeTitle);

        y += 95;

        // Nút xác nhận thanh toán tiền mặt
        btnConfirmCash = new Button
        {
            Text = "XÁC NHẬN THANH TOÁN TIỀN MẶT (F9)",
            Location = new Point(20, y),
            Size = new Size(590, 48),
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnConfirmCash.FlatAppearance.BorderSize = 0;
        btnConfirmCash.Click += BtnConfirmCash_Click;

        pnl.Controls.AddRange(new Control[]
        {
            lblInputTitle, txtCustomerCash, btnExact,
            lblSuggest, pnlQuickButtons,
            pnlChangeCard, btnConfirmCash
        });

        tab.Controls.Add(pnl);
    }

    private void TxtCustomerCash_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
        {
            e.Handled = true;
        }
    }

    private void TxtCustomerCash_TextChanged(object? sender, EventArgs e)
    {
        string raw = new string(txtCustomerCash.Text.Where(char.IsDigit).ToArray());
        if (decimal.TryParse(raw, out decimal val))
        {
            CustomerCash = val;
        }
        else
        {
            CustomerCash = 0m;
        }

        UpdateCashCalculation();
    }

    private void UpdateCashCalculation()
    {
        ChangeAmount = CustomerCash - _totalAmount;

        if (ChangeAmount >= 0)
        {
            lblChangeTitle.Text = "TIỀN THỪA TRẢ KHÁCH (TIỀN THỐI):";
            lblChangeTitle.ForeColor = Color.FromArgb(25, 135, 84);
            lblChangeResult.Text = ChangeAmount.ToString("#,##0 ₫", _cultureVi);
            lblChangeResult.ForeColor = Color.FromArgb(25, 135, 84);
            btnConfirmCash.Enabled = true;
            btnConfirmCash.BackColor = Color.FromArgb(40, 167, 69);
        }
        else
        {
            decimal missing = Math.Abs(ChangeAmount);
            lblChangeTitle.Text = "KHÁCH ĐƯA THIẾU:";
            lblChangeTitle.ForeColor = Color.FromArgb(220, 53, 69);
            lblChangeResult.Text = $"-{missing.ToString("#,##0 ₫", _cultureVi)}";
            lblChangeResult.ForeColor = Color.FromArgb(220, 53, 69);
            btnConfirmCash.Enabled = false;
            btnConfirmCash.BackColor = Color.FromArgb(108, 117, 125);
        }
    }

    private void BtnConfirmCash_Click(object? sender, EventArgs e)
    {
        if (CustomerCash < _totalAmount)
        {
            MessageBox.Show("Tiền khách đưa chưa đủ để thanh toán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        PaymentMethod = "Tiền mặt";
        DialogResult = DialogResult.OK;
        Close();
    }

    #endregion

    #region Tab Chuyển Khoản VietQR

    private void InitializeTransferTab(TabPage tab)
    {
        var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = Color.White };

        // PictureBox QR bên trái
        var pnlQrContainer = new Panel
        {
            Location = new Point(20, 15),
            Size = new Size(240, 280),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White
        };

        picQr = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.White
        };

        lblQrStatus = new Label
        {
            Text = "Đang tạo mã VietQR...",
            Dock = DockStyle.Bottom,
            Height = 25,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
            ForeColor = Color.FromArgb(108, 117, 125)
        };

        pnlQrContainer.Controls.Add(picQr);
        pnlQrContainer.Controls.Add(lblQrStatus);

        // Thông tin tài khoản bên phải
        var pnlInfo = new Panel
        {
            Location = new Point(275, 15),
            Size = new Size(350, 280),
            BackColor = Color.Transparent
        };

        int iy = 0;
        var lblBankTitle = new Label { Text = "Ngân hàng:", Location = new Point(0, iy), AutoSize = true, ForeColor = Color.FromArgb(108, 117, 125), Font = new Font("Segoe UI", 8.5F) };
        iy += 18;
        lblQrBank = new Label { Text = "MBBank", Location = new Point(0, iy), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
        iy += 26;

        var lblAccTitle = new Label { Text = "Số tài khoản:", Location = new Point(0, iy), AutoSize = true, ForeColor = Color.FromArgb(108, 117, 125), Font = new Font("Segoe UI", 8.5F) };
        var btnCopyAcc = new Button
        {
            Text = "Sao chép",
            Location = new Point(250, iy - 2),
            Size = new Size(70, 24),
            BackColor = Color.FromArgb(240, 242, 245),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 8F),
            Cursor = Cursors.Hand
        };
        btnCopyAcc.FlatAppearance.BorderSize = 0;
        btnCopyAcc.Click += (s, e) =>
        {
            Clipboard.SetText(lblQrAccount.Text);
            MessageBox.Show("Đã sao chép số tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        iy += 18;
        lblQrAccount = new Label { Text = "0987654321", Location = new Point(0, iy), AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(13, 110, 253) };
        iy += 26;

        var lblNameTitle = new Label { Text = "Chủ tài khoản:", Location = new Point(0, iy), AutoSize = true, ForeColor = Color.FromArgb(108, 117, 125), Font = new Font("Segoe UI", 8.5F) };
        iy += 18;
        lblQrName = new Label { Text = "NGUYEN VAN A", Location = new Point(0, iy), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
        iy += 26;

        var lblAmountTitle = new Label { Text = "Số tiền thanh toán:", Location = new Point(0, iy), AutoSize = true, ForeColor = Color.FromArgb(108, 117, 125), Font = new Font("Segoe UI", 8.5F) };
        iy += 18;
        lblQrAmount = new Label { Text = _totalAmount.ToString("#,##0 ₫", _cultureVi), Location = new Point(0, iy), AutoSize = true, Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.FromArgb(220, 53, 69) };
        iy += 26;

        var lblContentTitle = new Label { Text = "Nội dung chuyển khoản (bắt buộc):", Location = new Point(0, iy), AutoSize = true, ForeColor = Color.FromArgb(108, 117, 125), Font = new Font("Segoe UI", 8.5F) };
        iy += 18;
        lblQrContent = new Label { Text = $"HD{_orderId} {_tableNumber}", Location = new Point(0, iy), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(253, 126, 20) };
        iy += 28;

        var lnkChangeConfig = new LinkLabel
        {
            Text = "⚙️ Thay đổi tài khoản nhận tiền",
            Location = new Point(0, iy),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
            Cursor = Cursors.Hand
        };
        lnkChangeConfig.LinkClicked += async (s, e) =>
        {
            using var dlg = new frmBankConfig();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                await LoadVietQrAsync();
            }
        };

        pnlInfo.Controls.AddRange(new Control[]
        {
            lblBankTitle, lblQrBank,
            lblAccTitle, btnCopyAcc, lblQrAccount,
            lblNameTitle, lblQrName,
            lblAmountTitle, lblQrAmount,
            lblContentTitle, lblQrContent,
            lnkChangeConfig
        });

        // Nút xác nhận chuyển khoản
        btnConfirmTransfer = new Button
        {
            Text = "XÁC NHẬN ĐÃ NHẬN CHUYỂN KHOẢN THÀNH CÔNG (F9)",
            Location = new Point(20, 310),
            Size = new Size(605, 48),
            BackColor = Color.FromArgb(13, 110, 253),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnConfirmTransfer.FlatAppearance.BorderSize = 0;
        btnConfirmTransfer.Click += (s, e) =>
        {
            PaymentMethod = "Chuyển khoản VietQR";
            CustomerCash = _totalAmount;
            ChangeAmount = 0m;
            DialogResult = DialogResult.OK;
            Close();
        };

        pnl.Controls.AddRange(new Control[]
        {
            pnlQrContainer,
            pnlInfo,
            btnConfirmTransfer
        });

        tab.Controls.Add(pnl);
    }

    private async Task LoadVietQrAsync()
    {
        try
        {
            var config = BankConfig.Current;
            lblQrBank.Text = config.BankName;
            lblQrAccount.Text = config.AccountNumber;
            lblQrName.Text = config.AccountName;
            lblQrAmount.Text = _totalAmount.ToString("#,##0 ₫", _cultureVi);

            string orderCode = _orderId > 0 ? _orderId.ToString() : DateTime.Now.ToString("HHmmss");
            string transferContent = $"HD{orderCode} {_tableNumber.Replace(" ", "")}";
            lblQrContent.Text = transferContent;

            lblQrStatus.Text = "Đang tải mã QR từ VietQR...";
            lblQrStatus.ForeColor = Color.FromArgb(108, 117, 125);

            string url = config.GenerateQrUrl(_totalAmount, transferContent);

            var responseBytes = await _httpClient.GetByteArrayAsync(url);
            using var ms = new MemoryStream(responseBytes);
            var image = Image.FromStream(ms);

            picQr.Image?.Dispose();
            picQr.Image = new Bitmap(image);

            lblQrStatus.Text = "Quét mã để thanh toán tự động";
            lblQrStatus.ForeColor = Color.FromArgb(25, 135, 84);
        }
        catch
        {
            lblQrStatus.Text = "Không thể tải mã QR (Kiểm tra mạng)";
            lblQrStatus.ForeColor = Color.FromArgb(220, 53, 69);
            CreateOfflineQrFallback();
        }
    }

    private void CreateOfflineQrFallback()
    {
        var bmp = new Bitmap(240, 240);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.FromArgb(245, 247, 250));
        using var brush = new SolidBrush(Color.FromArgb(108, 117, 125));
        using var font = new Font("Segoe UI", 9F, FontStyle.Regular);
        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString("Quét QR VietQR\nVui lòng kiểm tra kết nối mạng", font, brush, new RectangleF(0, 0, bmp.Width, bmp.Height), sf);
        picQr.Image?.Dispose();
        picQr.Image = bmp;
    }

    #endregion
}
