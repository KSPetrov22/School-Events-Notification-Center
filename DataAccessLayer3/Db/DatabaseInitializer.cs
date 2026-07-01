using DataAccessLayer3.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer3.Db;

public sealed class DatabaseInitializer(AppDbContext db)
{
    public async Task EnsureCreatedAndSeededAsync(CancellationToken cancellationToken = default)
    {
        await db.Database.EnsureCreatedAsync(cancellationToken);

        await EnsureCompatibilityTablesAsync(cancellationToken);

        if (await db.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var password = BCrypt.Net.BCrypt.HashPassword("password");
        db.Users.AddRange(
            new User { Id = "organizer-1", Email = "organizer1@school.local", PasswordHash = password, Role = "ORGANIZER", DisplayName = "Mira Organizer" },
            new User { Id = "student-1", Email = "student1@school.local", PasswordHash = password, Role = "STUDENT", DisplayName = "Alex Student" },
            new User { Id = "student-2", Email = "student2@school.local", PasswordHash = password, Role = "STUDENT", DisplayName = "Nina Student" },
            new User { Id = "student-3", Email = "student3@school.local", PasswordHash = password, Role = "STUDENT", DisplayName = "Theo Student" });

        db.Events.AddRange(
            new SchoolEvent
            {
                Id = "event-robotics",
                OrganizerId = "organizer-1",
                Title = "Robotics Workshop",
                Description = "Build and program small robots in teams.",
                StartsAt = "2026-07-03T14:00:00Z",
                EndsAt = "2026-07-03T16:00:00Z",
                Capacity = 2,
                Location = "Lab 2",
                Status = "PUBLISHED",
            },
            new SchoolEvent
            {
                Id = "event-literature",
                OrganizerId = "organizer-1",
                Title = "Literature Club",
                Description = "Discuss short stories and prepare readings.",
                StartsAt = "2026-07-08T13:00:00Z",
                EndsAt = "2026-07-08T14:30:00Z",
                Capacity = 12,
                Location = "Library",
                Status = "PUBLISHED",
            },
            new SchoolEvent
            {
                Id = "event-draft",
                OrganizerId = "organizer-1",
                Title = "Chemistry Demo Day",
                Description = "Draft event for organizer editing and preview.",
                StartsAt = "2026-07-15T10:00:00Z",
                EndsAt = "2026-07-15T12:00:00Z",
                Capacity = 18,
                Location = "Science Hall",
                Status = "DRAFT",
            });

        db.Registrations.AddRange(
            new Registration { Id = "reg-robotics-1", EventId = "event-robotics", UserId = "student-2", Status = "CONFIRMED", RegisteredAt = "2026-06-20T08:00:00Z" },
            new Registration { Id = "reg-robotics-2", EventId = "event-robotics", UserId = "student-3", Status = "CONFIRMED", RegisteredAt = "2026-06-20T08:05:00Z" },
            new Registration { Id = "reg-robotics-3", EventId = "event-robotics", UserId = "student-1", Status = "WAITLISTED", RegisteredAt = "2026-06-20T08:10:00Z" });

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCompatibilityTablesAsync(CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS notification_log (
                id                  TEXT PRIMARY KEY,
                job_id              TEXT,
                recipient_email     TEXT NOT NULL,
                type                TEXT NOT NULL,
                subject             TEXT,
                success             INTEGER NOT NULL CHECK (success IN (0, 1)),
                error_message       TEXT,
                sent_at             TEXT NOT NULL DEFAULT (datetime('now')),
                FOREIGN KEY (job_id) REFERENCES notification_jobs (id) ON DELETE SET NULL
            );
            """,
            cancellationToken);
    }
}
