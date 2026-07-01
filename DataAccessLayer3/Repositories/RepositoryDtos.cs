namespace DataAccessLayer3.Repositories;

public sealed record UserRecord(string Id, string Email, string PasswordHash, string Role, string DisplayName);

public sealed record EventRecord(
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
    string? CurrentUserRegistrationId,
    string? CurrentUserRegistrationStatus,
    int? CurrentUserWaitlistPosition);

public sealed record RegistrationRecord(
    string Id,
    string EventId,
    string EventTitle,
    string UserId,
    string UserDisplayName,
    string UserEmail,
    string Status,
    string RegisteredAt,
    int? WaitlistPosition);

public sealed record EventUpsertData(
    string Title,
    string Description,
    string StartsAt,
    string EndsAt,
    int Capacity,
    string? Location);
