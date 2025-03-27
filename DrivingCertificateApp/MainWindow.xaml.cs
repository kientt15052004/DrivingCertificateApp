using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DrivingCertificateApp.Models;
using DrivingCertificateApp.Views;

namespace DrivingCertificateApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Visibility _stuVisibility;
        private Visibility _teacherVisibility;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // Đảm bảo rằng DataContext của cửa sổ là chính nó
            StuVisibility = Visibility.Collapsed;
            TeacherVisibility = Visibility.Collapsed;
            checkRole();

        }
        public void checkRole()
        {
            if (CurrentSession.Role == "Teacher")
            {
                TeacherVisibility = Visibility.Visible;
                StuVisibility = Visibility.Collapsed;
            }
            else
            {
                StuVisibility = Visibility.Visible;
                TeacherVisibility = Visibility.Collapsed;
            }
        }
        public Visibility StuVisibility
        {
            get { return _stuVisibility; }
            set
            {
                _stuVisibility = value;
                OnPropertyChanged(nameof(StuVisibility)); // Thông báo UI khi giá trị thay đổi
            }
        }
        public Visibility TeacherVisibility
        {
            get { return _teacherVisibility; }
            set
            {
                _teacherVisibility = value;
                OnPropertyChanged(nameof(TeacherVisibility)); // Thông báo UI khi giá trị thay đổi
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        private void btnRegisterCourse_Click(object sender, RoutedEventArgs e)
        {
            var registerCourseWindow = new RegisterCourse();
            registerCourseWindow.ShowDialog();
        }

        private void btnTeacherDashboard_Click(object sender, RoutedEventArgs e)
        {
            var teacherDashboardWindow = new TeacherDashboard();
            teacherDashboardWindow.ShowDialog();
        }
        private void btnViewCertificate_Click(object sender, RoutedEventArgs e)
        {
            var certificateWindow = new StudentCertificate();
            certificateWindow.Show();
        }
        private void TbStudent_Click(object sender, MouseButtonEventArgs e)
        {
            //StudentManager StudentManagerWindow = new StudentManager();
            //StudentManagerWindow.ShowDialog();
        }

        private void TbScore_Click(object sender, MouseButtonEventArgs e)
        {
        //    ScoreManager ScoreManagerWindow = new ScoreManager();
        //    ScoreManagerWindow.ShowDialog();
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