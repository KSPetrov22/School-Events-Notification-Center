using System.Text.Json;
using BusinessLogicLayer2.Dtos;
using DataAccessLayer3.Repositories;

namespace BusinessLogicLayer2.Services;

public sealed class NotificationService(INotificationRepository repository) : INotificationService
{
    public async Task<ProcessNotificationsResult> ProcessPendingAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var jobs = await repository.ClaimPendingAsync(batchSize, cancellationToken);
        var processed = 0;
        var failed = 0;

        foreach (var job in jobs)
        {
            try
            {
                var recipient = ExtractRecipient(job.Payload);
                var subject = SubjectFor(job.Type);
                await repository.MarkSentAsync(job.Id, recipient, subject, cancellationToken);
                processed += 1;
            }
            catch (Exception ex)
            {
                failed += 1;
                await repository.MarkFailedAsync(job.Id, "local-presentation@school-events.local", SubjectFor(job.Type), ex.Message, cancellationToken);
            }
        }

        return new ProcessNotificationsResult(processed, failed);
    }

    public async Task<IReadOnlyList<NotificationLogSummary>> GetRecentLogsAsync(int limit, CancellationToken cancellationToken = default) =>
        (await repository.GetRecentLogsAsync(limit, cancellationToken))
        .Select(log => new NotificationLogSummary(
            log.Id,
            log.JobId,
            log.RecipientEmail,
            log.Type,
            log.Subject,
            log.Success,
            log.ErrorMessage,
            log.SentAt))
        .ToList();

    private static string SubjectFor(string type) => type switch
    {
        "RegistrationConfirmed" => "Registration confirmed",
        "RegistrationWaitlisted" => "Added to waitlist",
        "RegistrationCancelled" => "Registration cancelled",
        "WaitlistPromoted" => "Waitlist spot promoted",
        "EventCancelled" => "Event cancelled",
        _ => $"Notification: {type}",
    };

    private static string ExtractRecipient(string payload)
    {
        using var doc = JsonDocument.Parse(payload);
        if (doc.RootElement.TryGetProperty("user_email", out var email) && email.GetString() is { Length: > 0 } value)
        {
            return value;
        }

        if (doc.RootElement.TryGetProperty("user_id", out var userId) && userId.GetString() is { Length: > 0 } user)
        {
            return $"{user}@school-events.local";
        }

        return "local-presentation@school-events.local";
    }
}
