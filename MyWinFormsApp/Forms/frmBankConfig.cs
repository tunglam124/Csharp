using MyWinFormsApp.Models;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Hộp thoại cấu hình tài khoản ngân hàng để sinh mã VietQR
/// </summary>
public class frmBankConfig : Form
{
    private ComboBox cboBank = null!;
    private TextBox txtAccountNo = null!;
    private TextBox txtAccountName = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;

    public frmBankConfig()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        Text = "Cài đặt tài khoản ngân hàng (VietQR)";
        Size = new Size(450, 320);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 48,
            BackColor = Color.FromArgb(13, 110, 253),
            Padding = new Padding(16, 0, 0, 0)
        };
        var lblTitle = new Label
        {
            Text = "CẤU HÌNH TÀI KHOẢN NHẬN TIỀN VIETQR",
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlHeader.Controls.Add(lblTitle);

        int y = 65;
        var lblBank = new Label { Text = "Ngân hàng thụ hưởng:", Location = new Point(24, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        cboBank = new ComboBox { Location = new Point(24, y), Width = 385, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
        
        // Nạp danh sách các ngân hàng phổ biến hỗ trợ VietQR
        cboBank.Items.Add(new BankItem("MB", "MBBank - Ngân hàng TMCP Quân Đội"));
        cboBank.Items.Add(new BankItem("VCB", "Vietcombank - Ngân hàng Ngoại Thương"));
        cboBank.Items.Add(new BankItem("ICB", "VietinBank - Ngân hàng Công Thương"));
        cboBank.Items.Add(new BankItem("BIDV", "BIDV - Ngân hàng Đầu Tư và Phát Triển"));
        cboBank.Items.Add(new BankItem("TCB", "Techcombank - Ngân hàng Kỹ Thương"));
        cboBank.Items.Add(new BankItem("ACB", "ACB - Ngân hàng Á Châu"));
        cboBank.Items.Add(new BankItem("TPB", "TPBank - Ngân hàng Tiên Phong"));
        cboBank.Items.Add(new BankItem("VPB", "VPBank - Ngân hàng Việt Nam Thịnh Vượng"));
        cboBank.Items.Add(new BankItem("STB", "Sacombank - Ngân hàng Sài Gòn Thương Tín"));
        cboBank.Items.Add(new BankItem("VIB", "VIB - Ngân hàng Quốc Tế"));

        y += 35;
        var lblAccNo = new Label { Text = "Số tài khoản nhận tiền:", Location = new Point(24, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        txtAccountNo = new TextBox { Location = new Point(24, y), Width = 385, Font = new Font("Segoe UI", 9.5F) };

        y += 35;
        var lblAccName = new Label { Text = "Tên chủ tài khoản (Không dấu, in hoa):", Location = new Point(24, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        txtAccountName = new TextBox { Location = new Point(24, y), Width = 385, Font = new Font("Segoe UI", 9.5F) };

        // Buttons
        btnSave = new Button
        {
            Text = "Lưu cấu hình",
            Location = new Point(190, 230),
            Size = new Size(110, 36),
            BackColor = Color.FromArgb(13, 110, 253),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Click += BtnSave_Click;

        btnCancel = new Button
        {
            Text = "Đóng",
            Location = new Point(310, 230),
            Size = new Size(100, 36),
            BackColor = Color.FromArgb(240, 242, 245),
            ForeColor = Color.FromArgb(33, 37, 41),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.Cancel
        };
        btnCancel.FlatAppearance.BorderSize = 0;

        Controls.AddRange(new Control[]
        {
            pnlHeader, lblBank, cboBank,
            lblAccNo, txtAccountNo,
            lblAccName, txtAccountName,
            btnSave, btnCancel
        });

        AcceptButton = btnSave;
        CancelButton = btnCancel;

        // Load current config
        var current = BankConfig.Current;
        txtAccountNo.Text = current.AccountNumber;
        txtAccountName.Text = current.AccountName;

        for (int i = 0; i < cboBank.Items.Count; i++)
        {
            if (cboBank.Items[i] is BankItem item && item.BankId == current.BankId)
            {
                cboBank.SelectedIndex = i;
                break;
            }
        }
        if (cboBank.SelectedIndex < 0 && cboBank.Items.Count > 0)
            cboBank.SelectedIndex = 0;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtAccountNo.Text))
        {
            MessageBox.Show("Vui lòng nhập số tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtAccountNo.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtAccountName.Text))
        {
            MessageBox.Show("Vui lòng nhập tên chủ tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtAccountName.Focus();
            return;
        }

        var bankItem = cboBank.SelectedItem as BankItem;
        var config = BankConfig.Current;
        config.BankId = bankItem?.BankId ?? "MB";
        config.BankName = bankItem?.BankName ?? "MBBank";
        config.AccountNumber = txtAccountNo.Text.Trim();
        config.AccountName = txtAccountName.Text.Trim().ToUpper();
        config.Save();

        MessageBox.Show("Đã lưu cấu hình tài khoản VietQR thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }

    private class BankItem
    {
        public string BankId { get; }
        public string BankName { get; }

        public BankItem(string id, string name)
        {
            BankId = id;
            BankName = name;
        }

        public override string ToString() => BankName;
    }
}
