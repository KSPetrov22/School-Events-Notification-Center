using BusinessLogicLayer2.Dtos;

namespace BusinessLogicLayer2.Services;

public interface INotificationService
{
    Task<ProcessNotificationsResult> ProcessPendingAsync(int batchSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationLogSummary>> GetRecentLogsAsync(int limit, CancellationToken cancellationToken = default);
}
