using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DrivingCertificateApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingCertificateApp.Views
{
    public partial class ScoreManager : Window
    {
        public ScoreManager()
        {
            InitializeComponent();
            LoadData();
            LoadPassStatus();
        }
        private void LoadPassStatus()
        {
            // Đặt ItemsSource cho ComboBox từ một mảng string
            cbStatus.ItemsSource = new string[] { "Passed", "Not Pass" };
        }
        private void LoadData()
        {
            using (var context = new PrnProjectContext())
            {
                // Lấy dữ liệu từ bảng Results, kèm theo thông tin từ bảng Exam và User
                var results = context.Results
                                      .Include(r => r.Exam)   // Bao gồm dữ liệu từ bảng Exam
                                      .Include(r => r.User)   // Bao gồm dữ liệu từ bảng User
                                      .ToList();  // Lấy danh sách các kết quả

                // Đặt ItemsSource cho DataGrid trực tiếp từ kết quả
                dgResults.ItemsSource = results;
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

        private void dgResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Result result = dgResults.SelectedItem as Result;
            if (result != null)
            {
                txtResultId.Text = result.ResultId.ToString();
                txtExTime.Text = result.Exam.Date.ToString();
                txtStudentName.Text = result.User.FullName.ToString();
                txtScore.Text = result.Score.ToString();
                if (result.PassStatus == true)
                {
                    cbStatus.SelectedIndex = 0;
                }
                else
                {
                    cbStatus.SelectedIndex = 1;
                }
            }
            else
            {
                txtResultId.Text = "";
                txtExTime.Text = "";
                txtStudentName.Text = "";
                txtScore.Text = "";
                cbStatus.Text = "";
            }
        }
        private void btnEditClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtResultId.Text))
            {
                MessageBox.Show("Vui lòng chọn học sinh để chấm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtScore.Text) || cbStatus.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng hoàn thành chấm điểm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new PrnProjectContext())
            {
                Result result = context.Results.Find(int.Parse(txtResultId.Text));
                result.Score = decimal.Parse(txtScore.Text);

                // Cập nhật đúng PassStatus dựa trên giá trị SelectedIndex
                result.PassStatus = cbStatus.SelectedIndex == 0;

                context.SaveChanges();
                LoadData();
            }
        }private void btnDeleClick(object sender, RoutedEventArgs e)
        {
            using (var context = new PrnProjectContext())
            {
                if (string.IsNullOrWhiteSpace(txtResultId.Text))
                {
                    MessageBox.Show("Vui lòng chọn học sinh để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    Result result = context.Results.Find(int.Parse(txtResultId.Text));
                    context.Results.Remove(result);
                    context.SaveChanges();
                    LoadData();
                }
            }
        }
        private void btnResetClick(object sender, RoutedEventArgs e)
        {
            txtResultId.Text = "";
            txtExTime.Text = "";
            txtStudentName.Text = "";
            txtScore.Text = "";
            cbStatus.SelectedIndex = -1;  // Reset ComboBox đúng cách

            LoadData(); // Nạp lại toàn bộ kết quả
        }
        private void btnSearchClick(object sender, RoutedEventArgs e)
        {
            using (var context = new PrnProjectContext())
            {
                string keyword = txtSearch.Text.ToLower();

                var results = context.Results
                                      .Include(r => r.Exam)
                                      .Include(r => r.User)
                                      .Where(r => r.User.FullName.ToLower().Contains(keyword))
                                      .ToList();

                dgResults.ItemsSource = results;
            }
        }
    }
}
