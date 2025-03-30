using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivingCertificateApp.Models
{
    public class Message
    {
        public string SenderName { get; set; }
        public DateTime SentDate { get; set; }
        public string Content { get; set; }

        public bool IsCurrentUser { get; set; }
    }
}
