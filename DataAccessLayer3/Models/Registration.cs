namespace DataAccessLayer3.Models;

public sealed class Registration
{
    public string Id { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RegisteredAt { get; set; } = string.Empty;
    public string? CancelledAt { get; set; }

    public SchoolEvent? Event { get; set; }
    public User? User { get; set; }
}
