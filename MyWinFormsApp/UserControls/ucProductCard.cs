using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace MyWinFormsApp.UserControls;

/// <summary>
/// UserControl đại diện cho thẻ sản phẩm trong FlowLayoutPanel của POS (120x150 px)
/// Khởi tạo hoàn toàn bằng code-behind
/// </summary>
public class ucProductCard : UserControl
{
    #region Fields & Controls

    private readonly PictureBox _picImage;
    private readonly Label _lblProductName;
    private readonly Label _lblPrice;
    private readonly Button _btnAdd;

    private int _id;
    private string _productName = string.Empty;
    private decimal _price;
    private Image? _image;

    private bool _isHovered;
    private readonly Color _colorNormalBg = Color.White;
    private readonly Color _colorHoverBg = Color.FromArgb(242, 247, 255);
    private readonly Color _colorNormalBorder = Color.FromArgb(220, 224, 230);
    private readonly Color _colorHoverBorder = Color.FromArgb(0, 123, 255);
    private const int CornerRadius = 8;

    #endregion

    #region Properties

    /// <summary>
    /// Mã sản phẩm
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Id
    {
        get => _id;
        set => _id = value;
    }

    /// <summary>
    /// Tên sản phẩm, cập nhật ngay lên UI khi gán
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new string ProductName
    {
        get => _productName;
        set
        {
            _productName = value;
            _lblProductName.Text = _productName;
        }
    }

    /// <summary>
    /// Đơn giá sản phẩm, cập nhật ngay lên UI định dạng tiền tệ vi-VN
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Price
    {
        get => _price;
        set
        {
            _price = value;
            _lblPrice.Text = _price.ToString("N0", new CultureInfo("vi-VN")) + " ₫";
        }
    }

    /// <summary>
    /// Hình ảnh sản phẩm, cập nhật ngay lên UI khi gán
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Image? Image
    {
        get => _image;
        set
        {
            _image = value;
            _picImage.Image = _image;
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Sự kiện kích hoạt khi click vào bất kỳ vị trí nào trên thẻ hoặc nút '+'
    /// </summary>
    public event EventHandler<ProductClickedEventArgs>? OnProductClicked;

    #endregion

    #region Constructor

    public ucProductCard()
    {
        // 1. Cấu hình UserControl
        Size = new Size(120, 150);
        Margin = new Padding(6);
        BackColor = _colorNormalBg;
        DoubleBuffered = true;
        Cursor = Cursors.Hand;

        // 2. PictureBox hiển thị hình ảnh ở trên
        _picImage = new PictureBox
        {
            Location = new Point(6, 6),
            Size = new Size(108, 65),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };

        // 3. Label tên món ở giữa (căn giữa, tự xuống dòng)
        _lblProductName = new Label
        {
            Location = new Point(4, 73),
            Size = new Size(112, 40),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 8.75F, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };

        // 4. Label đơn giá ở góc dưới bên trái
        _lblPrice = new Label
        {
            Location = new Point(6, 116),
            Size = new Size(72, 28),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 8.25F, FontStyle.Bold),
            ForeColor = Color.FromArgb(217, 83, 79),
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };

        // 5. Button '+' ở góc phải dưới
        _btnAdd = new Button
        {
            Location = new Point(82, 114),
            Size = new Size(30, 28),
            Text = "+",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };
        _btnAdd.FlatAppearance.BorderSize = 0;

        // 6. Thêm Controls vào UserControl
        Controls.Add(_picImage);
        Controls.Add(_lblProductName);
        Controls.Add(_lblPrice);
        Controls.Add(_btnAdd);

        // 7. Đăng ký sự kiện Click và Hover cho tất cả thành phần con
        RegisterInteractionEvents();

        // 8. Cắt bo góc nhẹ cho UserControl
        UpdateRoundedRegion();
    }

    #endregion

    #region Interaction & Hover Logic

    /// <summary>
    /// Gán sự kiện Click và Hover cho toàn bộ control con trên thẻ
    /// </summary>
    private void RegisterInteractionEvents()
    {
        Control[] interactiveControls = { this, _picImage, _lblProductName, _lblPrice, _btnAdd };

        foreach (var ctrl in interactiveControls)
        {
            ctrl.Click += (s, e) => TriggerProductClicked();
            ctrl.MouseEnter += (s, e) => SetHoverState(true);
            ctrl.MouseLeave += Control_MouseLeave;
        }
    }

    private void Control_MouseLeave(object? sender, EventArgs e)
    {
        // Chỉ bỏ hover khi con trỏ chuột ra khỏi hoàn toàn diện tích thẻ
        Point mousePos = PointToClient(Cursor.Position);
        if (!ClientRectangle.Contains(mousePos))
        {
            SetHoverState(false);
        }
    }

    private void SetHoverState(bool hovered)
    {
        if (_isHovered == hovered) return;
        _isHovered = hovered;
        BackColor = _isHovered ? _colorHoverBg : _colorNormalBg;
        Invalidate(); // Vẽ lại đường viền bo góc khi hover
    }

    private void TriggerProductClicked()
    {
        OnProductClicked?.Invoke(this, new ProductClickedEventArgs(_id, _productName, _price));
    }

    #endregion

    #region Drawing & Rounded Corners

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateRoundedRegion();
    }

    private void UpdateRoundedRegion()
    {
        if (Width <= 0 || Height <= 0) return;
        using var path = CreateRoundedRectanglePath(ClientRectangle, CornerRadius);
        Region = new Region(path);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Vẽ đường viền bo góc nhẹ (thay đổi màu viền khi hover)
        var borderRect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = CreateRoundedRectanglePath(borderRect, CornerRadius);
        using var pen = new Pen(_isHovered ? _colorHoverBorder : _colorNormalBorder, 1.5f);
        e.Graphics.DrawPath(pen, path);
    }

    private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        float diameter = radius * 2F;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }

    #endregion
}
