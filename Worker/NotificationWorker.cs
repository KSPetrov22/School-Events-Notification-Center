namespace Worker;

using BusinessLogicLayer2.Services;

public class NotificationWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<NotificationWorker> log) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(
        int.TryParse(Environment.GetEnvironmentVariable("WORKER_POLL_SECONDS"), out var s) ? s : 5);
    private static readonly int BatchSize =
        int.TryParse(Environment.GetEnvironmentVariable("WORKER_BATCH_SIZE"), out var size) ? size : 10;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        log.LogInformation("Notification worker started; polling every {Interval}, batch size {BatchSize}.", PollInterval, BatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var notifications = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var result = await notifications.ProcessPendingAsync(BatchSize, stoppingToken);
                if (result.Processed > 0 || result.Failed > 0)
                {
                    log.LogInformation(
                        "Processed notification jobs: {Processed} sent/logged, {Failed} failed.",
                        result.Processed,
                        result.Failed);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Notification worker iteration failed.");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }
}
