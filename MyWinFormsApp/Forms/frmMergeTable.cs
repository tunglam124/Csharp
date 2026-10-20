using MyWinFormsApp.DataAccess;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Hộp thoại Gộp bàn: Gộp 2 bàn đang có đơn Pending lại làm một
/// </summary>
public class frmMergeTable : Form
{
    private ComboBox cboSourceTable = null!;
    private ComboBox cboTargetTable = null!;
    private Label lblSummary = null!;
    private Button btnConfirm = null!;
    private Button btnCancel = null!;

    private readonly List<PendingOrderModel> _pendingOrders;

    public frmMergeTable(List<PendingOrderModel> pendingOrders, string? preselectedTable = null)
    {
        _pendingOrders = pendingOrders;
        InitializeUI(preselectedTable);
    }

    private void InitializeUI(string? preselectedTable)
    {
        Text = "Gộp bàn";
        Size = new Size(480, 340);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(253, 126, 20),
            Padding = new Padding(16, 0, 0, 0)
        };
        var lblTitle = new Label
        {
            Text = "GỘP ĐƠN HÀNG GIỮA 2 BÀN",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlHeader.Controls.Add(lblTitle);

        int y = 65;
        var lblSource = new Label { Text = "Bàn cần gộp (Bàn A - sẽ bị xóa đơn):", Location = new Point(24, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        cboSourceTable = new ComboBox { Location = new Point(24, y), Width = 415, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };

        y += 35;
        var lblTarget = new Label { Text = "Gộp vào bàn (Bàn B - nhận toàn bộ món):", Location = new Point(24, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        cboTargetTable = new ComboBox { Location = new Point(24, y), Width = 415, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };

        foreach (var order in _pendingOrders)
        {
            cboSourceTable.Items.Add(new ComboBoxItem(order.OrderID, $"{order.TableNumber} (Đơn #{order.OrderID} - {order.TotalAmount:N0} ₫)"));
            cboTargetTable.Items.Add(new ComboBoxItem(order.OrderID, $"{order.TableNumber} (Đơn #{order.OrderID} - {order.TotalAmount:N0} ₫)"));
        }

        y += 35;
        lblSummary = new Label
        {
            Text = "Chọn 2 bàn khác nhau để gộp...",
            Location = new Point(24, y),
            AutoSize = true,
            ForeColor = Color.FromArgb(13, 110, 253),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

        y += 40;
        btnConfirm = new Button
        {
            Text = "Xác nhận gộp bàn",
            Location = new Point(210, y),
            Size = new Size(135, 36),
            BackColor = Color.FromArgb(253, 126, 20),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnConfirm.FlatAppearance.BorderSize = 0;
        btnConfirm.Click += BtnConfirm_Click;

        btnCancel = new Button
        {
            Text = "Hủy bỏ",
            Location = new Point(355, y),
            Size = new Size(85, 36),
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
            pnlHeader, lblSource, cboSourceTable,
            lblTarget, cboTargetTable,
            lblSummary, btnConfirm, btnCancel
        });

        if (cboSourceTable.Items.Count > 0) cboSourceTable.SelectedIndex = 0;
        if (cboTargetTable.Items.Count > 1) cboTargetTable.SelectedIndex = 1;

        cboSourceTable.SelectedIndexChanged += (s, e) => UpdateSummary();
        cboTargetTable.SelectedIndexChanged += (s, e) => UpdateSummary();

        UpdateSummary();
    }

    private void UpdateSummary()
    {
        if (cboSourceTable.SelectedItem is ComboBoxItem itemA && cboTargetTable.SelectedItem is ComboBoxItem itemB)
        {
            if (itemA.Value == itemB.Value)
            {
                lblSummary.Text = "⚠️ Bàn nguồn và bàn đích không được trùng nhau!";
                lblSummary.ForeColor = Color.FromArgb(220, 53, 69);
                btnConfirm.Enabled = false;
                return;
            }

            var oA = _pendingOrders.FirstOrDefault(x => x.OrderID == itemA.Value);
            var oB = _pendingOrders.FirstOrDefault(x => x.OrderID == itemB.Value);
            if (oA != null && oB != null)
            {
                decimal totalCombined = oA.TotalAmount + oB.TotalAmount;
                lblSummary.Text = $"Toàn bộ món bàn {oA.TableNumber} sẽ chuyển sang {oB.TableNumber}.\nƯớc tính tổng tiền mới: {totalCombined:N0} ₫";
                lblSummary.ForeColor = Color.FromArgb(25, 135, 84);
                btnConfirm.Enabled = true;
            }
        }
    }

    private async void BtnConfirm_Click(object? sender, EventArgs e)
    {
        if (cboSourceTable.SelectedItem is not ComboBoxItem itemA || cboTargetTable.SelectedItem is not ComboBoxItem itemB)
            return;

        if (itemA.Value == itemB.Value)
        {
            MessageBox.Show("Vui lòng chọn 2 bàn khác nhau!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            "Bạn có chắc chắn muốn gộp 2 bàn này? Toàn bộ món ăn sẽ được chuyển sang bàn đích.",
            "Xác nhận gộp bàn",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
            return;

        bool success = await OrderRepository.MergeOrdersAsync(itemA.Value, itemB.Value);
        if (success)
        {
            MessageBox.Show("Đã gộp 2 bàn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private class ComboBoxItem
    {
        public int Value { get; }
        public string Text { get; }

        public ComboBoxItem(int value, string text)
        {
            Value = value;
            Text = text;
        }

        public override string ToString() => Text;
    }
}
