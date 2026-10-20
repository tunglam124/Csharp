using System.ComponentModel;

namespace MyWinFormsApp.Models;

/// <summary>
/// Đại diện cho một món hàng trong giỏ hàng của POS kèm Topping và Modifiers
/// </summary>
public class CartItem : INotifyPropertyChanged
{
    private int _productId;
    private string _productName = string.Empty;
    private int _quantity = 1;
    private decimal _unitPrice;
    private decimal _basePrice;
    private string _size = "Size M";
    private string _sugar = "100% đường";
    private string _ice = "100% đá";
    private string _toppings = string.Empty;
    private string _notes = string.Empty;

    public int ProductId
    {
        get => _productId;
        set
        {
            if (_productId != value)
            {
                _productId = value;
                OnPropertyChanged(nameof(ProductId));
            }
        }
    }

    public string ProductName
    {
        get => _productName;
        set
        {
            if (_productName != value)
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
                OnPropertyChanged(nameof(DisplayNameWithNotes));
            }
        }
    }

    public decimal BasePrice
    {
        get => _basePrice;
        set
        {
            if (_basePrice != value)
            {
                _basePrice = value;
                OnPropertyChanged(nameof(BasePrice));
            }
        }
    }

    public string Size
    {
        get => _size;
        set
        {
            if (_size != value)
            {
                _size = value;
                OnPropertyChanged(nameof(Size));
                OnPropertyChanged(nameof(DisplayNameWithNotes));
            }
        }
    }

    public string Sugar
    {
        get => _sugar;
        set
        {
            if (_sugar != value)
            {
                _sugar = value;
                OnPropertyChanged(nameof(Sugar));
                OnPropertyChanged(nameof(DisplayNameWithNotes));
            }
        }
    }

    public string Ice
    {
        get => _ice;
        set
        {
            if (_ice != value)
            {
                _ice = value;
                OnPropertyChanged(nameof(Ice));
                OnPropertyChanged(nameof(DisplayNameWithNotes));
            }
        }
    }

    public string Toppings
    {
        get => _toppings;
        set
        {
            if (_toppings != value)
            {
                _toppings = value;
                OnPropertyChanged(nameof(Toppings));
                OnPropertyChanged(nameof(DisplayNameWithNotes));
            }
        }
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity != value)
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(LineTotal));
            }
        }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (_unitPrice != value)
            {
                _unitPrice = value;
                OnPropertyChanged(nameof(UnitPrice));
                OnPropertyChanged(nameof(LineTotal));
            }
        }
    }

    /// <summary>
    /// Thành tiền tự động tính: Quantity * UnitPrice
    /// </summary>
    public decimal LineTotal => Quantity * UnitPrice;

    public string Notes
    {
        get => _notes;
        set
        {
            if (_notes != value)
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
                OnPropertyChanged(nameof(DisplayNameWithNotes));
            }
        }
    }

    /// <summary>
    /// Hiển thị tên món kèm tóm tắt tùy chọn (nếu có)
    /// </summary>
    public string DisplayNameWithNotes
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Notes))
                return ProductName;
            return $"{ProductName}\n({Notes})";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
