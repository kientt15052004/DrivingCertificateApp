using WebSocketSharp;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DrivingCertificateApp.Models;

namespace DrivingCertificateApp.Views
{
    public partial class Chat : UserControl
    {


        WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket("ws://localhost:8080/chat");

        public ObservableCollection<Message> Messages { get; set; }

        public Chat()
        {
            InitializeComponent();
            Messages = new ObservableCollection<Message>();
            this.DataContext = this;
            LoadMessages();

            ws.OnMessage += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Dispatcher.Invoke(() =>
                    {
                        Messages.Add(new Message
                        {
                            SenderName = "Server",
                            SentDate = DateTime.Now,
                            Content = e.Data
                        });
                        ChatMessages.SelectedItem = ChatMessages.Items[ChatMessages.Items.Count - 1];
                        ChatMessages.ScrollIntoView(ChatMessages.SelectedItem);
                    });
                }
            };
            ws.Connect();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text;
            if (!string.IsNullOrEmpty(message))
            {
                int senderId = CurrentSession.UserId;
                int receiverId = GetTeacherId();

                SaveMessageToDatabase(senderId, receiverId, message);

                Messages.Add(new Message
                {
                    SenderName = GetSenderName(senderId),
                    SentDate = DateTime.Now,
                    Content = message,
                    IsCurrentUser = true
                });

                MessageInput.Clear();
                ChatMessages.SelectedItem = ChatMessages.Items[ChatMessages.Items.Count - 1];
                ChatMessages.ScrollIntoView(ChatMessages.SelectedItem);
            }
        }

        private void SaveMessageToDatabase(int senderId, int receiverId, string content)
        {
            using (var context = new PrnProjectContext())
            {
                var newMessage = new Messenger
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = content,
                    SentDate = DateTime.Now,
                    IsRead = false,
                    Status = "Sent"
                };
                context.Messengers.Add(newMessage);
                context.SaveChanges();
            }
        }

        private int GetTeacherId()
        {
            using (var context = new PrnProjectContext())
            {
                // Lấy ID của giáo viên đầu tiên có role "Teacher"
                var teacher = context.Users
                    .FirstOrDefault(u => u.Role == "Teacher");
                return (int)(teacher?.UserId);
            }
        }

        private void LoadMessages()
        {
            using (var context = new PrnProjectContext())
            {
                var messages = context.Messengers
                    .OrderBy(m => m.SentDate)
                    .ToList();

                foreach (var message in messages)
                {
                    Messages.Add(new Message
                    {
                        SenderName = GetSenderName(message.SenderId),
                        SentDate = message.SentDate,
                        Content = message.Content,
                        IsCurrentUser = (message.SenderId == CurrentSession.UserId)
                    });
                }
            }
        }

        private string GetSenderName(int senderId)
        {
            if (senderId == CurrentSession.UserId)
                return "You";
            using (var context = new PrnProjectContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == senderId);
                return user != null ? user.FullName : "Unknown";
            }
        }
    }
}