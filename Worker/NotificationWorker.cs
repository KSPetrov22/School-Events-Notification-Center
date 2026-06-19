namespace Worker;

// Background host skeleton. The poll loop is here; the actual job processing
// (Epic 4) goes inside the loop once the Business service exists.
public class NotificationWorker(ILogger<NotificationWorker> log) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(
        int.TryParse(Environment.GetEnvironmentVariable("WORKER_POLL_SECONDS"), out var s) ? s : 5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        log.LogInformation("Notification worker started; polling every {Interval}.", PollInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            // TODO (Epic 4): claim + process notification jobs here.
            await Task.Delay(PollInterval, stoppingToken);
        }
    }
}
