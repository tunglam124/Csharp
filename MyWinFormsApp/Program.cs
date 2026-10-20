using MyWinFormsApp.Forms;
using MyWinFormsApp.Models;

namespace MyWinFormsApp;

static class Program
{
    /// <summary>
    /// Điểm khởi động chính của ứng dụng POS
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        while (true)
        {
            using var loginForm = new frmLogin();
            if (loginForm.ShowDialog() != DialogResult.OK)
            {
                break; // Người dùng đóng form đăng nhập -> Thoát ứng dụng
            }

            using var posForm = new frmPOS();
            Application.Run(posForm);

            // Nếu người dùng chọn Đăng xuất từ POS, vòng lặp quay lại frmLogin
            if (!posForm.RequestedLogout)
            {
                break; // Thoát hẳn ứng dụng khi đóng POS
            }

            UserSession.Logout();
        }
    }    
}