using System.Text.Json;

namespace MyWinFormsApp.Models;

/// <summary>
/// Cấu hình tài khoản ngân hàng để sinh mã VietQR động
/// </summary>
public class BankConfig
{
    public string BankId { get; set; } = "MB"; // MB, VCB, ICB, TCB, ACB, TPB...
    public string BankName { get; set; } = "MBBank (Ngân hàng Quân Đội)";
    public string AccountNumber { get; set; } = "0987654321";
    public string AccountName { get; set; } = "NGUYEN VAN A";

    private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vietqr_config.json");

    private static BankConfig? _current;

    public static BankConfig Current
    {
        get
        {
            if (_current == null)
            {
                _current = Load();
            }
            return _current;
        }
        set => _current = value;
    }

    public static BankConfig Load()
    {
        try
        {
            if (File.Exists(ConfigFilePath))
            {
                string json = File.ReadAllText(ConfigFilePath);
                var config = JsonSerializer.Deserialize<BankConfig>(json);
                if (config != null)
                    return config;
            }
        }
        catch
        {
            // Bỏ qua lỗi, dùng mặc định
        }

        return new BankConfig();
    }

    public void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath, json);
            _current = this;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể lưu cấu hình VietQR: {ex.Message}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>
    /// Sinh URL tạo mã QR động qua VietQR API
    /// Cú pháp: https://img.vietqr.io/image/{bankId}-{accountNo}-compact2.png?amount={amount}&addInfo={content}&accountName={accountName}
    /// </summary>
    public string GenerateQrUrl(decimal amount, string content)
    {
        long roundedAmount = (long)Math.Round(amount, 0);
        string encodedContent = Uri.EscapeDataString(content);
        string encodedName = Uri.EscapeDataString(AccountName.Trim().ToUpper());
        return $"https://img.vietqr.io/image/{BankId.Trim()}-{AccountNumber.Trim()}-compact2.png?amount={roundedAmount}&addInfo={encodedContent}&accountName={encodedName}";
    }
}
