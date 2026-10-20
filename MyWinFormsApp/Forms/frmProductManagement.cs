using System.Globalization;
using MyWinFormsApp.DataAccess;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Màn hình quản lý danh sách món ăn & thực đơn (Thêm, Sửa, Xóa, chọn ảnh)
/// </summary>
public class frmProductManagement : Form
{
    #region Controls & Fields

    private DataGridView dgvProducts = null!;
    private TextBox txtProductName = null!;
    private TextBox txtPrice = null!;
    private ComboBox cboCategory = null!;
    private TextBox txtImagePath = null!;
    private PictureBox picPreview = null!;
    private Button btnBrowse = null!;
    private Button btnAdd = null!;
    private Button btnUpdate = null!;
    private Button btnDelete = null!;
    private Button btnReset = null!;

    private int _selectedProductId = 0;
    private readonly CultureInfo _cultureVi = new("vi-VN");

    /// <summary>
    /// Thông tin sự kiện khi thực đơn thay đổi
    /// </summary>
    public class MenuChangedEventArgs : EventArgs
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Sự kiện kích hoạt khi thực đơn thay đổi (để frmPOS đồng bộ lại ngay)
    /// </summary>
    public event EventHandler<MenuChangedEventArgs>? OnMenuChanged;

    #endregion

    #region Constructor

    public frmProductManagement()
    {
        InitializeUI();
    }

    #endregion

    #region UI Initialization

