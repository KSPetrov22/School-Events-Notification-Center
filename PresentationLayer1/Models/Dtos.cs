using System.ComponentModel.DataAnnotations;

namespace PresentationLayer1.Models;

public sealed record UserInfo(string Id, string Email, string Role, string DisplayName);

public sealed record LoginResponse(string Token, UserInfo User);

public sealed record EventSummary(
    string Id,
    string OrganizerId,
    string Title,
    string Description,
    string StartsAt,
    string EndsAt,
    int Capacity,
    string? Location,
    string Status,
    int ConfirmedCount,
    int WaitlistCount,
    bool IsFull,
    string? CurrentUserRegistrationId,
    string? CurrentUserRegistrationStatus,
    int? CurrentUserWaitlistPosition);

public sealed record RegistrationSummary(
    string Id,
    string EventId,
    string EventTitle,
    string UserId,
    string UserDisplayName,
    string UserEmail,
    string Status,
    string RegisteredAt,
    int? WaitlistPosition);

public sealed class EventUpsertRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string StartsAt { get; set; } = string.Empty;

    [Required]
    public string EndsAt { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; } = 1;

    public string? Location { get; set; }
}

public sealed record LoginRequest([Required] string Email, string Password = "");
