using System.Globalization;
using MyWinFormsApp.DataAccess;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Màn hình danh sách các đơn hàng đang tạm giữ (Status = 'Pending')
/// Cho phép thu ngân chọn một đơn để nạp lại vào giỏ hàng hoặc hủy đơn
/// </summary>
public class frmPendingOrders : Form
{
    private DataGridView dgvOrders = null!;
    private TextBox txtSearch = null!;
    private Button btnReopen = null!;
    private Button btnCancel = null!;
    private Button btnRefresh = null!;
    private Button btnClose = null!;
    private Label lblCount = null!;

    private readonly CultureInfo _cultureVi = new("vi-VN");
    private List<PendingOrderModel> _allPendingOrders = new();

    /// <summary>
    /// Đơn hàng được người dùng chọn mở lại
    /// </summary>
    public PendingOrderModel? SelectedOrder { get; private set; }

    public frmPendingOrders()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        Text = "Danh sách đơn hàng tạm giữ (Pending Orders)";
        Size = new Size(880, 520);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        // Header Panel
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 55,
            BackColor = Color.FromArgb(253, 126, 20), // Màu cam nổi bật cho Pending
            Padding = new Padding(16, 0, 16, 0)
        };
        var lblTitle = new Label
        {
            Text = "DANH SÁCH ĐƠN HÀNG ĐANG TẠM GIỮ",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            Dock = DockStyle.Left,
            Width = 400,
            TextAlign = ContentAlignment.MiddleLeft
        };
        lblCount = new Label
        {
            Text = "0 đơn đang chờ",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            Dock = DockStyle.Right,
            Width = 200,
            TextAlign = ContentAlignment.MiddleRight
        };
        pnlHeader.Controls.Add(lblCount);
        pnlHeader.Controls.Add(lblTitle);

        // Filter / Search Panel
        var pnlSearch = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(248, 249, 250),
            Padding = new Padding(16, 10, 16, 10)
        };
        var lblSearch = new Label { Text = "Tìm bàn / Mã đơn:", Location = new Point(16, 15), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        txtSearch = new TextBox { Location = new Point(140, 12), Width = 220, Font = new Font("Segoe UI", 9.5F) };
        txtSearch.TextChanged += (s, e) => FilterList();

        btnRefresh = new Button
        {
            Text = "Làm mới",
            Location = new Point(370, 10),
            Size = new Size(90, 30),
            BackColor = Color.FromArgb(230, 235, 240),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += async (s, e) => await LoadDataAsync();

        pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnRefresh });

        // Action Panel at bottom
        var pnlActions = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 10, 16, 10)
        };

        btnReopen = new Button
        {
            Text = "MỞ LẠI ĐƠN NÀY (F2)",
            Dock = DockStyle.Right,
            Width = 170,
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnReopen.FlatAppearance.BorderSize = 0;
        btnReopen.Click += BtnReopen_Click;

        btnCancel = new Button
        {
            Text = "Hủy đơn tạm giữ",
            Dock = DockStyle.Right,
            Width = 140,
            BackColor = Color.FromArgb(248, 215, 218),
            ForeColor = Color.FromArgb(132, 32, 41),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 10, 0)
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += BtnCancel_Click;

        btnClose = new Button
        {
            Text = "Đóng",
            Dock = DockStyle.Left,
            Width = 90,
            BackColor = Color.FromArgb(240, 242, 245),
            ForeColor = Color.FromArgb(33, 37, 41),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.Cancel
        };
        btnClose.FlatAppearance.BorderSize = 0;

        pnlActions.Controls.Add(btnCancel);
        pnlActions.Controls.Add(btnReopen);
        pnlActions.Controls.Add(btnClose);

        // DataGridView
        dgvOrders = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            RowTemplate = { Height = 34 }
        };
        dgvOrders.DoubleClick += (s, e) => BtnReopen_Click(s, e);

        SetupGridColumns();

        Controls.Add(dgvOrders);
        Controls.Add(pnlActions);
        Controls.Add(pnlSearch);
        Controls.Add(pnlHeader);

        Load += async (s, e) => await LoadDataAsync();
    }

    private void SetupGridColumns()
    {
        dgvOrders.AutoGenerateColumns = false;
        dgvOrders.Columns.Clear();

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PendingOrderModel.OrderID),
            HeaderText = "Mã đơn",
            Width = 80,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PendingOrderModel.TableNumber),
            HeaderText = "Vị trí / Bàn",
            Width = 110,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PendingOrderModel.OrderDate),
            HeaderText = "Thời gian tạm giữ",
            Width = 160,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Format = "dd/MM/yyyy HH:mm:ss" }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PendingOrderModel.ItemCount),
            HeaderText = "Số món",
            Width = 85,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PendingOrderModel.CashierName),
            HeaderText = "Thu ngân",
            Width = 130,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PendingOrderModel.DiscountAmount),
            HeaderText = "Giảm giá",
            Width = 100,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "#,##0 ₫", FormatProvider = _cultureVi }
        });

        dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PendingOrderModel.TotalAmount),
            HeaderText = "Tổng thanh toán",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(220, 53, 69), Format = "#,##0 ₫", FormatProvider = _cultureVi }
        });
    }

    private async Task LoadDataAsync()
    {
        _allPendingOrders = await OrderRepository.GetPendingOrdersAsync();
        FilterList();
    }

    private void FilterList()
    {
        string keyword = txtSearch.Text.Trim().ToLower();
        var filtered = string.IsNullOrEmpty(keyword)
            ? _allPendingOrders
            : _allPendingOrders.Where(x =>
                x.OrderID.ToString().Contains(keyword) ||
                x.TableNumber.ToLower().Contains(keyword) ||
                x.CashierName.ToLower().Contains(keyword)
              ).ToList();

        dgvOrders.DataSource = filtered;
        lblCount.Text = $"{_allPendingOrders.Count} đơn đang chờ";
    }

    private void BtnReopen_Click(object? sender, EventArgs e)
    {
        if (dgvOrders.CurrentRow?.DataBoundItem is PendingOrderModel order)
        {
            SelectedOrder = order;
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show("Vui lòng chọn một đơn hàng trong danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private async void BtnCancel_Click(object? sender, EventArgs e)
    {
        if (dgvOrders.CurrentRow?.DataBoundItem is PendingOrderModel order)
        {
            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn hủy đơn hàng #{order.OrderID} của {order.TableNumber}?",
                "Xác nhận hủy đơn",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                bool success = await OrderRepository.CancelOrderAsync(order.OrderID);
                if (success)
                {
                    MessageBox.Show("Đã hủy đơn tạm giữ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn một đơn hàng để hủy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
