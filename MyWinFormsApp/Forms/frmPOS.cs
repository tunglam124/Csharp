using System.ComponentModel;
using System.Globalization;
using MyWinFormsApp.DataAccess;
using MyWinFormsApp.Models;
using MyWinFormsApp.Printing;
using MyWinFormsApp.UserControls;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Màn hình bán hàng chính (POS) với đầy đủ 7 tính năng nâng cao:
/// 1. Mở lại đơn tạm giữ (Pending Orders)
/// 2. Sơ đồ bàn trực quan (Table Map, Chuyển bàn, Gộp bàn)
/// 3. Tính tiền thừa trả khách (Tiền thối lại)
/// 4. Tạo mã VietQR động
/// 5. Topping, Cỡ ly & Ghi chú món (Modifiers)
/// 6. Đăng nhập & Phân quyền (Login & Roles)
/// 7. Xuất báo cáo Excel / CSV
/// </summary>
public partial class frmPOS : Form
{
    #region Fields & Constants

    // Giỏ hàng & Định dạng
    private readonly BindingList<CartItem> _cartItems = new();
    private readonly CultureInfo _cultureVi = new("vi-VN");

    // Thuế & Tiền tệ
    public const decimal TaxRate = 0.05m; // Thuế VAT 5%
    private decimal _subtotal = 0m;
    private decimal _taxAmount = 0m;
    private decimal _discountAmount = 0m;
    private decimal _totalAmount = 0m;

    // Đơn hàng đang được mở lại (nếu > 0 nghĩa là đang sửa đơn Pending cũ)
    private int _currentLoadedOrderId = 0;

    // Lưu trữ thông tin đơn hàng vừa thanh toán để in hóa đơn
    private int _lastCreatedOrderId = 0;
    private List<CartItem> _lastOrderItems = new();
    private decimal _lastSubtotal;
    private decimal _lastTax;
    private decimal _lastDiscount;
    private decimal _lastTotal;
    private string _lastTableNumber = "Bàn 01";
    private DateTime _lastOrderDate;
    private string _lastPaymentMethod = "Tiền mặt";
    private decimal _lastCustomerCash = 0m;
    private decimal _lastChangeAmount = 0m;

    // Quản lý nút danh mục
    private Button? _currentCategoryButton;
    private readonly Color _catBtnDefaultBg = Color.FromArgb(240, 242, 245);
    private readonly Color _catBtnDefaultFg = Color.FromArgb(33, 37, 41);
    private readonly Color _catBtnSelectedBg = Color.FromArgb(13, 110, 253);
    private readonly Color _catBtnSelectedFg = Color.White;

    // Danh sách 16 bàn + Mang về
    private readonly List<string> _allTableNames = Enumerable.Range(1, 16)
        .Select(i => $"Bàn {i:D2}")
        .Concat(new[] { "Mang về" })
        .ToList();

    // Các Control giao diện bổ sung
    private Panel pnlMenuView = null!;
    private Panel pnlTableMapView = null!;
    private FlowLayoutPanel flpTables = null!;
    private Label lblTableStatusSummary = null!;

    private Button btnViewMenu = null!;
    private Button btnViewTableMap = null!;
    private Button btnPendingOrders = null!;
    private Label lblCurrentUser = null!;
    private Button btnLogout = null!;
    private Button btnSwitchTable = null!;

    /// <summary>
    /// Ghi nhớ danh mục món đang chọn (0 = ⭐ Tất cả) để đồng bộ giữa các lần đăng nhập hoặc khi thêm món
    /// </summary>
    public static int LastActiveCategoryId { get; set; } = 0;

    private int _lastPendingCount = 0;

    #endregion

    #region Public Properties

    public BindingList<CartItem> CartItems => _cartItems;
    public decimal Subtotal => _subtotal;
    public decimal TaxAmount => _taxAmount;
    public decimal DiscountAmount => _discountAmount;
    public decimal TotalAmount => _totalAmount;
    public int LastCreatedOrderId => _lastCreatedOrderId;

    /// <summary>
    /// Đánh dấu nếu người dùng bấm nút Đăng xuất để Program.cs mở lại form Login
    /// </summary>
    public bool RequestedLogout { get; private set; } = false;

    #endregion

    #region Constructor

    public frmPOS()
    {
        InitializeComponent();
        InitializeCustomViews();
        SetupCartGrid();
    }

    #endregion

    #region Custom Views & UI Setup

    /// <summary>
    /// Khởi tạo và bố trí 2 view chính: Thực đơn món (pnlMenuView) và Sơ đồ bàn (pnlTableMapView),
    /// cùng với các nút điều hướng trên thanh TopBar.
    /// </summary>
    private void InitializeCustomViews()
    {
        // 1. Cấu hình pnlTopBar trải rộng toàn bộ form phía trên (Dock Top)
        pnlTopBar.Controls.Clear();
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Height = 54;
        pnlTopBar.Padding = new Padding(10, 8, 10, 8);
        pnlTopBar.BringToFront();

        // FlowLayoutPanel nhóm điều hướng bên trái
        var flpTopLeft = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        // Logo / Thương hiệu
        lblBrand.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblBrand.ForeColor = Color.FromArgb(13, 110, 253);
        lblBrand.AutoSize = true;
        lblBrand.Margin = new Padding(0, 7, 14, 0);

        // Nút chuyển sang View Thực đơn
        btnViewMenu = new Button
        {
            Text = "🛒 Thực đơn",
            Size = new Size(110, 36),
            BackColor = Color.FromArgb(13, 110, 253),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 8, 0)
        };
        btnViewMenu.FlatAppearance.BorderSize = 0;
        btnViewMenu.Click += (s, e) => SwitchToMenuView();

