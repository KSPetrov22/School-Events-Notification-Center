namespace DataAccessLayer3.Models;

public sealed class NotificationJob
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING";
    public int Attempts { get; set; }
    public int MaxAttempts { get; set; } = 3;
    public string IdempotencyKey { get; set; } = string.Empty;
    public string? LastError { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
