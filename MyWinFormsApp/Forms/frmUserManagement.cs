using MyWinFormsApp.DataAccess;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Màn hình quản lý tài khoản nhân viên (Thêm, Sửa, Xóa, Phân quyền) dành riêng cho Admin
/// </summary>
public class frmUserManagement : Form
{
    private DataGridView dgvUsers = null!;
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private TextBox txtFullName = null!;
    private ComboBox cboRole = null!;
    private CheckBox chkActive = null!;

    private Button btnAdd = null!;
    private Button btnUpdate = null!;
    private Button btnDelete = null!;
    private Button btnReset = null!;

    private int _selectedUserId = 0;

    public frmUserManagement()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        Text = "Quản lý tài khoản nhân viên & phân quyền";
        Size = new Size(860, 520);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        // Header Panel
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(13, 110, 253),
            Padding = new Padding(16, 0, 0, 0)
        };
        var lblTitle = new Label
        {
            Text = "QUẢN LÝ TÀI KHOẢN NHÂN VIÊN & PHÂN QUYỀN",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlHeader.Controls.Add(lblTitle);

        // Left Panel: Form nhập liệu
        var pnlInput = new Panel
        {
            Dock = DockStyle.Left,
            Width = 320,
            BackColor = Color.FromArgb(248, 249, 250),
            Padding = new Padding(16)
        };

        int y = 16;
        var lblUser = new Label { Text = "Tên đăng nhập:", Location = new Point(16, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        txtUsername = new TextBox { Location = new Point(16, y), Width = 280, Font = new Font("Segoe UI", 9.5F) };
        y += 34;

        var lblPass = new Label { Text = "Mật khẩu:", Location = new Point(16, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        txtPassword = new TextBox { Location = new Point(16, y), Width = 280, Font = new Font("Segoe UI", 9.5F) };
        y += 34;

        var lblName = new Label { Text = "Họ và tên nhân viên:", Location = new Point(16, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        txtFullName = new TextBox { Location = new Point(16, y), Width = 280, Font = new Font("Segoe UI", 9.5F) };
        y += 34;

        var lblRole = new Label { Text = "Vai trò (Phân quyền):", Location = new Point(16, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        cboRole = new ComboBox { Location = new Point(16, y), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
        cboRole.Items.AddRange(new object[] { "Cashier", "Admin" });
        cboRole.SelectedIndex = 0;
        y += 34;

        chkActive = new CheckBox { Text = "Đang kích hoạt (Hoạt động)", Location = new Point(16, y), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 40;

        // Nút hành động
        btnAdd = new Button { Text = "Thêm mới", Location = new Point(16, y), Size = new Size(135, 34), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.Click += async (s, e) => await AddUserAsync();

        btnUpdate = new Button { Text = "Cập nhật", Location = new Point(160, y), Size = new Size(135, 34), BackColor = Color.FromArgb(13, 110, 253), ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Enabled = false };
        btnUpdate.FlatAppearance.BorderSize = 0;
        btnUpdate.Click += async (s, e) => await UpdateUserAsync();
        y += 42;

        btnDelete = new Button { Text = "Xóa tài khoản", Location = new Point(16, y), Size = new Size(135, 34), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Enabled = false };
        btnDelete.FlatAppearance.BorderSize = 0;
        btnDelete.Click += async (s, e) => await DeleteUserAsync();

        btnReset = new Button { Text = "Làm mới form", Location = new Point(160, y), Size = new Size(135, 34), BackColor = Color.FromArgb(240, 242, 245), ForeColor = Color.FromArgb(33, 37, 41), Font = new Font("Segoe UI", 9F, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        btnReset.FlatAppearance.BorderSize = 0;
        btnReset.Click += (s, e) => ResetForm();

        pnlInput.Controls.AddRange(new Control[]
        {
            lblUser, txtUsername,
            lblPass, txtPassword,
            lblName, txtFullName,
            lblRole, cboRole,
            chkActive,
            btnAdd, btnUpdate,
            btnDelete, btnReset
        });

        // Right Panel: DataGridView
        dgvUsers = new DataGridView
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
            RowTemplate = { Height = 32 }
        };
        dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;

        SetupGridColumns();

        Controls.Add(dgvUsers);
        Controls.Add(pnlInput);
        Controls.Add(pnlHeader);

        Load += async (s, e) => await LoadUsersAsync();
    }

    private void SetupGridColumns()
    {
        dgvUsers.AutoGenerateColumns = false;
        dgvUsers.Columns.Clear();

        dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(User.UserID), HeaderText = "ID", Width = 50, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(User.Username), HeaderText = "Tên đăng nhập", Width = 120, DefaultCellStyle = { Font = new Font("Segoe UI", 9F, FontStyle.Bold) } });
        dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(User.FullName), HeaderText = "Họ và tên", Width = 160 });
        dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(User.Role), HeaderText = "Vai trò", Width = 90, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(User.IsActive), HeaderText = "Hoạt động", Width = 80 });
    }

    private async Task LoadUsersAsync()
    {
        var users = await UserDAL.GetAllUsersAsync();
        dgvUsers.DataSource = users;
        ResetForm();
    }

    private void DgvUsers_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvUsers.CurrentRow?.DataBoundItem is User u)
        {
            _selectedUserId = u.UserID;
            txtUsername.Text = u.Username;
            txtUsername.Enabled = false; // Không sửa username
            txtPassword.Text = u.Password;
            txtFullName.Text = u.FullName;
            cboRole.SelectedItem = u.Role;
            chkActive.Checked = u.IsActive;

            btnAdd.Enabled = false;
            btnUpdate.Enabled = true;
            btnDelete.Enabled = _selectedUserId != UserSession.CurrentUser?.UserID; // Không tự xóa chính mình
        }
    }

    private void ResetForm()
    {
        _selectedUserId = 0;
        txtUsername.Clear();
        txtUsername.Enabled = true;
        txtPassword.Clear();
        txtFullName.Clear();
        cboRole.SelectedIndex = 0;
        chkActive.Checked = true;

        btnAdd.Enabled = true;
        btnUpdate.Enabled = false;
        btnDelete.Enabled = false;
    }

    private async Task AddUserAsync()
    {
        if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text) || string.IsNullOrWhiteSpace(txtFullName.Text))
        {
            MessageBox.Show("Vui lòng nhập đầy đủ thông tin tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var u = new User
        {
            Username = txtUsername.Text.Trim(),
            Password = txtPassword.Text.Trim(),
            FullName = txtFullName.Text.Trim(),
            Role = cboRole.SelectedItem?.ToString() ?? "Cashier",
            IsActive = chkActive.Checked
        };

        bool ok = await UserDAL.AddUserAsync(u);
        if (ok)
        {
            MessageBox.Show("Thêm tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadUsersAsync();
        }
    }

    private async Task UpdateUserAsync()
    {
        if (_selectedUserId <= 0) return;

        var u = new User
        {
            UserID = _selectedUserId,
            Username = txtUsername.Text.Trim(),
            Password = txtPassword.Text.Trim(),
            FullName = txtFullName.Text.Trim(),
            Role = cboRole.SelectedItem?.ToString() ?? "Cashier",
            IsActive = chkActive.Checked
        };

        bool ok = await UserDAL.UpdateUserAsync(u);
        if (ok)
        {
            MessageBox.Show("Cập nhật thông tin tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadUsersAsync();
        }
    }

    private async Task DeleteUserAsync()
    {
        if (_selectedUserId <= 0) return;

        var confirm = MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa tài khoản '{txtUsername.Text}'?",
            "Xác nhận xóa",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            bool ok = await UserDAL.DeleteUserAsync(_selectedUserId);
            if (ok)
            {
                MessageBox.Show("Đã xóa tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadUsersAsync();
            }
        }
    }
}
