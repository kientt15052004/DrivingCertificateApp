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

namespace DrivingCertificateApp.Views
{
    /// <summary>
    /// Interaction logic for StudentCertificate.xaml
    /// </summary>
    public partial class StudentCertificate : Window
    {
        private readonly PrnProjectContext _context;
        public StudentCertificate()
        {
            InitializeComponent();
            _context = new PrnProjectContext();
            LoadCertificate();
        }
        private void LoadCertificate()
        {
            try
            {
                // Lấy danh sách kết quả thi của học sinh hiện tại với PassStatus = 1(true)
                var passedResults = _context.Results
                    .Include(r => r.Exam)
                    .ThenInclude(e => e.Course)
                    .Where(r => r.UserId == CurrentSession.UserId && r.PassStatus == true)
                    .ToList();

                if (passedResults.Any())
                {
                    // Lấy kết quả đầu tiên
                    var result = passedResults.First();

                    // Gán danh sách vào ComboBox
                    cbCourses.ItemsSource = passedResults;
                    cbCourses.SelectedIndex = 0; // Chọn chứng chỉ đầu tiên mặc định

                    // Hiển thị thông tin chứng chỉ
                    txtStudentName.Text = CurrentSession.FullName;
                    txtCourseName.Text = result.Exam.Course.CourseName;
                    txtIssueDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                }
                else
                {
                    //MessageBox.Show("Bạn chưa có chứng chỉ nào! Vui lòng hoàn thành kỳ thi với trạng thái Pass.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    //this.Close();
                    txtStudentName.Text = "";
                    txtCourseName.Text = "Bạn chưa có chứng chỉ nào!";
                    txtIssueDate.Text = "";
                    cbCourses.Visibility = Visibility.Collapsed; // Ẩn ComboBox nếu không có chứng chỉ
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải chứng chỉ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
        private void cbCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCourses.SelectedItem is Result result)
            {
                // Cập nhật thông tin chứng chỉ khi chọn khóa học
                txtStudentName.Text = CurrentSession.FullName;
                txtCourseName.Text = result.Exam.Course.CourseName;
                txtIssueDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
    }
}
