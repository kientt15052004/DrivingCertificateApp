using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// <summary>
    /// Interaction logic for TeacherDashboard.xaml
    /// </summary>
    public partial class TeacherDashboard : Window
    {
        private readonly PrnProjectContext _context;
        private Notification _selectedNotification;
        public TeacherDashboard()
        {
            InitializeComponent();
            _context = new PrnProjectContext();
            LoadNotifications();
        }
        private void LoadNotifications()
        {
            // Load danh sách thông báo của giảng viên hiện tại
            var notifications = _context.Notifications
                .Where(n => n.UserId == CurrentSession.UserId)
                .OrderByDescending(n => n.SentDate)
                .ToList();
            lvNotifications.ItemsSource = notifications;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            ProcessRegistration("Approved");
        }

        private void btnDecline_Click(object sender, RoutedEventArgs e)
        {
            ProcessRegistration("Declined");
        }

        private void ProcessRegistration(string status)
        {
            // Kiểm tra xem giảng viên đã chọn thông báo chưa
            _selectedNotification = lvNotifications.SelectedItem as Notification;
            if (_selectedNotification == null)
            {
                MessageBox.Show("Vui lòng chọn một thông báo để xử lý!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Trích xuất thông tin học sinh và khóa học từ thông báo
            // Message có dạng: "Học sinh {FullName} đã đăng ký khóa học {CourseName}. Vui lòng duyệt."
            string message = _selectedNotification.Message;
            var match = Regex.Match(message, @"Học sinh (.*) đã đăng ký khóa học (.*)\. Vui lòng duyệt\.");
            if (!match.Success)
            {
                MessageBox.Show("Không thể xử lý thông báo này!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string studentName = match.Groups[1].Value;
            string courseName = match.Groups[2].Value;

            // Tìm học sinh và khóa học
            var student = _context.Users.FirstOrDefault(u => u.FullName == studentName && u.Role == "Student");
            var course = _context.Courses.FirstOrDefault(c => c.CourseName == courseName && c.TeacherId == CurrentSession.UserId);
            if (student == null || course == null)
            {
                MessageBox.Show("Không tìm thấy học sinh hoặc khóa học!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Tìm bản ghi đăng ký
            var registration = _context.Registrations
                .FirstOrDefault(r => r.UserId == student.UserId && r.CourseId == course.CourseId && r.Status == "Pending");
            if (registration == null)
            {
                MessageBox.Show("Không tìm thấy bản ghi đăng ký!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Cập nhật trạng thái đăng ký
            registration.Status = status;
            _selectedNotification.IsRead = true;

            // Gửi thông báo cho học sinh
            //var studentNotification = new Notification
            //{
            //    UserId = student.UserId,
            //    Message = $"Yêu cầu đăng ký khóa học {course.CourseName} của bạn đã được {status.ToLower()}.",
            //    SentDate = DateTime.Now,
            //    IsRead = false
            //};
            //_context.Notifications.Add(studentNotification);

            // Nếu "Accept", giả lập đăng ký kỳ thi bằng cách thêm vào bảng Results
            if (status == "Approved")
            {
                var exam = _context.Exams.FirstOrDefault(e => e.CourseId == course.CourseId);
                if (exam != null)
                {
                    var result = new Result
                    {
                        ExamId = exam.ExamId,
                        UserId = student.UserId,
                        Score = 0,
                        PassStatus = false
                    };
                    _context.Results.Add(result);
                }
            }

            // Lưu tất cả thay đổi
            _context.SaveChanges();

            MessageBox.Show($"Đã {status.ToLower()} yêu cầu đăng ký!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadNotifications();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadNotifications();
        }
    }
}
