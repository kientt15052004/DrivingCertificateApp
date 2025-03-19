using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivingCertificateApp.Models
{
    public static class CurrentSession
    {
        public static int UserId { get; set; } // ID của người dùng hiện tại
        public static string FullName { get; set; } // Tên đầy đủ
        public static string Email { get; set; } // Email
        public static string Role { get; set; }  // Vai trò (Student, Teacher, Admin, ...)
        public static string Class { get; set; } // Lớp học (nếu là học viên)
        public static string School { get; set; } // Trường học (nếu là học viên)
        public static string Phone { get; set; } // Số điện thoại
    }
}

