
using System.ComponentModel;
using System.Text;
using System.Windows;

using System.Windows.Input;

using System.Windows.Media.Imaging;
using WebSocketSharp.Server;
using DrivingCertificateApp.Models;
using DrivingCertificateApp.Views;
using System.Windows.Threading;

namespace DrivingCertificateApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Visibility _stuVisibility;
        private Visibility _teacherVisibility;

        private bool isImage1 = true;
        private DispatcherTimer timer;

        private WebSocketServer _wssv;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; 
            StuVisibility = Visibility.Collapsed;
            TeacherVisibility = Visibility.Collapsed;
            checkRole();
            DisplayWelcomeMessage();

            StartWebSocketServer();

            // Thiết lập timer để tự động thay đổi hình ảnh mỗi 5 giây
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            btnChangeBackground_Click(null, null);
        }

        private void btnChangeBackground_Click(object sender, RoutedEventArgs e)
        {
            if (isImage1)
            {
                BackgroundImage.ImageSource = new BitmapImage(new Uri("D:\\FPT Education\\SP25\\1_PRN212\\PRN_PROJECT\\DrivingCertificateApp\\Image\\car4.jpg"));
                isImage1 = false;
            }
            else
            {
                BackgroundImage.ImageSource = new BitmapImage(new Uri("D:\\FPT Education\\SP25\\1_PRN212\\PRN_PROJECT\\DrivingCertificateApp\\Image\\car2.jpg"));
                isImage1 = true;
            }
        }
        private void DisplayWelcomeMessage()
        {
            if (!string.IsNullOrEmpty(CurrentSession.FullName))
            {
                txtWelcomeUser.Text = $"Hello {CurrentSession.FullName} !";
            }
            else
            {
                txtWelcomeUser.Text = "Hello Guest";
            }
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
            certificateWindow.ShowDialog();
        }

        //lâm tùng-------------------------------------
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


        //lâm tùng-------------------------------------


        //tuấn
        private void TextBlock_ChatHs(object sender, MouseButtonEventArgs e)
        {

            ChatArea.Visibility = Visibility.Visible;
            MainFrame.Visibility = Visibility.Collapsed;
        }
        private void TextBlock_ChatGv(object sender, MouseButtonEventArgs e)
        {

            ChatArea.Visibility = Visibility.Visible;
            MainFrame.Visibility = Visibility.Collapsed;
        }

        private void StartWebSocketServer()
        {
            _wssv = new WebSocketServer(); // Initialize the WebSocket server
            try
            {
                _wssv.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi động WebSocket server: {ex.Message}");
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_wssv != null)
            {
                _wssv.Stop();
            }
        }
        private void btnMockTest_Click(object sender, MouseButtonEventArgs e)
        {
            var mockTestWindow = new MockTest();
            mockTestWindow.ShowDialog();
        }
        //tuan



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

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            //về trang home, ẩn chat
            ChatArea.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;

        }
    }
}