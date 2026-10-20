using MyWinFormsApp.DataAccess;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Màn hình đăng nhập hệ thống POS & phân quyền Admin / Cashier
/// </summary>
public class frmLogin : Form
{
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private CheckBox chkShowPassword = null!;
    private Button btnLogin = null!;
    private Label lblError = null!;

    public frmLogin()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        Text = "Đăng nhập hệ thống - POS FASTFOOD & CAFE";
        Size = new Size(460, 480);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        // Header Panel
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 110,
            BackColor = Color.FromArgb(13, 110, 253),
            Padding = new Padding(24, 20, 24, 20)
        };

        var lblBrand = new Label
        {
            Text = "☕ POS FASTFOOD & CAFE",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Top,
            Height = 35,
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblSub = new Label
        {
            Text = "Vui lòng đăng nhập để tiếp tục làm việc",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(220, 235, 255),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        pnlHeader.Controls.Add(lblSub);
        pnlHeader.Controls.Add(lblBrand);

        // Content
        int y = 135;

        var lblUser = new Label { Text = "Tên đăng nhập:", Location = new Point(40, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(33, 37, 41) };
        y += 24;
        txtUsername = new TextBox { Location = new Point(40, y), Width = 360, Font = new Font("Segoe UI", 11F) };
        txtUsername.Text = "cashier"; // Mặc định thu ngân để đăng nhập nhanh

        y += 40;
        var lblPass = new Label { Text = "Mật khẩu:", Location = new Point(40, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(33, 37, 41) };
        y += 24;
        txtPassword = new TextBox { Location = new Point(40, y), Width = 360, Font = new Font("Segoe UI", 11F), UseSystemPasswordChar = true };
        txtPassword.Text = "123";

        y += 34;
        chkShowPassword = new CheckBox { Text = "Hiển thị mật khẩu", Location = new Point(40, y), AutoSize = true, Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(108, 117, 125) };
        chkShowPassword.CheckedChanged += (s, e) => txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

        y += 26;
        lblError = new Label
        {
            Text = "",
            Location = new Point(40, y),
            Size = new Size(360, 20),
            ForeColor = Color.FromArgb(220, 53, 69),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        y += 22;
        btnLogin = new Button
        {
            Text = "ĐĂNG NHẬP (ENTER)",
            Location = new Point(40, y),
            Size = new Size(360, 42),
            BackColor = Color.FromArgb(13, 110, 253),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Click += async (s, e) => await PerformLoginAsync();

        y += 50;
        // Demo quick login buttons
        var lblQuick = new Label { Text = "Đăng nhập nhanh (Test tài khoản):", Location = new Point(40, y), AutoSize = true, Font = new Font("Segoe UI", 8.5F, FontStyle.Italic), ForeColor = Color.FromArgb(108, 117, 125) };
        y += 20;

        var btnQuickAdmin = new Button
        {
            Text = "🔑 Quản trị (admin)",
            Location = new Point(40, y),
            Size = new Size(175, 32),
            BackColor = Color.FromArgb(231, 241, 255),
            ForeColor = Color.FromArgb(13, 110, 253),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnQuickAdmin.FlatAppearance.BorderSize = 0;
        btnQuickAdmin.Click += async (s, e) =>
        {
            txtUsername.Text = "admin";
            txtPassword.Text = "admin123";
            await PerformLoginAsync();
        };

        var btnQuickCashier = new Button
        {
            Text = "👤 Thu ngân (cashier)",
            Location = new Point(225, y),
            Size = new Size(175, 32),
            BackColor = Color.FromArgb(209, 231, 221),
            ForeColor = Color.FromArgb(15, 81, 50),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnQuickCashier.FlatAppearance.BorderSize = 0;
        btnQuickCashier.Click += async (s, e) =>
        {
            txtUsername.Text = "cashier";
            txtPassword.Text = "123";
            await PerformLoginAsync();
        };

        Controls.AddRange(new Control[]
        {
            pnlHeader, lblUser, txtUsername,
            lblPass, txtPassword, chkShowPassword,
            lblError, btnLogin,
            lblQuick, btnQuickAdmin, btnQuickCashier
        });

        AcceptButton = btnLogin;
    }

    private async Task PerformLoginAsync()
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            lblError.Text = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";
            return;
        }

        btnLogin.Enabled = false;
        btnLogin.Text = "Đang kiểm tra...";
        lblError.Text = "";

        try
        {
            var user = await UserDAL.AuthenticateAsync(username, password);
            if (user != null)
            {
                UserSession.CurrentUser = user;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                lblError.Text = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                txtPassword.Focus();
                txtPassword.SelectAll();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = $"Lỗi: {ex.Message}";
        }
        finally
        {
            btnLogin.Enabled = true;
            btnLogin.Text = "ĐĂNG NHẬP (ENTER)";
        }
    }
}
