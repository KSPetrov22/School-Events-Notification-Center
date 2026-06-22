using System.ComponentModel.DataAnnotations;

namespace PresentationLayer1.Models;

public sealed record MockUser(string Id, string Email, string Role, string DisplayName);

public sealed record LoginResponse(string Token, MockUser User);

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

public sealed record EventUpsertRequest(
    [Required] string Title,
    [Required] string Description,
    [Required] string StartsAt,
    [Required] string EndsAt,
    [Range(1, int.MaxValue)] int Capacity,
    string? Location);

public sealed record LoginRequest([Required] string Email);

