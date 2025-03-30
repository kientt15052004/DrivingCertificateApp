using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DrivingCertificateApp.Models;

public partial class PrnProjectContext : DbContext
{
    public PrnProjectContext()
    {
    }

    public PrnProjectContext(DbContextOptions<PrnProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Certificate> Certificates { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Messenger> Messengers { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=KIEN\\SQLEXPRESS;uid=sa;password=sa;database=PRN_PROJECT;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId).HasName("PK__Certific__BBF8A7E1ADED5B1D");

            entity.HasIndex(e => e.CertificateCode, "UQ__Certific__9B8558301C850DFC").IsUnique();

            entity.Property(e => e.CertificateId).HasColumnName("CertificateID");
            entity.Property(e => e.CertificateCode).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Certifica__UserI__49C3F6B7");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D7187CE0D3B02");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName).HasMaxLength(100);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses).HasForeignKey(d => d.TeacherId);
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exams__297521A7FC6031BF");

            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Room).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.Exams)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exams__CourseID__4222D4EF");
        });

        modelBuilder.Entity<Messenger>(entity =>
        {
            entity.HasKey(e => e.MessageId);

            entity.ToTable("Messenger");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.ReceiverId).HasColumnName("ReceiverID");
            entity.Property(e => e.SenderId).HasColumnName("SenderID");
            entity.Property(e => e.SentDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessengerReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messenger_Users1");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessengerSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messenger_Users");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E321BD7ED63");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__4E88ABD4");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Question");

            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.Option1)
                .HasMaxLength(500)
                .HasColumnName("Option_1");
            entity.Property(e => e.Option2)
                .HasMaxLength(500)
                .HasColumnName("Option_2");
            entity.Property(e => e.Option3)
                .HasMaxLength(500)
                .HasColumnName("Option_3");
            entity.Property(e => e.Option4)
                .HasMaxLength(500)
                .HasColumnName("Option_4");
            entity.Property(e => e.Text).HasMaxLength(300);
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__Registra__6EF58830C0C862F6");

            entity.Property(e => e.RegistrationId).HasColumnName("RegistrationID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Waiting");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Course).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__Cours__3F466844");

            entity.HasOne(d => d.User).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__UserI__3E52440B");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__Results__976902284531C451");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Exam).WithMany(p => p.Results)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Results__ExamID__44FF419A");

            entity.HasOne(d => d.User).WithMany(p => p.Results)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Results__UserID__45F365D3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC0DAD1DCA");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534C2C98C42").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Class).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.School).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
