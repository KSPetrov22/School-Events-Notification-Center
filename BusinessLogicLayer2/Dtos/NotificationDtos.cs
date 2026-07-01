namespace BusinessLogicLayer2.Dtos;

public sealed record ProcessNotificationsResult(int Processed, int Failed);

public sealed record NotificationLogSummary(
    string Id,
    string? JobId,
    string RecipientEmail,
    string Type,
    string? Subject,
    bool Success,
    string? ErrorMessage,
    string SentAt);