    private void InitializeUI()
    {
        Text = "Quản lý thực đơn món ăn";
        Size = new Size(1000, 620);
        MinimumSize = new Size(900, 550);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        // Header Panel
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(13, 110, 253),
            Padding = new Padding(15, 8, 15, 8)
        };
        var lblTitle = new Label
        {
            Text = "QUẢN LÝ DANH SÁCH MÓN ĂN & THỰC ĐƠN",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var btnManageUsers = new Button
        {
            Text = "👥 Quản lý nhân viên",
            Dock = DockStyle.Right,
            Width = 160,
            BackColor = Color.FromArgb(231, 241, 255),
            ForeColor = Color.FromArgb(13, 110, 253),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnManageUsers.FlatAppearance.BorderSize = 0;
        btnManageUsers.Click += (s, e) =>
        {
            using var frmUsers = new frmUserManagement();
            frmUsers.ShowDialog(this);
        };

        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(btnManageUsers);

        // Left Panel: Form nhập liệu
        var pnlInput = new Panel
        {
            Dock = DockStyle.Left,
            Width = 340,
            BackColor = Color.FromArgb(248, 249, 250),
            Padding = new Padding(16)
        };

        int y = 16;
        var lblName = new Label { Text = "Tên món ăn:", Location = new Point(16, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        txtProductName = new TextBox { Location = new Point(16, y), Width = 300, Font = new Font("Segoe UI", 9.5F) };
        y += 34;

        var lblCat = new Label { Text = "Danh mục:", Location = new Point(16, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        cboCategory = new ComboBox { Location = new Point(16, y), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
        y += 34;

        var lblPrice = new Label { Text = "Đơn giá (VNĐ):", Location = new Point(16, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        txtPrice = new TextBox { Location = new Point(16, y), Width = 300, Font = new Font("Segoe UI", 9.5F) };
        txtPrice.KeyPress += TxtPrice_KeyPress;
        y += 34;

        var lblImg = new Label { Text = "Hình ảnh món:", Location = new Point(16, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        txtImagePath = new TextBox { Location = new Point(16, y), Width = 210, ReadOnly = true, Font = new Font("Segoe UI", 8.5F) };
        btnBrowse = new Button
        {
            Text = "Chọn ảnh...",
            Location = new Point(232, y - 1),
            Width = 84,
            Height = 26,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(222, 226, 230),
            Cursor = Cursors.Hand
        };
        btnBrowse.FlatAppearance.BorderSize = 0;
        btnBrowse.Click += BtnBrowse_Click;
        y += 34;

        // Preview PictureBox
        picPreview = new PictureBox
        {
            Location = new Point(16, y),
            Size = new Size(300, 110),
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White
        };
        y += 122;

        // Action Buttons
        btnAdd = new Button
        {
            Text = "Thêm món mới",
            Location = new Point(16, y),
            Size = new Size(145, 36),
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.Click += BtnAdd_Click;

        btnUpdate = new Button
        {
            Text = "Cập nhật",
            Location = new Point(171, y),
            Size = new Size(145, 36),
            BackColor = Color.FromArgb(13, 110, 253),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnUpdate.FlatAppearance.BorderSize = 0;
        btnUpdate.Click += BtnUpdate_Click;
        y += 44;

        btnDelete = new Button
        {
            Text = "Xóa món",
            Location = new Point(16, y),
            Size = new Size(145, 36),
            BackColor = Color.FromArgb(220, 53, 69),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnDelete.FlatAppearance.BorderSize = 0;
        btnDelete.Click += BtnDelete_Click;

        btnReset = new Button
        {
            Text = "Làm mới / Bỏ chọn",
            Location = new Point(171, y),
            Size = new Size(145, 36),
            BackColor = Color.FromArgb(222, 226, 230),
            ForeColor = Color.FromArgb(33, 37, 41),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnReset.FlatAppearance.BorderSize = 0;
        btnReset.Click += (s, e) => ResetInputFields();

        pnlInput.Controls.AddRange(new Control[]
        {
            lblName, txtProductName,
            lblCat, cboCategory,
            lblPrice, txtPrice,
            lblImg, txtImagePath, btnBrowse,
            picPreview,
            btnAdd, btnUpdate,
            btnDelete, btnReset
        });

        // Center Panel: DataGridView
        var pnlGrid = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            BackColor = Color.White
        };

        dgvProducts = new DataGridView
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
        dgvProducts.CellClick += DgvProducts_CellClick;

        SetupGridColumns();
        pnlGrid.Controls.Add(dgvProducts);

        Controls.Add(pnlGrid);
        Controls.Add(pnlInput);
        Controls.Add(pnlHeader);

        Load += FrmProductManagement_Load;
    }

    private void SetupGridColumns()
    {
        dgvProducts.AutoGenerateColumns = false;
        dgvProducts.Columns.Clear();

        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Product.ProductID),
            HeaderText = "Mã",
            Width = 55,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });

        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Product.ProductName),
            HeaderText = "Tên món ăn",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft }
        });

        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Product.CategoryName),
            HeaderText = "Danh mục",
            Width = 110,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });

        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Product.Price),
            HeaderText = "Đơn giá",
            Width = 110,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "#,##0 ₫", FormatProvider = _cultureVi }
        });

        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Product.ImagePath),
            HeaderText = "Đường dẫn ảnh",
            Width = 140,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft }
        });
    }

    #endregion

    #region Data Loading & Binding

    private async void FrmProductManagement_Load(object? sender, EventArgs e)
    {
        await LoadCategoriesAsync();
        await LoadProductsAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await CategoryDAL.GetCategories();
        cboCategory.DataSource = categories;
        cboCategory.DisplayMember = nameof(Category.CategoryName);
        cboCategory.ValueMember = nameof(Category.CategoryID);
    }

    private async Task LoadProductsAsync()
    {
        var products = await ProductDAL.GetAllProductsAsync();
        dgvProducts.DataSource = products;
    }

    #endregion

    #region Event Handlers & CRUD Logic

    private void DgvProducts_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || dgvProducts.Rows[e.RowIndex].DataBoundItem is not Product product)
            return;

        _selectedProductId = product.ProductID;
        txtProductName.Text = product.ProductName;
        txtPrice.Text = product.Price.ToString("N0", _cultureVi);
        cboCategory.SelectedValue = product.CategoryID;
        txtImagePath.Text = product.ImagePath ?? string.Empty;

        LoadPreviewImage(product.ImagePath);
    }

    private void TxtPrice_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
        {
            e.Handled = true;
        }
    }

    private void BtnBrowse_Click(object? sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog
        {
            Title = "Chọn hình ảnh món ăn",
            Filter = "Ảnh (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Tất cả file (*.*)|*.*"
        };

        if (ofd.ShowDialog(this) == DialogResult.OK)
        {
            txtImagePath.Text = ofd.FileName;
            LoadPreviewImage(ofd.FileName);
        }
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput(out string name, out decimal price, out int categoryId))
            return;

        var newProduct = new Product
        {
            ProductName = name,
            Price = price,
            CategoryID = categoryId,
            ImagePath = string.IsNullOrWhiteSpace(txtImagePath.Text) ? null : txtImagePath.Text.Trim()
        };

        int newId = await ProductDAL.AddProductAsync(newProduct);
        if (newId > 0)
        {
            MessageBox.Show($"Thêm món '{name}' thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadProductsAsync();
            ResetInputFields();
            OnMenuChanged?.Invoke(this, new MenuChangedEventArgs { CategoryId = categoryId, ProductId = newId, ProductName = name });
        }
    }

    private async void BtnUpdate_Click(object? sender, EventArgs e)
    {
        if (_selectedProductId <= 0)
        {
            MessageBox.Show("Vui lòng chọn một món trong bảng để cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!ValidateInput(out string name, out decimal price, out int categoryId))
            return;

        var product = new Product
        {
            ProductID = _selectedProductId,
            ProductName = name,
            Price = price,
            CategoryID = categoryId,
            ImagePath = string.IsNullOrWhiteSpace(txtImagePath.Text) ? null : txtImagePath.Text.Trim()
        };

        bool ok = await ProductDAL.UpdateProductAsync(product);
        if (ok)
        {
            MessageBox.Show($"Cập nhật món #{_selectedProductId} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadProductsAsync();
            ResetInputFields();
            OnMenuChanged?.Invoke(this, new MenuChangedEventArgs { CategoryId = categoryId, ProductId = _selectedProductId, ProductName = name });
        }
    }

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (_selectedProductId <= 0)
        {
            MessageBox.Show("Vui lòng chọn một món trong bảng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa món '{txtProductName.Text}' (Mã: {_selectedProductId})?",
            "Xác nhận xóa món",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        bool ok = await ProductDAL.DeleteProductAsync(_selectedProductId);
        if (ok)
        {
            MessageBox.Show("Xóa món ăn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadProductsAsync();
            ResetInputFields();
            OnMenuChanged?.Invoke(this, new MenuChangedEventArgs { CategoryId = 0, ProductId = _selectedProductId });
        }
    }

    private void ResetInputFields()
    {
        _selectedProductId = 0;
        txtProductName.Clear();
        txtPrice.Clear();
        txtImagePath.Clear();
        if (cboCategory.Items.Count > 0) cboCategory.SelectedIndex = 0;
        picPreview.Image?.Dispose();
        picPreview.Image = null;
        dgvProducts.ClearSelection();
    }

    private bool ValidateInput(out string name, out decimal price, out int categoryId)
    {
        name = txtProductName.Text.Trim();
        price = 0;
        categoryId = 0;

        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Vui lòng nhập tên món ăn.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtProductName.Focus();
            return false;
        }

        string rawPrice = txtPrice.Text.Trim().Replace(".", "").Replace(",", ".");
        if (!decimal.TryParse(rawPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out price) || price < 0)
        {
            MessageBox.Show("Vui lòng nhập đơn giá hợp lệ (>= 0).", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPrice.Focus();
            return false;
        }

        if (cboCategory.SelectedValue is int catId)
        {
            categoryId = catId;
        }
        else
        {
            MessageBox.Show("Vui lòng chọn danh mục cho món.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cboCategory.Focus();
            return false;
        }

        return true;
    }

    private void LoadPreviewImage(string? imagePath)
    {
        picPreview.Image?.Dispose();
        picPreview.Image = null;

        if (string.IsNullOrWhiteSpace(imagePath)) return;

        string fullPath = Path.IsPathRooted(imagePath) 
            ? imagePath 
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);

        if (File.Exists(fullPath))
        {
            try
            {
                using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var sourceImage = Image.FromStream(stream);
                picPreview.Image = new Bitmap(sourceImage);
            }
            catch
            {
                picPreview.Image = null;
            }
        }
    }

    #endregion
}
