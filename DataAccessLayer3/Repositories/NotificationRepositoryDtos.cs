namespace DataAccessLayer3.Repositories;

public sealed record NotificationJobRecord(
    string Id,
    string Type,
    string Payload,
    int Attempts,
    int MaxAttempts,
    string IdempotencyKey);

public sealed record NotificationLogRecord(
    string Id,
    string? JobId,
    string RecipientEmail,
    string Type,
    string? Subject,
    bool Success,
    string? ErrorMessage,
    string SentAt);
