﻿using System;
using System.Collections.Generic;

namespace DrivingCertificateApp.Models;

public partial class Messenger
{
    public int MessageId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SentDate { get; set; }

    public bool IsRead { get; set; }

    public string Status { get; set; } = null!;

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
