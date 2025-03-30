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
using Microsoft.EntityFrameworkCore;
using DrivingCertificateApp.Converters;



namespace DrivingCertificateApp.Views
{
    /// <summary>
    /// Interaction logic for RegisterCourse.xaml
    /// </summary>
    public partial class RegisterCourse : Window
    {
        private readonly PrnProjectContext _context;
        private CourseWithStatus _selectedCourse;
        public RegisterCourse()
        {
            InitializeComponent();
            _context = new PrnProjectContext();
            LoadCourses();
        }
        //private void LoadCourses()
        //{
        //    // Load danh sách khóa học từ bảng Courses, bao gồm thông tin giảng viên
        //    var courses = _context.Courses
        //        .Include(c => c.Teacher)
        //        .ToList();
        //    lvCourses.ItemsSource = courses;
        //}


        private void LoadCourses()
        {
            try
            {
                // Lấy danh sách khóa học và thông tin giảng viên
                var now = DateOnly.FromDateTime(DateTime.Now);
                var courses = _context.Courses
                    .Include(c => c.Teacher)
                    .Where(c => c.StartDate > now)
                    .ToList();

                // Lấy danh sách đăng ký của học sinh hiện tại
                var registrations = _context.Registrations
                    .Where(r => r.UserId == CurrentSession.UserId)
                    .ToList();

                // Ánh xạ dữ liệu vào CourseWithStatus
                var courseWithStatusList = courses.Select(c => new CourseWithStatus
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    TeacherName = c.Teacher?.FullName ?? "Không có giảng viên",
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    // Lấy trạng thái từ bảng Registrations
                    Status = registrations.FirstOrDefault(r => r.CourseId == c.CourseId)?.Status ?? "Waiting"
                }).ToList();

                lvCourses.ItemsSource = courseWithStatusList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            _selectedCourse = lvCourses.SelectedItem as CourseWithStatus;
            if (_selectedCourse == null)
            {
                MessageBox.Show("Vui lòng chọn một khóa học để đăng ký!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existingRegistration = _context.Registrations
                .FirstOrDefault(r => r.UserId == CurrentSession.UserId && r.CourseId == _selectedCourse.CourseId);
            if (existingRegistration != null)
            {
                MessageBox.Show("Bạn đã đăng ký khóa học này rồi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var registration = new Registration
            {
                UserId = CurrentSession.UserId,
                CourseId = _selectedCourse.CourseId,
                Status = "Pending",
                Comments = $"Học sinh đăng ký khóa học {_selectedCourse.CourseName}"
            };
            _context.Registrations.Add(registration);

            var notification = new Notification
            {
                UserId = _context.Courses
                    .Where(c => c.CourseId == _selectedCourse.CourseId)
                    .Select(c => c.TeacherId)
                    .FirstOrDefault(),
                Message = $"Học sinh {CurrentSession.FullName} đã đăng ký khóa học {_selectedCourse.CourseName}. Vui lòng duyệt.",
                SentDate = DateTime.Now,
                IsRead = false
            };
            _context.Notifications.Add(notification);

            _context.SaveChanges();

            MessageBox.Show("Đăng ký thành công! Vui lòng chờ giảng viên duyệt.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadCourses(); // Làm mới danh sách sau khi đăng ký
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
