using System.Globalization;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Forms;

/// <summary>
/// Hộp thoại chọn tùy chọn món: Cỡ ly (Size), Mức đường, Mức đá, Topping và Ghi chú
/// </summary>
public class frmProductModifier : Form
{
    private readonly Product _product;
    private readonly CultureInfo _cultureVi = new("vi-VN");

    // Controls
    private RadioButton rdoSizeS = null!;
    private RadioButton rdoSizeM = null!;
    private RadioButton rdoSizeL = null!;
    private RadioButton rdoSizeXL = null!;

    private RadioButton rdoSugar100 = null!;
    private RadioButton rdoSugar70 = null!;
    private RadioButton rdoSugar50 = null!;
    private RadioButton rdoSugar30 = null!;
    private RadioButton rdoSugar0 = null!;

    private RadioButton rdoIce100 = null!;
    private RadioButton rdoIce70 = null!;
    private RadioButton rdoIce50 = null!;
    private RadioButton rdoIce0 = null!;
    private RadioButton rdoIceHot = null!;

    private CheckBox chkToppingPearl = null!;
    private CheckBox chkToppingWhitePearl = null!;
    private CheckBox chkToppingCheeseJelly = null!;
    private CheckBox chkToppingPudding = null!;
    private CheckBox chkToppingCreamCheese = null!;
    private CheckBox chkToppingPeach = null!;

    private TextBox txtCustomNotes = null!;
    private Label lblQuantity = null!;
    private Label lblCalculatedPrice = null!;
    private Label lblCalculatedTotal = null!;
    private Button btnAdd = null!;
    private Button btnCancel = null!;

    private int _quantity = 1;
    private decimal _unitPrice = 0m;

    public CartItem? ResultCartItem { get; private set; }

    public frmProductModifier(Product product, CartItem? existingItem = null)
    {
        _product = product;
        InitializeUI();

        if (existingItem != null)
        {
            _quantity = existingItem.Quantity;
            lblQuantity.Text = _quantity.ToString();
        }

        Recalculate();
    }

    private void InitializeUI()
    {
        Text = $"Tùy chọn: {_product.ProductName}";
        Size = new Size(580, 680);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        // Header Panel
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = Color.FromArgb(13, 110, 253),
            Padding = new Padding(16, 10, 16, 10)
        };
        var lblProductName = new Label
        {
            Text = _product.ProductName.ToUpper(),
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Top,
            Height = 24
        };
        var lblBasePrice = new Label
        {
            Text = $"Giá cơ bản: {_product.Price.ToString("#,##0 ₫", _cultureVi)}",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(220, 235, 255),
            Dock = DockStyle.Fill
        };
        pnlHeader.Controls.Add(lblBasePrice);
        pnlHeader.Controls.Add(lblProductName);

