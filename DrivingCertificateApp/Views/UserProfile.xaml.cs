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
    public partial class UserProfile : Window
    {
        private readonly PrnProjectContext _context;
        private readonly int _userId; // ID của người dùng hiện tại
        public UserProfile(int userId)
        {
            InitializeComponent();
            _context = new PrnProjectContext();
            _userId = userId;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserData();
        }
        private void LoadUserData()
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == _userId);
            if (user != null)
            {
                txtFullName.Text = user.FullName;
                txtEmail.Text = user.Email;
                txtClass.Text = user.Class;
                txtSchool.Text = user.School;
                txtPhone.Text = user.Phone;
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin người dùng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == _userId);
            if (user != null)
            {
                user.FullName = txtFullName.Text;
                user.Class = txtClass.Text;
                user.School = txtSchool.Text;
                user.Phone = txtPhone.Text;

                _context.SaveChanges();
                MessageBox.Show("Thông tin đã được cập nhật!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Lỗi khi cập nhật thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
    }
}
