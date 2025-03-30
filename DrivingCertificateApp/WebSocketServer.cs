using WebSocketSharp;
using WebSocketSharp.Server;
using System;

namespace DrivingCertificateApp
{
    public class ChatService : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            // Nhận tin nhắn từ client và gửi lại cho client (echo message)
            Send(e.Data);
        }

        protected override void OnOpen()
        {
            Console.WriteLine("Client connected");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("Client disconnected");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    public class WebSocketServer
    {
        private WebSocketSharp.Server.WebSocketServer _server;

        public void Start()
        {
            _server = new WebSocketSharp.Server.WebSocketServer("ws://localhost:8080");

            // Đăng ký dịch vụ WebSocket
            _server.AddWebSocketService<ChatService>("/chat");  // Chỉ định đường dẫn /chat cho dịch vụ Chat

            // Bắt đầu WebSocket server
            _server.Start();
            Console.WriteLine("Server started at ws://localhost:8080/");
        }

        public void Stop()
        {
            if (_server != null)
            {
                _server.Stop();  // Dừng server
                Console.WriteLine("Server stopped");
            }
        }
    }
}
