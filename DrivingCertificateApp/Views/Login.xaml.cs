using System;
using System.Linq;
using System.Windows;
using DrivingCertificateApp.Models;
using Microsoft.Win32;

namespace DrivingCertificateApp.Views
{
    public partial class Login : Window
    {
        private readonly PrnProjectContext _context = new PrnProjectContext();


        public Login()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(email) )
            {
                lblError.Text = "Vui lòng nhập email hợp lệ!";
                lblError.Visibility = Visibility.Visible;
                return;
            } else if (string.IsNullOrEmpty(password))
            {
                lblError.Text = "Vui lòng nhập mật khẩu hợp lệ!";
                lblError.Visibility = Visibility.Visible;
                return;
            } else if (!email.Contains("@email.com"))
            {
                lblError.Text = "Email phải chứa '@email.com'!";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                // Xác thực người dùng trong database
                var user = _context.Users
                                   .FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user == null)
                {
                    lblError.Text = "Email hoặc mật khẩu không đúng.";
                    lblError.Visibility = Visibility.Visible;
                }
                else
                {
                    // Gán thông tin vào CurrentSession
                    CurrentSession.UserId = user.UserId;
                    CurrentSession.FullName = user.FullName;
                    CurrentSession.Email = user.Email;
                    CurrentSession.Role = user.Role;
                    CurrentSession.Class = user.Class;
                    CurrentSession.School = user.School;
                    CurrentSession.Phone = user.Phone;

                    MessageBox.Show($"Xin chào, {user.FullName}!", "Đăng nhập thành công" );

                    // Điều hướng đến màn hình chính (MainWindow)
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Hyperlink_Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Register registerWindow = new Register(); // Tạo instance của Register
                registerWindow.ShowDialog(); // Hiển thị form Register
                this.Close(); // Đóng form Login
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở form đăng ký: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Đóng toàn bộ ứng dụng
        }
    }
}
