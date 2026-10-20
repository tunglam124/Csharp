using System.Globalization;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Hộp thoại nhập giảm giá (Số tiền hoặc Phần trăm)
/// </summary>
public class frmDiscountInput : Form
{
    private readonly decimal _maxApplicableAmount;
    private readonly CultureInfo _cultureVi = new("vi-VN");

    private TextBox _txtValue = null!;
    private RadioButton _rdoAmount = null!;
    private RadioButton _rdoPercent = null!;
    private Label _lblMaxAmount = null!;
    private Button _btnApply = null!;
    private Button _btnCancel = null!;

    /// <summary>
    /// Số tiền giảm giá thực tế sau khi tính toán và validate hợp lệ
    /// </summary>
    public decimal CalculatedDiscount { get; private set; }

    public frmDiscountInput(decimal maxApplicableAmount)
    {
        _maxApplicableAmount = maxApplicableAmount;
        InitializeUI();
    }

    private void InitializeUI()
    {
        Text = "Áp dụng giảm giá";
        Size = new Size(380, 260);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        var lblInstruction = new Label
        {
            Text = "Chọn hình thức giảm giá và nhập giá trị:",
            Location = new Point(20, 16),
            Size = new Size(320, 20),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

        _rdoAmount = new RadioButton
        {
            Text = "Số tiền (VNĐ)",
            Location = new Point(24, 46),
            Size = new Size(130, 24),
            Checked = true
        };

        _rdoPercent = new RadioButton
        {
            Text = "Phần trăm (%)",
            Location = new Point(170, 46),
            Size = new Size(130, 24)
        };

        _txtValue = new TextBox
        {
            Location = new Point(24, 82),
            Size = new Size(316, 27),
            Font = new Font("Segoe UI", 11F, FontStyle.Bold)
        };
        _txtValue.KeyPress += TxtValue_KeyPress;

        _lblMaxAmount = new Label
        {
            Text = $"Tối đa có thể giảm: {_maxApplicableAmount.ToString("#,##0 ₫", _cultureVi)}",
            Location = new Point(24, 118),
            Size = new Size(316, 20),
            ForeColor = Color.FromArgb(108, 117, 125)
        };

        _btnApply = new Button
        {
            Text = "Áp dụng",
            Location = new Point(136, 160),
            Size = new Size(100, 36),
            BackColor = Color.FromArgb(13, 110, 253),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        _btnApply.FlatAppearance.BorderSize = 0;
        _btnApply.Click += BtnApply_Click;

        _btnCancel = new Button
        {
            Text = "Hủy bỏ",
            Location = new Point(244, 160),
            Size = new Size(96, 36),
            BackColor = Color.FromArgb(240, 242, 245),
            ForeColor = Color.FromArgb(33, 37, 41),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.Cancel
        };
        _btnCancel.FlatAppearance.BorderSize = 0;

        Controls.Add(lblInstruction);
        Controls.Add(_rdoAmount);
        Controls.Add(_rdoPercent);
        Controls.Add(_txtValue);
        Controls.Add(_lblMaxAmount);
        Controls.Add(_btnApply);
        Controls.Add(_btnCancel);

        AcceptButton = _btnApply;
        CancelButton = _btnCancel;
    }

    private void TxtValue_KeyPress(object? sender, KeyPressEventArgs e)
    {
        // Chỉ cho phép nhập số, dấu chấm/phẩy và phím xóa Backspace
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
        {
            e.Handled = true;
        }
    }

    private void BtnApply_Click(object? sender, EventArgs e)
    {
        string rawInput = _txtValue.Text.Trim().Replace(",", ".");
        if (!decimal.TryParse(rawInput, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
        {
            MessageBox.Show("Vui lòng nhập một số hợp lệ.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtValue.Focus();
            return;
        }

        if (val < 0)
        {
            MessageBox.Show("Giá trị giảm giá không được âm.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtValue.Focus();
            return;
        }

        if (_rdoPercent.Checked)
        {
            if (val > 100)
            {
                MessageBox.Show("Phần trăm giảm giá không được vượt quá 100%.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtValue.Focus();
                return;
            }

            // Tính tiền giảm theo tỷ lệ phần trăm
            CalculatedDiscount = Math.Round(_maxApplicableAmount * (val / 100m), 0);
        }
        else
        {
            if (val > _maxApplicableAmount)
            {
                MessageBox.Show($"Số tiền giảm không được vượt quá tổng tiền ({_maxApplicableAmount.ToString("#,##0 ₫", _cultureVi)}).", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtValue.Focus();
                return;
            }

            CalculatedDiscount = Math.Round(val, 0);
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
