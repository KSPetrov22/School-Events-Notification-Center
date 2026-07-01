namespace DataAccessLayer3.Repositories;

public interface INotificationRepository
{
    Task<IReadOnlyList<NotificationJobRecord>> ClaimPendingAsync(int batchSize, CancellationToken cancellationToken = default);
    Task MarkSentAsync(string jobId, string recipientEmail, string subject, CancellationToken cancellationToken = default);
    Task MarkFailedAsync(string jobId, string recipientEmail, string subject, string error, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationLogRecord>> GetRecentLogsAsync(int limit, CancellationToken cancellationToken = default);
}
