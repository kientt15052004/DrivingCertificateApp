using System;
using System.Collections.Generic;

namespace DrivingCertificateApp.Models;

public partial class Question
{
    public int Id { get; set; }

    public string? Text { get; set; }

    public string? Option1 { get; set; }

    public string? Option2 { get; set; }

    public string? Option3 { get; set; }

    public string? Option4 { get; set; }

    public int? CorrectAnswer { get; set; }

    public string? Image { get; set; }
}
