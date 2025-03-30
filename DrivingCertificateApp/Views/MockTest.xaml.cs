using DrivingCertificateApp.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DrivingCertificateApp.Views
{
    /// <summary>
    /// Interaction logic for MockTest.xaml
    /// </summary>
    public partial class MockTest : Window
    {
        private List<Question> questions;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private readonly PrnProjectContext _context;
        public MockTest()
        {
            InitializeComponent();
            _context = new PrnProjectContext();
            LoadQuestions();
            DisplayQuestion();
        }
        private void LoadQuestions()
        {
            try
            {
                if (_context == null)
                {
                    MessageBox.Show("Không thể khởi tạo PrnProjectContext!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    questions = new List<Question>();
                    return;
                }

                // Lấy tất cả câu hỏi từ bảng Question
                questions = _context.Questions.ToList();

                if (questions == null || questions.Count == 0)
                {
                    MessageBox.Show("Không có câu hỏi nào trong cơ sở dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải câu hỏi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DisplayQuestion()
        {
            if (currentQuestionIndex >= 0 && currentQuestionIndex < questions.Count)
            {
                var question = questions[currentQuestionIndex];
                txtQuestion.Text = $"Câu {currentQuestionIndex + 1}: {question.Text}";
                rbOption1.Visibility = Visibility.Collapsed;
                rbOption2.Visibility = Visibility.Collapsed;
                rbOption3.Visibility = Visibility.Collapsed;
                rbOption4.Visibility = Visibility.Collapsed;

                if (!string.IsNullOrWhiteSpace(question.Option1))
                {
                    rbOption1.Content = question.Option1;
                    rbOption1.Visibility = Visibility.Visible;
                }

                if (!string.IsNullOrWhiteSpace(question.Option2))
                {
                    rbOption2.Content = question.Option2;
                    rbOption2.Visibility = Visibility.Visible;
                }

                if (!string.IsNullOrWhiteSpace(question.Option3))
                {
                    rbOption3.Content = question.Option3;
                    rbOption3.Visibility = Visibility.Visible;
                }

                if (!string.IsNullOrWhiteSpace(question.Option4))
                {
                    rbOption4.Content = question.Option4;
                    rbOption4.Visibility = Visibility.Visible;
                }

                // Hiển thị ảnh nếu có
                if (!string.IsNullOrEmpty(question.Image))
                {
                    try
                    {
                        // Kiểm tra xem Image có phải là URL không
                        if (Uri.TryCreate(question.Image, UriKind.Absolute, out Uri uriResult) &&
                            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                        {
                            // Nếu là URL (http hoặc https), tải ảnh từ URL
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = uriResult;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            imgQuestion.Source = bitmap;
                        }
                        else
                        {
                            // Nếu là đường dẫn cục bộ, tải như trước
                            imgQuestion.Source = new BitmapImage(new Uri($"pack://application:,,,/{question.Image}"));
                        }
                        imgQuestion.Visibility = Visibility.Visible;
                    }
                    catch (Exception ex)
                    {
                        imgQuestion.Visibility = Visibility.Collapsed;
                        MessageBox.Show($"Lỗi tải ảnh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    imgQuestion.Visibility = Visibility.Collapsed;
                }

                // Reset lựa chọn
                rbOption1.IsChecked = rbOption2.IsChecked = rbOption3.IsChecked = rbOption4.IsChecked = false;
            }
            else if (questions.Count == 0)
            {
                txtQuestion.Text = "Không có câu hỏi nào để hiển thị.";
                rbOption1.Content = rbOption2.Content = rbOption3.Content = rbOption4.Content = "";
                imgQuestion.Visibility = Visibility.Collapsed;
            }

            // Ẩn/hiện nút điều hướng
            btnPrevious.IsEnabled = currentQuestionIndex > 0;
            btnNext.IsEnabled = currentQuestionIndex < questions.Count - 1;
        }
        private void btn_Prev(object sender, RoutedEventArgs e)
        {
            CheckAnswer();
            currentQuestionIndex--;
            DisplayQuestion();
        }

        private void btn_Next(object sender, RoutedEventArgs e)
        {
            CheckAnswer();
            currentQuestionIndex++;
            DisplayQuestion();
        }

        private void btn_Submit(object sender, RoutedEventArgs e)
        {
            CheckAnswer();
            MessageBox.Show($"Bạn đã hoàn thành bài thi!\nĐiểm số: {score}/{questions.Count}",
                "Kết quả", MessageBoxButton.OK, MessageBoxImage.Information);

            // Reset bài thi
            score = 0;
            currentQuestionIndex = 0;
            LoadQuestions();
        }

        // Kiểm tra đáp án và tính điểm
        private void CheckAnswer()
        {
            if (currentQuestionIndex >= 0 && currentQuestionIndex < questions.Count)
            {
                var question = questions[currentQuestionIndex];
                int selectedAnswer = -1;

                if (rbOption1.IsChecked == true) selectedAnswer = 0;
                else if (rbOption2.IsChecked == true) selectedAnswer = 1;
                else if (rbOption3.IsChecked == true) selectedAnswer = 2;
                else if (rbOption4.IsChecked == true) selectedAnswer = 3;

                if (selectedAnswer == question.CorrectAnswer)
                {
                    score++;
                }
            }

        }
    }
}
