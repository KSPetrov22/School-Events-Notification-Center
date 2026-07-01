using DataAccessLayer3.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer3.Db;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<SchoolEvent> Events => Set<SchoolEvent>();
    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<NotificationJob> NotificationJobs => Set<NotificationJob>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.Email).IsUnique();
            entity.HasIndex(user => user.Role);
            entity.Property(user => user.Id).HasColumnName("id");
            entity.Property(user => user.Email).HasColumnName("email");
            entity.Property(user => user.PasswordHash).HasColumnName("password_hash");
            entity.Property(user => user.Role).HasColumnName("role");
            entity.Property(user => user.DisplayName).HasColumnName("display_name");
            entity.Property(user => user.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("datetime('now')");
        });

        modelBuilder.Entity<SchoolEvent>(entity =>
        {
            entity.ToTable("events");
            entity.HasKey(evt => evt.Id);
            entity.HasIndex(evt => evt.OrganizerId);
            entity.HasIndex(evt => evt.Status);
            entity.HasIndex(evt => evt.StartsAt);
            entity.Property(evt => evt.Id).HasColumnName("id");
            entity.Property(evt => evt.OrganizerId).HasColumnName("organizer_id");
            entity.Property(evt => evt.Title).HasColumnName("title");
            entity.Property(evt => evt.Description).HasColumnName("description").HasDefaultValue("");
            entity.Property(evt => evt.StartsAt).HasColumnName("starts_at");
            entity.Property(evt => evt.EndsAt).HasColumnName("ends_at");
            entity.Property(evt => evt.Capacity).HasColumnName("capacity");
            entity.Property(evt => evt.Location).HasColumnName("location");
            entity.Property(evt => evt.Status).HasColumnName("status").HasDefaultValue("DRAFT");
            entity.Property(evt => evt.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("datetime('now')");
            entity.Property(evt => evt.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("datetime('now')");
            entity.HasOne(evt => evt.Organizer)
                .WithMany(user => user.OrganizedEvents)
                .HasForeignKey(evt => evt.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.ToTable("registrations");
            entity.HasKey(registration => registration.Id);
            entity.HasIndex(registration => new { registration.EventId, registration.Status });
            entity.HasIndex(registration => registration.UserId);
            entity.HasIndex(registration => new { registration.EventId, registration.RegisteredAt });
            entity.HasIndex(registration => new { registration.EventId, registration.UserId })
                .IsUnique()
                .HasFilter("status IN ('CONFIRMED', 'WAITLISTED')");
            entity.Property(registration => registration.Id).HasColumnName("id");
            entity.Property(registration => registration.EventId).HasColumnName("event_id");
            entity.Property(registration => registration.UserId).HasColumnName("user_id");
            entity.Property(registration => registration.Status).HasColumnName("status");
            entity.Property(registration => registration.RegisteredAt).HasColumnName("registered_at").HasDefaultValueSql("datetime('now')");
            entity.Property(registration => registration.CancelledAt).HasColumnName("cancelled_at");
            entity.HasOne(registration => registration.Event)
                .WithMany(evt => evt.Registrations)
                .HasForeignKey(registration => registration.EventId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(registration => registration.User)
                .WithMany(user => user.Registrations)
                .HasForeignKey(registration => registration.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<NotificationJob>(entity =>
        {
            entity.ToTable("notification_jobs");
            entity.HasKey(job => job.Id);
            entity.HasIndex(job => job.IdempotencyKey).IsUnique();
            entity.HasIndex(job => job.Type);
            entity.Property(job => job.Id).HasColumnName("id");
            entity.Property(job => job.Type).HasColumnName("type");
            entity.Property(job => job.Payload).HasColumnName("payload");
            entity.Property(job => job.Status).HasColumnName("status").HasDefaultValue("PENDING");
            entity.Property(job => job.Attempts).HasColumnName("attempts").HasDefaultValue(0);
            entity.Property(job => job.MaxAttempts).HasColumnName("max_attempts").HasDefaultValue(3);
            entity.Property(job => job.IdempotencyKey).HasColumnName("idempotency_key");
            entity.Property(job => job.LastError).HasColumnName("last_error");
            entity.Property(job => job.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("datetime('now')");
            entity.Property(job => job.ProcessedAt).HasColumnName("processed_at");
        });

        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.ToTable("notification_log");
            entity.HasKey(log => log.Id);
            entity.HasIndex(log => log.JobId);
            entity.HasIndex(log => log.SentAt);
            entity.Property(log => log.Id).HasColumnName("id");
            entity.Property(log => log.JobId).HasColumnName("job_id");
            entity.Property(log => log.RecipientEmail).HasColumnName("recipient_email");
            entity.Property(log => log.Type).HasColumnName("type");
            entity.Property(log => log.Subject).HasColumnName("subject");
            entity.Property(log => log.Success).HasColumnName("success");
            entity.Property(log => log.ErrorMessage).HasColumnName("error_message");
            entity.Property(log => log.SentAt).HasColumnName("sent_at").HasDefaultValueSql("datetime('now')");
        });
    }
}
