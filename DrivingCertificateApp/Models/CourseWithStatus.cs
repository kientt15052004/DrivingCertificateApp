﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivingCertificateApp.Models
{
    internal class CourseWithStatus
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string TeacherName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } 
    }
}
