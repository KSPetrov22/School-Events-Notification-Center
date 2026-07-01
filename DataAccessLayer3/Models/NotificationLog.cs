namespace DataAccessLayer3.Models;

public sealed class NotificationLog
{
    public string Id { get; set; } = string.Empty;
    public string? JobId { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime SentAt { get; set; }
}