        // Nút chuyển sang View Sơ đồ bàn
        btnViewTableMap = new Button
        {
            Text = "🪑 Sơ đồ bàn",
            Size = new Size(115, 36),
            BackColor = Color.FromArgb(240, 242, 245),
            ForeColor = Color.FromArgb(33, 37, 41),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 8, 0)
        };
        btnViewTableMap.FlatAppearance.BorderSize = 0;
        btnViewTableMap.Click += async (s, e) => await SwitchToTableMapViewAsync();

        // Nút Đơn tạm giữ
        btnPendingOrders = new Button
        {
            Text = "📋 Đơn tạm giữ (0)",
            Size = new Size(140, 36),
            BackColor = Color.FromArgb(254, 242, 232),
            ForeColor = Color.FromArgb(217, 119, 6),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 8, 0)
        };
        btnPendingOrders.FlatAppearance.BorderSize = 0;
        btnPendingOrders.Click += btnPendingOrders_Click;

        flpTopLeft.Controls.AddRange(new Control[]
        {
            lblBrand, btnViewMenu, btnViewTableMap, btnPendingOrders
        });

        // FlowLayoutPanel nhóm hành động hệ thống bên phải (RightToLeft tự động sắp xếp không bao giờ đè)
        var flpTopRight = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = Color.Transparent,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        btnLogout = new Button
        {
            Text = "🚪 Đăng xuất",
            Size = new Size(95, 36),
            BackColor = Color.FromArgb(248, 215, 218),
            ForeColor = Color.FromArgb(132, 32, 41),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 0, 0)
        };
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.Click += (s, e) =>
        {
            RequestedLogout = true;
            Close();
        };

        lblCurrentUser = new Label
        {
            Text = UserSession.DisplayName,
            AutoSize = true,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(60, 70, 85),
            Margin = new Padding(12, 9, 12, 0)
        };

        btnRevenueReport.Size = new Size(115, 36);
        btnRevenueReport.Text = "📊 Doanh thu";
        btnRevenueReport.Margin = new Padding(6, 0, 6, 0);

        btnManageProducts.Size = new Size(125, 36);
        btnManageProducts.Text = "⚙ Quản lý món";
        btnManageProducts.Margin = new Padding(6, 0, 6, 0);

        flpTopRight.Controls.AddRange(new Control[]
        {
            btnLogout, lblCurrentUser, btnRevenueReport, btnManageProducts
        });

        pnlTopBar.Controls.Add(flpTopRight);
        pnlTopBar.Controls.Add(flpTopLeft);

        pnlTopBar.Resize += (s, e) => UpdateTopBarResponsiveLayout();

        // 2. Tách pnlMain thành 2 view panels
        pnlMain.Controls.Remove(flpProducts);
        pnlMain.Controls.Remove(pnlCategories);

        pnlMenuView = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };
        pnlMenuView.Controls.Add(flpProducts);
        pnlMenuView.Controls.Add(pnlCategories);
        pnlCategories.BringToFront();

        // 3. Khởi tạo Sơ đồ bàn (pnlTableMapView)
        pnlTableMapView = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(245, 247, 250),
            Visible = false
        };

        // Header trong Sơ đồ bàn
        var pnlTableMapHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 46,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(12, 5, 12, 5)
        };

        lblTableStatusSummary = new Label
        {
            Text = "🟢 Bàn trống: 17  |  🟠 Đang phục vụ: 0",
            Dock = DockStyle.Left,
            AutoSize = true,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 7, 0, 0)
        };

        var flpTableActions = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = Color.Transparent,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        var btnRefreshMap = new Button
        {
            Text = "🔄 Làm mới",
            Size = new Size(95, 34),
            BackColor = Color.FromArgb(240, 242, 245),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 0, 0)
        };
        btnRefreshMap.FlatAppearance.BorderSize = 0;
        btnRefreshMap.Click += async (s, e) => await LoadTableMapAsync();

        var btnMoveTable = new Button
        {
            Text = "⇄ Chuyển bàn",
            Size = new Size(110, 34),
            BackColor = Color.FromArgb(231, 241, 255),
            ForeColor = Color.FromArgb(13, 110, 253),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(6, 0, 6, 0)
        };
        btnMoveTable.FlatAppearance.BorderSize = 0;
        btnMoveTable.Click += btnMoveTable_Click;

        var btnMergeTable = new Button
        {
            Text = "☵ Gộp bàn",
            Size = new Size(100, 34),
            BackColor = Color.FromArgb(254, 242, 232),
            ForeColor = Color.FromArgb(217, 119, 6),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(6, 0, 6, 0)
        };
        btnMergeTable.FlatAppearance.BorderSize = 0;
        btnMergeTable.Click += btnMergeTable_Click;

        flpTableActions.Controls.AddRange(new Control[]
        {
            btnRefreshMap, btnMoveTable, btnMergeTable
        });

        pnlTableMapHeader.Controls.Add(flpTableActions);
        pnlTableMapHeader.Controls.Add(lblTableStatusSummary);

        // FlowLayoutPanel chứa các ô bàn
        flpTables = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(12),
            BackColor = Color.FromArgb(245, 247, 250)
        };

        pnlTableMapView.Controls.Add(flpTables);
        pnlTableMapView.Controls.Add(pnlTableMapHeader);

        pnlMain.Controls.Add(pnlMenuView);
        pnlMain.Controls.Add(pnlTableMapView);
        pnlMenuView.BringToFront();

        // 4. Thêm nút "Đổi bàn" vào pnlCartHeader
        btnSwitchTable = new Button
        {
            Text = "Đổi bàn",
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(pnlCartHeader.Width - 95, 10),
            Size = new Size(80, 28),
            BackColor = Color.FromArgb(231, 241, 255),
            ForeColor = Color.FromArgb(13, 110, 253),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnSwitchTable.FlatAppearance.BorderSize = 0;
        btnSwitchTable.Click += async (s, e) => await SwitchToTableMapViewAsync();

        lblTable.Location = new Point(110, 14);
        lblTable.Size = new Size(pnlCartHeader.Width - 220, 22);
        lblTable.TextAlign = ContentAlignment.MiddleRight;
        pnlCartHeader.Controls.Add(btnSwitchTable);
    }

    private void UpdateTopBarResponsiveLayout()
    {
        if (btnPendingOrders == null || lblBrand == null) return;

        if (pnlTopBar.Width < 1080)
        {
            lblBrand.Text = "☕ POS";
            btnPendingOrders.Text = $"📋 Tạm ({_lastPendingCount})";
            btnViewTableMap.Text = "🪑 Bàn";
            btnManageProducts.Text = "⚙ Món";
            btnRevenueReport.Text = "📊 Báo cáo";
        }
        else
        {
            lblBrand.Text = "☕ POS FASTFOOD & CAFE";
            btnPendingOrders.Text = $"📋 Đơn tạm giữ ({_lastPendingCount})";
            btnViewTableMap.Text = "🪑 Sơ đồ bàn";
            btnManageProducts.Text = "⚙ Quản lý món";
            btnRevenueReport.Text = "📊 Doanh thu";
        }
    }

    private void SwitchToMenuView()
    {
        pnlTableMapView.Visible = false;
        pnlMenuView.Visible = true;
        pnlMenuView.BringToFront();

        btnViewMenu.BackColor = Color.FromArgb(13, 110, 253);
        btnViewMenu.ForeColor = Color.White;
        btnViewTableMap.BackColor = Color.FromArgb(240, 242, 245);
        btnViewTableMap.ForeColor = Color.FromArgb(33, 37, 41);
    }

    private async Task SwitchToTableMapViewAsync()
    {
        pnlMenuView.Visible = false;
        pnlTableMapView.Visible = true;
        pnlTableMapView.BringToFront();

        btnViewTableMap.BackColor = Color.FromArgb(13, 110, 253);
        btnViewTableMap.ForeColor = Color.White;
        btnViewMenu.BackColor = Color.FromArgb(240, 242, 245);
        btnViewMenu.ForeColor = Color.FromArgb(33, 37, 41);

        await LoadTableMapAsync();
    }

    #endregion

    #region Cart Grid Setup & Calculation

    private void SetupCartGrid()
    {
        dgvCart.AutoGenerateColumns = false;
        dgvCart.AllowUserToAddRows = false;
        dgvCart.AllowUserToDeleteRows = false;
        dgvCart.AllowUserToResizeRows = false;
        dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCart.MultiSelect = false;
        dgvCart.RowHeadersVisible = false;
        dgvCart.BackgroundColor = Color.White;
        dgvCart.BorderStyle = BorderStyle.None;
        dgvCart.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvCart.GridColor = Color.FromArgb(235, 238, 242);
        dgvCart.RowTemplate.Height = 44; // Tăng chiều cao để hiện tên món + ghi chú topping gọn gàng

        dgvCart.Columns.Clear();

        // 1. Cột Tên món & Tùy chọn Topping
        var colName = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(CartItem.DisplayNameWithNotes),
            HeaderText = "Tên món & Tùy chọn",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            ReadOnly = true,
            DefaultCellStyle =
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                WrapMode = DataGridViewTriState.True,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular)
            }
        };

        // 2. Nút '-' giảm số lượng
        var colMinus = new DataGridViewButtonColumn
        {
            Name = "colMinus",
            HeaderText = "",
            Text = "-",
            UseColumnTextForButtonValue = true,
            Width = 30,
            FlatStyle = FlatStyle.Flat,
            DefaultCellStyle =
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(220, 53, 69),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            }
        };

        // 3. Cột Số lượng (Quantity)
        var colQuantity = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(CartItem.Quantity),
            HeaderText = "SL",
            Width = 36,
            ReadOnly = true,
            DefaultCellStyle =
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            }
        };

        // 4. Nút '+' tăng số lượng
        var colPlus = new DataGridViewButtonColumn
        {
            Name = "colPlus",
            HeaderText = "",
            Text = "+",
            UseColumnTextForButtonValue = true,
            Width = 30,
            FlatStyle = FlatStyle.Flat,
            DefaultCellStyle =
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(40, 167, 69),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            }
        };

        // 5. Cột Đơn giá (UnitPrice)
        var colUnitPrice = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(CartItem.UnitPrice),
            HeaderText = "Đơn giá",
            Width = 80,
            ReadOnly = true,
            DefaultCellStyle =
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Format = "#,##0 ₫",
                FormatProvider = _cultureVi
            }
        };

        // 6. Cột Tổng (LineTotal)
        var colLineTotal = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(CartItem.LineTotal),
            HeaderText = "Tổng",
            Width = 90,
            ReadOnly = true,
            DefaultCellStyle =
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Format = "#,##0 ₫",
                FormatProvider = _cultureVi
            }
        };

        dgvCart.Columns.AddRange(colName, colMinus, colQuantity, colPlus, colUnitPrice, colLineTotal);
        dgvCart.DataSource = _cartItems;
        dgvCart.CellContentClick += dgvCart_CellContentClick;
    }

    private void dgvCart_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _cartItems.Count)
            return;

        string colName = dgvCart.Columns[e.ColumnIndex].Name;
        var item = _cartItems[e.RowIndex];

        if (colName == "colPlus")
        {
            item.Quantity++;
            _cartItems.ResetItem(e.RowIndex);
            CalculateTotal();
        }
        else if (colName == "colMinus")
        {
            item.Quantity--;
            if (item.Quantity <= 0)
            {
                _cartItems.RemoveAt(e.RowIndex);
            }
            else
            {
                _cartItems.ResetItem(e.RowIndex);
            }
            CalculateTotal();
        }
    }

    /// <summary>
    /// Xử lý khi click vào thẻ sản phẩm: Mở popup chọn Cỡ ly, Đường, Đá, Topping (Modifiers)
    /// </summary>
    private async void HandleProductClicked(object? sender, ProductClickedEventArgs e)
    {
        try
        {
            var product = await ProductDAL.GetProductByIdAsync(e.Id) ?? new Product
            {
                ProductID = e.Id,
                ProductName = e.ProductName,
                Price = e.Price
            };

            using var dlg = new frmProductModifier(product);
            if (dlg.ShowDialog(this) == DialogResult.OK && dlg.ResultCartItem != null)
            {
                var newItem = dlg.ResultCartItem;

                // Kiểm tra xem món cùng ID VÀ cùng tùy chọn (Notes) đã có trong giỏ chưa
                var existingItem = _cartItems.FirstOrDefault(x => x.ProductId == newItem.ProductId && x.Notes == newItem.Notes);

                if (existingItem != null)
                {
                    existingItem.Quantity += newItem.Quantity;
                    int index = _cartItems.IndexOf(existingItem);
                    _cartItems.ResetItem(index);
                }
                else
                {
                    _cartItems.Add(newItem);
                }

                CalculateTotal();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi chọn món: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void CalculateTotal()
    {
        _subtotal = _cartItems.Sum(x => x.LineTotal);
        _taxAmount = Math.Round(_subtotal * TaxRate, 0);
        _totalAmount = Math.Max(0m, _subtotal + _taxAmount - _discountAmount);

        lblSubtotal.Text = _subtotal.ToString("#,##0 ₫", _cultureVi);
        lblTax.Text = _taxAmount.ToString("#,##0 ₫", _cultureVi);
        lblDiscount.Text = _discountAmount.ToString("#,##0 ₫", _cultureVi);
        lblTotal.Text = _totalAmount.ToString("#,##0 ₫", _cultureVi);

        dgvCart.Refresh();
    }

    public void SetDiscount(decimal discount)
    {
        _discountAmount = Math.Max(0m, discount);
        CalculateTotal();
    }

    public void ResetOrder()
    {
        _cartItems.Clear();
        _discountAmount = 0m;
        _currentLoadedOrderId = 0;
        lblTable.Text = "Bàn 01";
        CalculateTotal();
    }

    #endregion

    #region Action Buttons (Thanh toán, Tạm giữ, Đơn Pending)

    private void btnCancelOrder_Click(object? sender, EventArgs e)
    {
        if (_cartItems.Count == 0 && _discountAmount == 0)
            return;

        var confirm = MessageBox.Show(
            "Bạn có chắc chắn muốn hủy giỏ hàng hiện tại?",
            "Xác nhận hủy đơn",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            ResetOrder();
        }
    }

    /// <summary>
    /// Tạm giữ đơn hàng với Status = 'Pending' (hoặc cập nhật đơn Pending hiện tại)
    /// </summary>
    private async void btnHoldOrder_Click(object? sender, EventArgs e)
    {
        if (_cartItems.Count == 0)
        {
            MessageBox.Show("Giỏ hàng đang trống, không thể tạm giữ đơn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string cashier = UserSession.CurrentUser?.FullName ?? "Thu ngân 01";
        int orderId;

        if (_currentLoadedOrderId > 0)
        {
            // Cập nhật lại đơn Pending đã có
            bool ok = await OrderRepository.UpdateOrderAsync(
                _currentLoadedOrderId,
                "Pending",
                _cartItems,
                lblTable.Text,
                cashier,
                _subtotal,
                _taxAmount,
                _discountAmount,
                _totalAmount);

            orderId = ok ? _currentLoadedOrderId : -1;
        }
        else
        {
            // Tạo mới đơn Pending
            orderId = await OrderRepository.SaveOrderAsync(
                status: "Pending",
                cartItems: _cartItems,
                tableNumber: lblTable.Text,
                cashierName: cashier,
                subtotal: _subtotal,
                taxAmount: _taxAmount,
                discountAmount: _discountAmount,
                totalAmount: _totalAmount);
        }

        if (orderId > 0)
        {
            SaveOrderSnapshot(orderId, "Tạm giữ", 0, 0);
            MessageBox.Show($"Đã tạm giữ đơn hàng #{orderId} cho {lblTable.Text} thành công!", "Tạm giữ đơn", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetOrder();
            await RefreshPendingCountAsync();
            await LoadTableMapAsync();
        }
    }

    private void btnAddDiscount_Click(object? sender, EventArgs e)
    {
        if (_cartItems.Count == 0)
        {
            MessageBox.Show("Vui lòng chọn món vào giỏ hàng trước khi áp dụng giảm giá!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        decimal maxApplicable = _subtotal + _taxAmount;
        using var dlg = new frmDiscountInput(maxApplicable);

        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            SetDiscount(dlg.CalculatedDiscount);
        }
    }

    /// <summary>
    /// Thanh toán đơn hàng: Mở popup tính tiền thừa hoặc mã VietQR động, lưu 'Completed'
    /// </summary>
    private async void btnPay_Click(object? sender, EventArgs e)
    {
        if (_cartItems.Count == 0)
        {
            MessageBox.Show("Giỏ hàng đang trống, không thể thanh toán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string cashier = UserSession.CurrentUser?.FullName ?? "Thu ngân 01";

        using var dlgPayment = new frmPayment(_totalAmount, lblTable.Text, _currentLoadedOrderId, cashier);
        if (dlgPayment.ShowDialog(this) != DialogResult.OK)
        {
            return; // Người dùng đóng hộp thoại thanh toán
        }

        int orderId;

        if (_currentLoadedOrderId > 0)
        {
            // Cập nhật đơn Pending cũ thành 'Completed'
            bool ok = await OrderRepository.UpdateOrderAsync(
                _currentLoadedOrderId,
                "Completed",
                _cartItems,
                lblTable.Text,
                cashier,
                _subtotal,
                _taxAmount,
                _discountAmount,
                _totalAmount,
                dlgPayment.PaymentMethod);

            orderId = ok ? _currentLoadedOrderId : -1;
        }
        else
        {
            // Tạo đơn mới 'Completed'
            orderId = await OrderRepository.SaveOrderAsync(
                status: "Completed",
                cartItems: _cartItems,
                tableNumber: lblTable.Text,
                cashierName: cashier,
                subtotal: _subtotal,
                taxAmount: _taxAmount,
                discountAmount: _discountAmount,
                totalAmount: _totalAmount,
                paymentMethod: dlgPayment.PaymentMethod);
        }

        if (orderId > 0)
        {
            SaveOrderSnapshot(orderId, dlgPayment.PaymentMethod, dlgPayment.CustomerCash, dlgPayment.ChangeAmount);

            string paymentSummary = $"Thanh toán thành công đơn hàng #{orderId}!\n" +
                                    $"Vị trí: {lblTable.Text}\n" +
                                    $"Phương thức: {dlgPayment.PaymentMethod}\n" +
                                    $"Tổng tiền: {_totalAmount.ToString("#,##0 ₫", _cultureVi)}\n";

            if (dlgPayment.PaymentMethod == "Tiền mặt")
            {
                paymentSummary += $"Tiền khách đưa: {dlgPayment.CustomerCash.ToString("#,##0 ₫", _cultureVi)}\n" +
                                  $"Tiền thối lại: {dlgPayment.ChangeAmount.ToString("#,##0 ₫", _cultureVi)}\n";
            }

            paymentSummary += "\nBạn có muốn in hóa đơn không?";

            var printConfirm = MessageBox.Show(
                paymentSummary,
                "Thanh toán & In hóa đơn",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            ResetOrder();
            await RefreshPendingCountAsync();
            await LoadTableMapAsync();

            if (printConfirm == DialogResult.Yes)
            {
                PrintOrderSnapshot(previewFirst: true);
            }
        }
    }

    /// <summary>
    /// Mở danh sách các đơn tạm giữ (Pending) và nạp lại vào giỏ hàng
    /// </summary>
    private async void btnPendingOrders_Click(object? sender, EventArgs e)
    {
        using var dlg = new frmPendingOrders();
        if (dlg.ShowDialog(this) == DialogResult.OK && dlg.SelectedOrder != null)
        {
            await LoadPendingOrderIntoCartAsync(dlg.SelectedOrder);
        }

        await RefreshPendingCountAsync();
        await LoadTableMapAsync();
    }

    private async Task LoadPendingOrderIntoCartAsync(PendingOrderModel order)
    {
        if (_cartItems.Count > 0 && _currentLoadedOrderId != order.OrderID)
        {
            var confirm = MessageBox.Show(
                $"Giỏ hàng hiện tại đang có món của '{lblTable.Text}'.\nBạn có muốn thay thế bằng đơn của '{order.TableNumber}' không?",
                "Xác nhận nạp đơn",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;
        }

        var items = await OrderRepository.GetOrderDetailsByOrderIdAsync(order.OrderID);
        _cartItems.Clear();
        foreach (var item in items)
        {
            _cartItems.Add(item);
        }

        _currentLoadedOrderId = order.OrderID;
        lblTable.Text = order.TableNumber;
        _discountAmount = order.DiscountAmount;
        CalculateTotal();

        SwitchToMenuView();
    }

    private void btnPrint_Click(object? sender, EventArgs e)
    {
        if (_lastCreatedOrderId > 0 && _lastOrderItems.Count > 0)
        {
            PrintOrderSnapshot(previewFirst: true);
        }
        else if (_cartItems.Count > 0)
        {
            var printer = new ReceiptPrinter(
                orderId: 0,
                receiptCode: $"TT{DateTime.Now:yyyyMMddHHmmss}",
                orderDate: DateTime.Now,
                cashierName: UserSession.CurrentUser?.FullName ?? "Thu ngân 01",
                tableNumber: lblTable.Text,
                items: _cartItems,
                subtotal: _subtotal,
                tax: _taxAmount,
                discount: _discountAmount,
                total: _totalAmount,
                paymentMethod: "Tạm tính",
                customerCash: _totalAmount,
                changeAmount: 0m);

            printer.PrintPreview(this);
        }
        else
        {
            MessageBox.Show("Chưa có hóa đơn nào để in.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    #endregion

    #region Management & Reports (Phân quyền Admin)

    private async void btnManageProducts_Click(object? sender, EventArgs e)
    {
        if (!UserSession.IsAdmin)
        {
            MessageBox.Show("Chỉ tài khoản Quản trị viên (Admin) mới có quyền truy cập quản lý món!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int targetCatId = _currentCategoryButton?.Tag is int id ? id : LastActiveCategoryId;

        using var frm = new frmProductManagement();
        frm.OnMenuChanged += async (s, args) =>
        {
            targetCatId = args.CategoryId;
            await ReloadCategoriesAndProductsAsync(targetCatId);
        };
        frm.ShowDialog(this);
        await ReloadCategoriesAndProductsAsync(targetCatId);
    }

    private void btnRevenueReport_Click(object? sender, EventArgs e)
    {
        if (!UserSession.IsAdmin)
        {
            MessageBox.Show("Chỉ tài khoản Quản trị viên (Admin) mới có quyền xem báo cáo doanh thu!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var frm = new frmRevenueReport();
        frm.ShowDialog(this);
    }

    public async Task ReloadCategoriesAndProductsAsync(int targetCategoryId = -1)
    {
        int catIdToSelect = targetCategoryId >= 0
            ? targetCategoryId
            : (_currentCategoryButton?.Tag is int id ? id : LastActiveCategoryId);

        await LoadCategoriesAsync(catIdToSelect);
    }

    #endregion

    #region Table Map (Sơ đồ bàn, Chuyển bàn, Gộp bàn)

    /// <summary>
    /// Tải và vẽ sơ đồ các bàn với màu sắc trạng thái trực quan:
    /// Màu xanh: Bàn trống.
    /// Màu cam/đỏ: Bàn đang có khách ngồi ăn (Status = 'Pending').
    /// </summary>
    private async Task LoadTableMapAsync()
    {
        try
        {
            var pendingOrders = await OrderRepository.GetPendingOrdersAsync();
            var pendingDict = pendingOrders.ToDictionary(x => x.TableNumber, StringComparer.OrdinalIgnoreCase);

            int occupiedCount = pendingDict.Count;
            int emptyCount = _allTableNames.Count - occupiedCount;

            lblTableStatusSummary.Text = $"🟢 Bàn trống: {emptyCount}  |  🟠 Đang phục vụ: {occupiedCount}";
            btnViewTableMap.Text = $"🪑 Sơ đồ bàn ({occupiedCount})";

            flpTables.SuspendLayout();
            flpTables.Controls.Clear();

            foreach (var tableName in _allTableNames)
            {
                bool isOccupied = pendingDict.TryGetValue(tableName, out var order);
                var card = CreateTableCard(tableName, isOccupied, order);
                flpTables.Controls.Add(card);
            }

            flpTables.ResumeLayout();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi nạp sơ đồ bàn: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Control CreateTableCard(string tableName, bool isOccupied, PendingOrderModel? order)
    {
        var pnl = new Panel
        {
            Size = new Size(150, 115),
            Margin = new Padding(8),
            Cursor = Cursors.Hand,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(8)
        };

        if (isOccupied && order != null)
        {
            // Bàn đang có khách
            pnl.BackColor = Color.FromArgb(255, 243, 235);

            var lblName = new Label
            {
                Text = tableName.ToUpper(),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(183, 28, 28),
                Dock = DockStyle.Top,
                Height = 22
            };

            var lblBadge = new Label
            {
                Text = "🟠 CÓ KHÁCH",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(217, 119, 6),
                Dock = DockStyle.Top,
                Height = 16
            };

            var lblAmount = new Label
            {
                Text = order.TotalAmount.ToString("#,##0 ₫", _cultureVi),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                Dock = DockStyle.Top,
                Height = 24
            };

            var lblTime = new Label
            {
                Text = $"Đơn #{order.OrderID} • {order.OrderDate:HH:mm}",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Dock = DockStyle.Fill
            };

            pnl.Controls.AddRange(new Control[] { lblTime, lblAmount, lblBadge, lblName });

            // Gán sự kiện click vào bàn có khách -> Mở lại đơn nạp vào giỏ
            void TableClick(object? s, EventArgs e)
            {
                _ = LoadPendingOrderIntoCartAsync(order);
            }
            pnl.Click += TableClick;
            lblName.Click += TableClick;
            lblBadge.Click += TableClick;
            lblAmount.Click += TableClick;
            lblTime.Click += TableClick;
        }
        else
        {
            // Bàn trống
            pnl.BackColor = Color.FromArgb(235, 247, 238);

            var lblName = new Label
            {
                Text = tableName.ToUpper(),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 135, 84),
                Dock = DockStyle.Top,
                Height = 22
            };

            var lblBadge = new Label
            {
                Text = "🟢 BÀN TRỐNG",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 135, 84),
                Dock = DockStyle.Top,
                Height = 20
            };

            var lblSub = new Label
            {
                Text = "Bấm để chọn bàn này",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnl.Controls.AddRange(new Control[] { lblSub, lblBadge, lblName });

            // Gán sự kiện click vào bàn trống -> Gán bàn cho giỏ hàng và chuyển sang Thực đơn
            void EmptyTableClick(object? s, EventArgs e)
            {
                lblTable.Text = tableName;
                SwitchToMenuView();
            }
            pnl.Click += EmptyTableClick;
            lblName.Click += EmptyTableClick;
            lblBadge.Click += EmptyTableClick;
            lblSub.Click += EmptyTableClick;
        }

        return pnl;
    }

    private async void btnMoveTable_Click(object? sender, EventArgs e)
    {
        var pendingOrders = await OrderRepository.GetPendingOrdersAsync();
        if (pendingOrders.Count == 0)
        {
            MessageBox.Show("Hiện không có bàn nào đang có khách để chuyển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new frmMoveTable(pendingOrders, _allTableNames, lblTable.Text);
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            await LoadTableMapAsync();
            await RefreshPendingCountAsync();
        }
    }

    private async void btnMergeTable_Click(object? sender, EventArgs e)
    {
        var pendingOrders = await OrderRepository.GetPendingOrdersAsync();
        if (pendingOrders.Count < 2)
        {
            MessageBox.Show("Cần ít nhất 2 bàn đang có khách để thực hiện gộp bàn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new frmMergeTable(pendingOrders, lblTable.Text);
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            await LoadTableMapAsync();
            await RefreshPendingCountAsync();
            ResetOrder();
        }
    }

    private async Task RefreshPendingCountAsync()
    {
        int count = await OrderRepository.GetPendingOrderCountAsync();
        _lastPendingCount = count;
        UpdateTopBarResponsiveLayout();

        if (count > 0)
        {
            btnPendingOrders.BackColor = Color.FromArgb(253, 126, 20);
            btnPendingOrders.ForeColor = Color.White;
        }
        else
        {
            btnPendingOrders.BackColor = Color.FromArgb(254, 242, 232);
            btnPendingOrders.ForeColor = Color.FromArgb(217, 119, 6);
        }
    }

    #endregion

    #region Printing Helpers

    private void SaveOrderSnapshot(int orderId, string paymentMethod, decimal customerCash, decimal changeAmount)
    {
        _lastCreatedOrderId = orderId;
        _lastOrderItems = _cartItems.Select(x => new CartItem
        {
            ProductId = x.ProductId,
            ProductName = x.ProductName,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            Notes = x.Notes
        }).ToList();

        _lastSubtotal = _subtotal;
        _lastTax = _taxAmount;
        _lastDiscount = _discountAmount;
        _lastTotal = _totalAmount;
        _lastTableNumber = lblTable.Text;
        _lastOrderDate = DateTime.Now;
        _lastPaymentMethod = paymentMethod;
        _lastCustomerCash = customerCash;
        _lastChangeAmount = changeAmount;
    }

    private void PrintOrderSnapshot(bool previewFirst)
    {
        var printer = new ReceiptPrinter(
            orderId: _lastCreatedOrderId,
            receiptCode: $"HD{_lastOrderDate:yyyyMMddHHmmss}",
            orderDate: _lastOrderDate,
            cashierName: UserSession.CurrentUser?.FullName ?? "Thu ngân 01",
            tableNumber: _lastTableNumber,
            items: _lastOrderItems,
            subtotal: _lastSubtotal,
            tax: _lastTax,
            discount: _lastDiscount,
            total: _lastTotal,
            paymentMethod: _lastPaymentMethod,
            customerCash: _lastCustomerCash,
            changeAmount: _lastChangeAmount);

        if (previewFirst)
        {
            printer.PrintPreview(this);
        }
        else
        {
            printer.Print();
        }
    }

    #endregion

    #region Form Events

    private async void frmPOS_Load(object? sender, EventArgs e)
    {
        // Phân quyền hiển thị
        lblCurrentUser.Text = UserSession.DisplayName;
        bool isAdmin = UserSession.IsAdmin;
        btnManageProducts.Visible = isAdmin;
        btnRevenueReport.Visible = isAdmin;

        CalculateTotal();
        await LoadCategoriesAsync(LastActiveCategoryId);
        await RefreshPendingCountAsync();
        await LoadTableMapAsync();
    }

    private Button CreateCategoryButton(string text, int categoryId)
    {
        var btn = new Button
        {
            Text = text,
            Tag = categoryId,
            Height = 40,
            AutoSize = true,
            MinimumSize = new Size(110, 40),
            Margin = new Padding(4, 2, 6, 2),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
            BackColor = _catBtnDefaultBg,
            ForeColor = _catBtnDefaultFg,
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.Click += CategoryButton_Click;
        return btn;
    }

    public async Task LoadCategoriesAsync(int targetCategoryId = -1)
    {
        try
        {
            List<Category> categories = await CategoryDAL.GetCategories();

            pnlCategories.SuspendLayout();
            pnlCategories.Controls.Clear();

            int selectedId = targetCategoryId >= 0 ? targetCategoryId : LastActiveCategoryId;

            // 1. Luôn thêm tab "⭐ Tất cả" ở vị trí đầu tiên để thấy toàn bộ món
            var btnAll = CreateCategoryButton("⭐ Tất cả", 0);
            pnlCategories.Controls.Add(btnAll);

            Button selectTargetBtn = btnAll;

            // 2. Thêm các tab danh mục từ cơ sở dữ liệu
            foreach (var cat in categories)
            {
                var btn = CreateCategoryButton(cat.CategoryName, cat.CategoryID);
                pnlCategories.Controls.Add(btn);

                if (cat.CategoryID == selectedId)
                {
                    selectTargetBtn = btn;
                }
            }

            pnlCategories.ResumeLayout();

            // Kích hoạt tab mục tiêu (tự động load món tương ứng)
            selectTargetBtn.PerformClick();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi tải danh mục: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void CategoryButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Button clickedButton || clickedButton.Tag is not int categoryId)
            return;

        if (_currentCategoryButton != null)
        {
            _currentCategoryButton.BackColor = _catBtnDefaultBg;
            _currentCategoryButton.ForeColor = _catBtnDefaultFg;
        }

        _currentCategoryButton = clickedButton;
        _currentCategoryButton.BackColor = _catBtnSelectedBg;
        _currentCategoryButton.ForeColor = _catBtnSelectedFg;

        LastActiveCategoryId = categoryId;

        await LoadProductsByCategory(categoryId);
    }

    public async Task LoadProductsByCategory(int categoryId)
    {
        try
        {
            foreach (Control ctrl in flpProducts.Controls)
            {
                if (ctrl is ucProductCard card)
                {
                    card.Image?.Dispose();
                }
                ctrl.Dispose();
            }
            flpProducts.Controls.Clear();

            // Nếu categoryId == 0: Lấy tất cả món ăn trong toàn bộ thực đơn
            List<Product> products = categoryId == 0
                ? await ProductDAL.GetAllProductsAsync()
                : await ProductDAL.GetProductsByCategory(categoryId);

            flpProducts.SuspendLayout();

            foreach (var prod in products)
            {
                var card = new ucProductCard
                {
                    Id = prod.ProductID,
                    ProductName = prod.ProductName,
                    Price = prod.Price,
                    Image = LoadImageWithoutLocking(prod.ImagePath)
                };

                card.OnProductClicked += HandleProductClicked;
                flpProducts.Controls.Add(card);
            }

            flpProducts.ResumeLayout();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi tải danh sách món: {ex.Message}", "Lỗi tải món", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    #endregion

    #region Image Helper (Non-locking)

    private static Image LoadImageWithoutLocking(string? imagePath)
    {
        if (!string.IsNullOrWhiteSpace(imagePath))
        {
            string fullPath = Path.IsPathRooted(imagePath) 
                ? imagePath 
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);

            if (File.Exists(fullPath))
            {
                try
                {
                    using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var sourceImage = Image.FromStream(stream);
                    return new Bitmap(sourceImage);
                }
                catch
                {
                    // Chuyển sang ảnh mặc định
                }
            }
        }

        return CreateDefaultImage();
    }

    private static Image CreateDefaultImage()
    {
        var bmp = new Bitmap(108, 65);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.FromArgb(240, 243, 246));

        using var font = new Font("Segoe UI", 9F, FontStyle.Regular);
        using var brush = new SolidBrush(Color.FromArgb(140, 150, 160));
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        g.DrawString("No Image", font, brush, new RectangleF(0, 0, bmp.Width, bmp.Height), format);
        return bmp;
    }

    #endregion
}
