using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DrivingCertificateApp.Models;

namespace DrivingCertificateApp.Views
{
    public partial class Register : Window
    {
        private readonly PrnProjectContext _context = new PrnProjectContext();

        public Register()
        {
            InitializeComponent();
        }
        // Xử lý sự kiện khi thay đổi Role
        private void CmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRole.SelectedItem is ComboBoxItem selectedItem && txtClass != null && txtSchool != null)
            {
                string role = selectedItem.Content.ToString();
                if (role == "Student")
                {
                    txtClass.IsEnabled = true;
                    txtSchool.IsEnabled = true;
                }
                else if (role == "Teacher")
                {
                    txtClass.IsEnabled = false;
                    txtSchool.IsEnabled = false;
                    txtClass.Text = "";
                    txtSchool.Text = "";
                }
                else
                {
                    txtClass.IsEnabled = false;
                    txtSchool.IsEnabled = false;
                }
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();
            string confirmPassword = txtConfirmPassword.Password.Trim();
            string role = (cmbRole.SelectedItem as ComboBoxItem)?.Content.ToString();
            string className = txtClass.Text.Trim();
            string school = txtSchool.Text.Trim();
            string phone = txtPhone.Text.Trim();

            // Validate dữ liệu
            if (string.IsNullOrEmpty(fullName) && string.IsNullOrEmpty(email) &&
                string.IsNullOrEmpty(password) && string.IsNullOrEmpty(confirmPassword) &&
                role == "Role")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin cơ bản và chọn vai trò!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            } else if  (string.IsNullOrEmpty(fullName))
                    {
                MessageBox.Show("Vui lòng nhập họ và tên của bạn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email của bạn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (!email.Contains("@email.com"))
            {
                MessageBox.Show("Email phải chứa @email.com !", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu xác nhận!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            } if (role == "Role")
            {
                MessageBox.Show("Vui lòng chọn vai trò của bạn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            } if (role == "Student" && (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(school)))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin Lớp và Trường khi chọn vai trò Student!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            } if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại của bạn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
                if (existingUser != null)
                {
                    MessageBox.Show("Email đã được sử dụng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newUser = new User
                {
                    FullName = fullName,
                    Email = email,
                    Password = password,
                    Role = role, // Lưu role đã chọn
                    Class = role == "Student" ? className : "", // Chỉ lưu nếu là Student
                    School = role == "Student" ? school : "",   // Chỉ lưu nếu là Student
                    Phone = phone
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thành công");
                Login loginWindow = new Login();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Hyperlink_Login_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}
