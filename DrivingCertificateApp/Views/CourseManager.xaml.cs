using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DrivingCertificateApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingCertificateApp.Views
{
    public partial class CourseManager : Window
    {
        public CourseManager()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            using (var context = new PrnProjectContext())
            {
                var now = DateOnly.FromDateTime(DateTime.Now);
                var courses = context.Courses
                             .Include(c => c.Teacher)
                             .Where(c => c.StartDate > now)
                             .ToList();

                dgCourses.ItemsSource = courses;
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
        private void btnAddClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCourseName.Text) || dpStart.SelectedDate == null || dpEnd.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin khóa học.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateOnly startDate = DateOnly.FromDateTime(dpStart.SelectedDate.Value.Date);
            DateOnly endDate = DateOnly.FromDateTime(dpEnd.SelectedDate.Value.Date);

            if (startDate >= endDate)
            {
                MessageBox.Show("Ngày bắt đầu phải trước ngày kết thúc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var now = DateOnly.FromDateTime(DateTime.Now);
            if (startDate < now)
            {
                MessageBox.Show("Khóa học mới không thể trước ngày hôm nay!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string courseName = txtCourseName.Text;
            int teacherId = int.Parse(txtTeacherID.Text);

            Course course = new Course(courseName, teacherId, startDate, endDate);

            using (var context = new PrnProjectContext())
            {
                context.Courses.Add(course);
                context.SaveChanges();
                LoadData();
            }
        }
        private void dgCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Course course = dgCourses.SelectedItem as Course;
            if (course != null)
            {
                txtCourseId.Text = course.CourseId.ToString();
                txtCourseName.Text = course.CourseName;
                dpStart.SelectedDate = course.StartDate.ToDateTime(new TimeOnly(0, 0));
                dpEnd.SelectedDate = course.EndDate.ToDateTime(new TimeOnly(0, 0));
                txtTeacherID.Text = course.TeacherId.ToString();
            }
            else
            {
                txtCourseId.Text = "";
                txtCourseName.Text = "";
                dpStart.SelectedDate = DateTime.Now;
                dpEnd.SelectedDate = DateTime.Now;
                txtTeacherID.Text = "";
            }
        }
        private void btnEditClick(object sender, RoutedEventArgs e)
        {
            if (this.txtCourseId == null)
            {
                MessageBox.Show("Vui lòng chọn khóa học để chỉnh sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string courseName = this.txtCourseName.Text;
            DateOnly startDate = DateOnly.FromDateTime(this.dpStart.SelectedDate.Value.Date);
            DateOnly endDate = DateOnly.FromDateTime(this.dpEnd.SelectedDate.Value.Date);
            if (string.IsNullOrWhiteSpace(txtCourseName.Text) || dpStart.SelectedDate == null || dpEnd.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin khóa học.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (startDate >= endDate)
            {
                MessageBox.Show("Ngày bắt đầu phải trước ngày kết thúc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var now = DateOnly.FromDateTime(DateTime.Now);
            if (startDate < now)
            {
                MessageBox.Show("Khóa học mới không thể trước ngày hôm nay!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int teacherId = int.Parse(this.txtTeacherID.Text);
            Course courseUpdate = new Course(courseName, teacherId, startDate, endDate);
            using (var context = new PrnProjectContext())
            {
                if (string.IsNullOrWhiteSpace(txtCourseId.Text))
                {
                    MessageBox.Show("Vui lòng chọn khóa học để chỉnh .", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    Course course = context.Courses.Find(int.Parse(txtCourseId.Text));
                    course.CourseName = courseUpdate.CourseName;
                    course.StartDate = courseUpdate.StartDate;
                    course.EndDate = courseUpdate.EndDate;
                    course.TeacherId = courseUpdate.TeacherId;
                    context.SaveChanges();
                    LoadData();
                }

            }
        }
        private void btnDeleClick(object sender, RoutedEventArgs e)
        {
            using (var context = new PrnProjectContext())
            {
                Course course = context.Courses.Find(int.Parse(txtCourseId.Text));
                context.Courses.Remove(course);
                context.SaveChanges();
                LoadData();
            }
        }
        private void btnResetClick(object sender, RoutedEventArgs e)
        {
            txtCourseId.Text = "";
            txtCourseName.Text = "";
            dpStart.SelectedDate = DateTime.Now;
            dpEnd.SelectedDate = DateTime.Now;
            txtTeacherID.Text = "";
            LoadData();
        }
        private void btnSearchClick(object sender, RoutedEventArgs e)
        {
            using (var context = new PrnProjectContext())
            {
                var now = DateOnly.FromDateTime(DateTime.Now);
                var courses = context.Courses
                             .Include(c => c.Teacher)
                             .Where(c => c.CourseName.Contains(txtSearch.Text) && c.StartDate > now)
                             .ToList();
                dgCourses.ItemsSource = courses;
            }
        }
        private void btnSearchOldClick(object sender, RoutedEventArgs e)
        {
            using (var context = new PrnProjectContext())
            {
                var now = DateOnly.FromDateTime(DateTime.Now);
                var courses = context.Courses
                             .Include(c => c.Teacher)
                             .Where(c => c.CourseName.Contains(txtSearch.Text) && c.StartDate < now)
                             .ToList();
                dgCourses.ItemsSource = courses;
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
