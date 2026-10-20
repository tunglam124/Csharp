using MyWinFormsApp.DataAccess;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Hộp thoại Chuyển bàn: Chuyển đơn hàng từ bàn đang có khách sang một bàn trống
/// </summary>
public class frmMoveTable : Form
{
    private ComboBox cboSourceTable = null!;
    private ComboBox cboTargetTable = null!;
    private Label lblOrderInfo = null!;
    private Button btnConfirm = null!;
    private Button btnCancel = null!;

    private readonly List<PendingOrderModel> _pendingOrders;
    private readonly List<string> _allTableNames;

    public frmMoveTable(List<PendingOrderModel> pendingOrders, List<string> allTableNames, string? preselectedSourceTable = null)
    {
        _pendingOrders = pendingOrders;
        _allTableNames = allTableNames;

        InitializeUI(preselectedSourceTable);
    }

    private void InitializeUI(string? preselectedSourceTable)
    {
        Text = "Chuyển bàn";
        Size = new Size(460, 320);
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
            BackColor = Color.FromArgb(13, 110, 253),
            Padding = new Padding(16, 0, 0, 0)
        };
        var lblTitle = new Label
        {
            Text = "CHUYỂN ĐƠN HÀNG SANG BÀN MỚI",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlHeader.Controls.Add(lblTitle);

        int y = 65;
        var lblSource = new Label { Text = "Bàn nguồn (Đang có khách):", Location = new Point(24, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        cboSourceTable = new ComboBox { Location = new Point(24, y), Width = 395, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };

        foreach (var order in _pendingOrders)
        {
            cboSourceTable.Items.Add(new ComboBoxItem(order.OrderID, $"{order.TableNumber} (Đơn #{order.OrderID} - {order.TotalAmount:N0} ₫)"));
        }

        y += 35;
        lblOrderInfo = new Label
        {
            Text = "Chọn bàn cần chuyển...",
            Location = new Point(24, y),
            AutoSize = true,
            ForeColor = Color.FromArgb(108, 117, 125)
        };

        y += 25;
        var lblTarget = new Label { Text = "Chuyển đến bàn trống:", Location = new Point(24, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        y += 22;
        cboTargetTable = new ComboBox { Location = new Point(24, y), Width = 395, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };

        // Lọc các bàn đang trống
        var occupiedTables = _pendingOrders.Select(x => x.TableNumber).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var tbl in _allTableNames)
        {
            if (!occupiedTables.Contains(tbl))
            {
                cboTargetTable.Items.Add(tbl);
            }
        }

        y += 45;
        btnConfirm = new Button
        {
            Text = "Xác nhận chuyển",
            Location = new Point(190, y),
            Size = new Size(130, 36),
            BackColor = Color.FromArgb(13, 110, 253),
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
            Location = new Point(330, y),
            Size = new Size(90, 36),
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
            lblOrderInfo, lblTarget, cboTargetTable,
            btnConfirm, btnCancel
        });

        // Preselect
        if (!string.IsNullOrEmpty(preselectedSourceTable))
        {
            for (int i = 0; i < cboSourceTable.Items.Count; i++)
            {
                if (cboSourceTable.Items[i] is ComboBoxItem item && item.Text.StartsWith(preselectedSourceTable))
                {
                    cboSourceTable.SelectedIndex = i;
                    break;
                }
            }
        }
        if (cboSourceTable.SelectedIndex < 0 && cboSourceTable.Items.Count > 0)
            cboSourceTable.SelectedIndex = 0;

        if (cboTargetTable.Items.Count > 0)
            cboTargetTable.SelectedIndex = 0;

        cboSourceTable.SelectedIndexChanged += (s, e) =>
        {
            if (cboSourceTable.SelectedItem is ComboBoxItem item)
            {
                var o = _pendingOrders.FirstOrDefault(x => x.OrderID == item.Value);
                if (o != null)
                {
                    lblOrderInfo.Text = $"Đơn lúc: {o.OrderDate:HH:mm} | {o.ItemCount} món | Tổng: {o.TotalAmount:N0} ₫";
                }
            }
        };
    }

    private async void BtnConfirm_Click(object? sender, EventArgs e)
    {
        if (cboSourceTable.SelectedItem is not ComboBoxItem sourceItem)
        {
            MessageBox.Show("Vui lòng chọn bàn nguồn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string? targetTable = cboTargetTable.SelectedItem as string;
        if (string.IsNullOrEmpty(targetTable))
        {
            MessageBox.Show("Vui lòng chọn bàn trống đích đến!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        bool success = await OrderRepository.MoveTableAsync(sourceItem.Value, targetTable);
        if (success)
        {
            MessageBox.Show($"Đã chuyển đơn hàng sang {targetTable} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
