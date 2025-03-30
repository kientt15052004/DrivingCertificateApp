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
    /// <summary>
    /// Interaction logic for StudentManager.xaml
    /// </summary>
    public partial class StudentManager : Window
    {
        public StudentManager()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            using (var context = new PrnProjectContext())
            {
                // Lấy danh sách các học viên
                var students = context.Users.Where(u => u.Role == "Student").ToList();
                dgStudents.ItemsSource = students;
            }
        }
        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSession.UserId > 0) // Kiểm tra xem đã có thông tin đăng nhập chưa
            {
                UserProfile userProfileWindow = new UserProfile(CurrentSession.UserId);
                userProfileWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Không có thông tin người dùng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TbCourse_Click(object sender, MouseButtonEventArgs e)
        {
            CourseManager CourseManagerWindow = new CourseManager();
            CourseManagerWindow.ShowDialog();
        }

        private void TbStudent_Click(object sender, MouseButtonEventArgs e)
        {
            StudentManager StudentManagerWindow = new StudentManager();
            StudentManagerWindow.ShowDialog();
        }

        private void TbScore_Click(object sender, MouseButtonEventArgs e)
        {
            ScoreManager ScoreManagerWindow = new ScoreManager();
            ScoreManagerWindow.ShowDialog();
        }
        private void dgStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User student = dgStudents.SelectedItem as User;
            if (student != null)
            {
                txtStudentId.Text = student.UserId.ToString();
                txtFullName.Text = student.FullName;
                txtEmail.Text = student.Email;
                txtPhone.Text = student.Phone;
                txtSchool.Text = student.School;
                txtClass.Text = student.Class;
            }
            else
            {
                txtStudentId.Text = "";
                txtFullName.Text = "";
                txtEmail.Text = "";
                txtPhone.Text = "";
                txtSchool.Text = "";
                txtClass.Text = "";
            }
        }
        private void btnEditClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentId.Text))
            {
                MessageBox.Show("Vui lòng chọn học sinh để chỉnh sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string fullName = this.txtFullName.Text;
            string email = this.txtEmail.Text;
            string phone = this.txtPhone.Text;
            string school = this.txtSchool.Text;
            string @class = this.txtClass.Text;
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPhone.Text)
                || string.IsNullOrWhiteSpace(txtSchool.Text) || string.IsNullOrWhiteSpace(txtClass.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin học sinh.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var context = new PrnProjectContext())
            {
                User user = context.Users.Find(int.Parse(txtStudentId.Text));
                user.FullName = user.FullName;
                user.Email = user.Email;
                user.Phone = user.Phone;
                user.School = user.School;
                user.Class = user.Class;
                context.SaveChanges();
                LoadData();
            }
        }
        private void btnDeleClick(object sender, RoutedEventArgs e)
        {
            using (var context = new PrnProjectContext())
            {
                User student = context.Users.Find(int.Parse(txtStudentId.Text));
                context.Users.Remove(student);
                context.SaveChanges();
                LoadData();

            }
        }
        private void btnResetClick(object sender, RoutedEventArgs e)
        {
            txtStudentId.Text = "";
            txtFullName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtSchool.Text = "";
            txtClass.Text = "";
            LoadData();
        }
        private void btnSearchClick(object sender, RoutedEventArgs e)
        {
            using (var context = new PrnProjectContext())
            {
                string search = txtSearch.Text;
                var students = context.Users.Where(u => u.Role == "Student" && u.FullName.Contains(search)).ToList();
                dgStudents.ItemsSource = students;
            }
        }
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Xóa thông tin phiên đăng nhập (nếu cần)
            CurrentSession.ClearSession();

            // Mở lại form Login
            Login loginWindow = new Login();
            loginWindow.Show();

            // Đóng form MainWindow
            this.Close();
        }
    }
}