        // Content Panel (Scrollable)
        var pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(16, 10, 16, 10)
        };

        int curY = 10;

        // 1. Group Size
        var grpSize = new GroupBox
        {
            Text = "1. CHỌN CỠ LY (SIZE)",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            Location = new Point(16, curY),
            Size = new Size(520, 65)
        };
        rdoSizeS = new RadioButton { Text = "Size S (+0đ)", Location = new Point(15, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        rdoSizeM = new RadioButton { Text = "Size M (Chuẩn)", Location = new Point(135, 25), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        rdoSizeL = new RadioButton { Text = "Size L (+5.000đ)", Location = new Point(270, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        rdoSizeXL = new RadioButton { Text = "Size XL (+10.000đ)", Location = new Point(395, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };

        rdoSizeS.CheckedChanged += (s, e) => Recalculate();
        rdoSizeM.CheckedChanged += (s, e) => Recalculate();
        rdoSizeL.CheckedChanged += (s, e) => Recalculate();
        rdoSizeXL.CheckedChanged += (s, e) => Recalculate();

        grpSize.Controls.AddRange(new Control[] { rdoSizeS, rdoSizeM, rdoSizeL, rdoSizeXL });
        pnlContent.Controls.Add(grpSize);
        curY += 75;

        // 2. Group Sugar
        var grpSugar = new GroupBox
        {
            Text = "2. MỨC ĐƯỜNG",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            Location = new Point(16, curY),
            Size = new Size(520, 65)
        };
        rdoSugar100 = new RadioButton { Text = "100% (Chuẩn)", Location = new Point(15, 25), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9F) };
        rdoSugar70 = new RadioButton { Text = "70%", Location = new Point(135, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        rdoSugar50 = new RadioButton { Text = "50%", Location = new Point(220, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        rdoSugar30 = new RadioButton { Text = "30%", Location = new Point(305, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        rdoSugar0 = new RadioButton { Text = "Không đường (0%)", Location = new Point(385, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };

        grpSugar.Controls.AddRange(new Control[] { rdoSugar100, rdoSugar70, rdoSugar50, rdoSugar30, rdoSugar0 });
        pnlContent.Controls.Add(grpSugar);
        curY += 75;

        // 3. Group Ice
        var grpIce = new GroupBox
        {
            Text = "3. MỨC ĐÁ",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            Location = new Point(16, curY),
            Size = new Size(520, 65)
        };
        rdoIce100 = new RadioButton { Text = "100% (Chuẩn)", Location = new Point(15, 25), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9F) };
        rdoIce70 = new RadioButton { Text = "70%", Location = new Point(135, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        rdoIce50 = new RadioButton { Text = "50%", Location = new Point(220, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        rdoIce0 = new RadioButton { Text = "Không đá", Location = new Point(305, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        rdoIceHot = new RadioButton { Text = "Uống nóng", Location = new Point(410, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };

        grpIce.Controls.AddRange(new Control[] { rdoIce100, rdoIce70, rdoIce50, rdoIce0, rdoIceHot });
        pnlContent.Controls.Add(grpIce);
        curY += 75;

        // 4. Group Topping (Multiple Choice)
        var grpTopping = new GroupBox
        {
            Text = "4. THÊM TOPPING (CHỌN NHIỀU)",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            Location = new Point(16, curY),
            Size = new Size(520, 95)
        };

        chkToppingPearl = new CheckBox { Text = "Trân châu đen (+5k)", Location = new Point(15, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        chkToppingWhitePearl = new CheckBox { Text = "Trân châu trắng (+5k)", Location = new Point(185, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        chkToppingCheeseJelly = new CheckBox { Text = "Thạch phô mai (+10k)", Location = new Point(355, 25), AutoSize = true, Font = new Font("Segoe UI", 9F) };

        chkToppingPudding = new CheckBox { Text = "Pudding trứng (+8k)", Location = new Point(15, 55), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        chkToppingCreamCheese = new CheckBox { Text = "Kem Cheese (+10k)", Location = new Point(185, 55), AutoSize = true, Font = new Font("Segoe UI", 9F) };
        chkToppingPeach = new CheckBox { Text = "Đào miếng (+8k)", Location = new Point(355, 55), AutoSize = true, Font = new Font("Segoe UI", 9F) };

        chkToppingPearl.CheckedChanged += (s, e) => Recalculate();
        chkToppingWhitePearl.CheckedChanged += (s, e) => Recalculate();
        chkToppingCheeseJelly.CheckedChanged += (s, e) => Recalculate();
        chkToppingPudding.CheckedChanged += (s, e) => Recalculate();
        chkToppingCreamCheese.CheckedChanged += (s, e) => Recalculate();
        chkToppingPeach.CheckedChanged += (s, e) => Recalculate();

        grpTopping.Controls.AddRange(new Control[]
        {
            chkToppingPearl, chkToppingWhitePearl, chkToppingCheeseJelly,
            chkToppingPudding, chkToppingCreamCheese, chkToppingPeach
        });
        pnlContent.Controls.Add(grpTopping);
        curY += 105;

        // 5. Notes & Quantity
        var lblNoteTitle = new Label { Text = "5. GHI CHÚ TÙY CHỌN (VD: Ít ngọt, mang về,...):", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(16, curY), AutoSize = true };
        curY += 22;
        txtCustomNotes = new TextBox { Location = new Point(16, curY), Width = 520, Font = new Font("Segoe UI", 9.5F) };
        curY += 36;

        // Quantity selector
        var lblQtyTitle = new Label { Text = "SỐ LƯỢNG:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(16, curY + 6), AutoSize = true };
        var btnMinus = new Button
        {
            Text = "-",
            Location = new Point(100, curY),
            Size = new Size(35, 30),
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            BackColor = Color.FromArgb(240, 242, 245),
            FlatStyle = FlatStyle.Flat
        };
        btnMinus.FlatAppearance.BorderSize = 0;
        btnMinus.Click += (s, e) =>
        {
            if (_quantity > 1)
            {
                _quantity--;
                lblQuantity.Text = _quantity.ToString();
                Recalculate();
            }
        };

        lblQuantity = new Label
        {
            Text = "1",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            Location = new Point(140, curY),
            Size = new Size(40, 30),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var btnPlus = new Button
        {
            Text = "+",
            Location = new Point(185, curY),
            Size = new Size(35, 30),
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            BackColor = Color.FromArgb(240, 242, 245),
            FlatStyle = FlatStyle.Flat
        };
        btnPlus.FlatAppearance.BorderSize = 0;
        btnPlus.Click += (s, e) =>
        {
            _quantity++;
            lblQuantity.Text = _quantity.ToString();
            Recalculate();
        };

        pnlContent.Controls.AddRange(new Control[] { lblNoteTitle, txtCustomNotes, lblQtyTitle, btnMinus, lblQuantity, btnPlus });

        // Bottom Action Panel
        var pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 85,
            BackColor = Color.FromArgb(248, 249, 250),
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 10, 16, 10)
        };

        var lblUnitTitle = new Label { Text = "Đơn giá:", Location = new Point(16, 12), AutoSize = true, ForeColor = Color.FromArgb(108, 117, 125) };
        lblCalculatedPrice = new Label
        {
            Text = "0 ₫",
            Location = new Point(16, 28),
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            AutoSize = true
        };

        var lblTotalTitle = new Label { Text = "Tổng tiền món:", Location = new Point(140, 12), AutoSize = true, ForeColor = Color.FromArgb(108, 117, 125) };
        lblCalculatedTotal = new Label
        {
            Text = "0 ₫",
            Location = new Point(140, 28),
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.FromArgb(220, 53, 69),
            AutoSize = true
        };

        btnAdd = new Button
        {
            Text = "THÊM VÀO GIỎ HÀNG",
            Location = new Point(310, 15),
            Size = new Size(160, 48),
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.Click += BtnAdd_Click;

        btnCancel = new Button
        {
            Text = "Hủy",
            Location = new Point(480, 15),
            Size = new Size(70, 48),
            BackColor = Color.FromArgb(230, 235, 240),
            ForeColor = Color.FromArgb(33, 37, 41),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.Cancel
        };
        btnCancel.FlatAppearance.BorderSize = 0;

        pnlBottom.Controls.AddRange(new Control[]
        {
            lblUnitTitle, lblCalculatedPrice,
            lblTotalTitle, lblCalculatedTotal,
            btnAdd, btnCancel
        });

        Controls.Add(pnlContent);
        Controls.Add(pnlBottom);
        Controls.Add(pnlHeader);

        AcceptButton = btnAdd;
        CancelButton = btnCancel;
    }

    private void Recalculate()
    {
        decimal basePrice = _product.Price;
        decimal extraPrice = 0m;

        // Size
        if (rdoSizeL.Checked) extraPrice += 5000m;
        else if (rdoSizeXL.Checked) extraPrice += 10000m;

        // Toppings
        if (chkToppingPearl.Checked) extraPrice += 5000m;
        if (chkToppingWhitePearl.Checked) extraPrice += 5000m;
        if (chkToppingCheeseJelly.Checked) extraPrice += 10000m;
        if (chkToppingPudding.Checked) extraPrice += 8000m;
        if (chkToppingCreamCheese.Checked) extraPrice += 10000m;
        if (chkToppingPeach.Checked) extraPrice += 8000m;

        _unitPrice = basePrice + extraPrice;
        decimal total = _unitPrice * _quantity;

        lblCalculatedPrice.Text = _unitPrice.ToString("#,##0 ₫", _cultureVi);
        lblCalculatedTotal.Text = total.ToString("#,##0 ₫", _cultureVi);
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        // Thu thập thông tin
        string size = rdoSizeS.Checked ? "Size S" :
                      rdoSizeL.Checked ? "Size L (+5k)" :
                      rdoSizeXL.Checked ? "Size XL (+10k)" : "Size M";

        string sugar = rdoSugar70.Checked ? "70% đường" :
                       rdoSugar50.Checked ? "50% đường" :
                       rdoSugar30.Checked ? "30% đường" :
                       rdoSugar0.Checked ? "Không đường" : "100% đường";

        string ice = rdoIce70.Checked ? "70% đá" :
                     rdoIce50.Checked ? "50% đá" :
                     rdoIce0.Checked ? "Không đá" :
                     rdoIceHot.Checked ? "Uống nóng" : "100% đá";

        var toppingList = new List<string>();
        if (chkToppingPearl.Checked) toppingList.Add("Trân châu đen (+5k)");
        if (chkToppingWhitePearl.Checked) toppingList.Add("Trân châu trắng (+5k)");
        if (chkToppingCheeseJelly.Checked) toppingList.Add("Thạch phô mai (+10k)");
        if (chkToppingPudding.Checked) toppingList.Add("Pudding (+8k)");
        if (chkToppingCreamCheese.Checked) toppingList.Add("Kem Cheese (+10k)");
        if (chkToppingPeach.Checked) toppingList.Add("Đào miếng (+8k)");

        string toppingsStr = string.Join(", ", toppingList);

        // Tổng hợp notes hiển thị
        var noteParts = new List<string>();
        if (size != "Size M") noteParts.Add(size);
        if (sugar != "100% đường") noteParts.Add(sugar);
        if (ice != "100% đá") noteParts.Add(ice);
        if (!string.IsNullOrEmpty(toppingsStr)) noteParts.Add(toppingsStr);

        string customNote = txtCustomNotes.Text.Trim();
        if (!string.IsNullOrEmpty(customNote))
        {
            noteParts.Add(customNote);
        }

        string fullNotes = string.Join(" | ", noteParts);

        ResultCartItem = new CartItem
        {
            ProductId = _product.ProductID,
            ProductName = _product.ProductName,
            BasePrice = _product.Price,
            UnitPrice = _unitPrice,
            Quantity = _quantity,
            Size = size,
            Sugar = sugar,
            Ice = ice,
            Toppings = toppingsStr,
            Notes = fullNotes
        };

        DialogResult = DialogResult.OK;
        Close();
    }
}
