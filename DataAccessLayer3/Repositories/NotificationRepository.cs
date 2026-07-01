using DataAccessLayer3.Db;
using DataAccessLayer3.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer3.Repositories;

public sealed class NotificationRepository(AppDbContext db) : INotificationRepository
{
    public async Task<IReadOnlyList<NotificationJobRecord>> ClaimPendingAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var jobs = await db.NotificationJobs
            .Where(job => job.Status == "PENDING" && job.Attempts < job.MaxAttempts)
            .OrderBy(job => job.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        foreach (var job in jobs)
        {
            job.Status = "PROCESSING";
            job.Attempts += 1;
        }

        await db.SaveChangesAsync(cancellationToken);
        return jobs.Select(job => new NotificationJobRecord(
            job.Id,
            job.Type,
            job.Payload,
            job.Attempts,
            job.MaxAttempts,
            job.IdempotencyKey)).ToList();
    }

    public async Task MarkSentAsync(string jobId, string recipientEmail, string subject, CancellationToken cancellationToken = default)
    {
        var job = await db.NotificationJobs.FirstAsync(job => job.Id == jobId, cancellationToken);
        job.Status = "SENT";
        job.ProcessedAt = DateTime.UtcNow;
        job.LastError = null;
        db.NotificationLogs.Add(new NotificationLog
        {
            Id = Guid.NewGuid().ToString("N"),
            JobId = jobId,
            RecipientEmail = recipientEmail,
            Type = job.Type,
            Subject = subject,
            Success = true,
        });
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(string jobId, string recipientEmail, string subject, string error, CancellationToken cancellationToken = default)
    {
        var job = await db.NotificationJobs.FirstAsync(job => job.Id == jobId, cancellationToken);
        job.Status = job.Attempts >= job.MaxAttempts ? "FAILED" : "PENDING";
        job.LastError = error;
        if (job.Status == "FAILED")
        {
            job.ProcessedAt = DateTime.UtcNow;
        }

        db.NotificationLogs.Add(new NotificationLog
        {
            Id = Guid.NewGuid().ToString("N"),
            JobId = jobId,
            RecipientEmail = recipientEmail,
            Type = job.Type,
            Subject = subject,
            Success = false,
            ErrorMessage = error,
        });
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationLogRecord>> GetRecentLogsAsync(int limit, CancellationToken cancellationToken = default)
    {
        var logs = await db.NotificationLogs.AsNoTracking()
            .OrderByDescending(log => log.SentAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return logs.Select(log => new NotificationLogRecord(
            log.Id,
            log.JobId,
            log.RecipientEmail,
            log.Type,
            log.Subject,
            log.Success,
            log.ErrorMessage,
            log.SentAt.ToString("O"))).ToList();
    }
}
