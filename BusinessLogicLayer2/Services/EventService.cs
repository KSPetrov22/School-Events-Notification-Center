using BusinessLogicLayer2.Dtos;
using DataAccessLayer3.Repositories;

namespace BusinessLogicLayer2.Services;

public sealed class EventService(ISchoolEventsRepository repository) : IEventService
{
    public async Task<IReadOnlyList<EventSummary>> GetEventsAsync(UserInfo? user, CancellationToken cancellationToken = default) =>
        (await repository.GetEventsAsync(ToRecord(user), cancellationToken)).Select(ToDto).ToList();

    public async Task<EventSummary?> GetEventAsync(string id, UserInfo? user, CancellationToken cancellationToken = default)
    {
        var record = await repository.GetEventAsync(id, ToRecord(user), cancellationToken);
        return record is null ? null : ToDto(record);
    }

    public async Task<EventSummary?> CreateEventAsync(UserInfo user, EventUpsertRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsOrganizer(user))
        {
            return null;
        }

        return ToDto(await repository.CreateEventAsync(user.Id, ToData(request), cancellationToken));
    }

    public async Task<EventSummary?> UpdateEventAsync(string id, UserInfo user, EventUpsertRequest request, CancellationToken cancellationToken = default) =>
        !IsOrganizer(user)
            ? null
            : ToNullableDto(await repository.UpdateEventAsync(id, user.Id, ToData(request), cancellationToken));

    public async Task<EventSummary?> PublishEventAsync(string id, UserInfo user, CancellationToken cancellationToken = default) =>
        !IsOrganizer(user)
            ? null
            : ToNullableDto(await repository.SetEventStatusAsync(id, user.Id, "PUBLISHED", cancellationToken));

    public async Task<EventSummary?> CancelEventAsync(string id, UserInfo user, CancellationToken cancellationToken = default) =>
        !IsOrganizer(user)
            ? null
            : ToNullableDto(await repository.SetEventStatusAsync(id, user.Id, "CANCELLED", cancellationToken));

    public async Task<RegistrationSummary?> RegisterAsync(string eventId, UserInfo user, CancellationToken cancellationToken = default) =>
        !IsStudent(user)
            ? null
            : ToNullableDto(await repository.RegisterAsync(eventId, user.Id, cancellationToken));

    public Task<bool> CancelRegistrationAsync(string registrationId, UserInfo user, CancellationToken cancellationToken = default) =>
        IsStudent(user) ? repository.CancelRegistrationAsync(registrationId, user.Id, cancellationToken) : Task.FromResult(false);

    public async Task<IReadOnlyList<RegistrationSummary>> GetMyRegistrationsAsync(UserInfo user, CancellationToken cancellationToken = default) =>
        !IsStudent(user)
            ? []
            : (await repository.GetRegistrationsForUserAsync(user.Id, cancellationToken)).Select(ToDto).ToList();

    public async Task<IReadOnlyList<RegistrationSummary>> GetConfirmedRegistrationsAsync(string eventId, UserInfo user, CancellationToken cancellationToken = default) =>
        !IsOrganizer(user)
            ? []
            : (await repository.GetRegistrationsForEventAsync(eventId, user.Id, "CONFIRMED", cancellationToken)).Select(ToDto).ToList();

    public async Task<IReadOnlyList<RegistrationSummary>> GetWaitlistAsync(string eventId, UserInfo user, CancellationToken cancellationToken = default) =>
        !IsOrganizer(user)
            ? []
            : (await repository.GetRegistrationsForEventAsync(eventId, user.Id, "WAITLISTED", cancellationToken)).Select(ToDto).ToList();

    private static bool IsOrganizer(UserInfo user) =>
        string.Equals(user.Role, "ORGANIZER", StringComparison.OrdinalIgnoreCase);

    private static bool IsStudent(UserInfo user) =>
        string.Equals(user.Role, "STUDENT", StringComparison.OrdinalIgnoreCase);

    private static UserRecord? ToRecord(UserInfo? user) =>
        user is null ? null : new UserRecord(user.Id, user.Email, "", user.Role, user.DisplayName);

    private static EventUpsertData ToData(EventUpsertRequest request) =>
        new(request.Title, request.Description, request.StartsAt, request.EndsAt, request.Capacity, request.Location);

    private static EventSummary? ToNullableDto(EventRecord? record) =>
        record is null ? null : ToDto(record);

    private static EventSummary ToDto(EventRecord record) =>
        new(
            record.Id,
            record.OrganizerId,
            record.Title,
            record.Description,
            record.StartsAt,
            record.EndsAt,
            record.Capacity,
            record.Location,
            record.Status,
            record.ConfirmedCount,
            record.WaitlistCount,
            record.ConfirmedCount >= record.Capacity,
            record.CurrentUserRegistrationId,
            record.CurrentUserRegistrationStatus,
            record.CurrentUserWaitlistPosition);

    private static RegistrationSummary? ToNullableDto(RegistrationRecord? record) =>
        record is null ? null : ToDto(record);

    private static RegistrationSummary ToDto(RegistrationRecord record) =>
        new(
            record.Id,
            record.EventId,
            record.EventTitle,
            record.UserId,
            record.UserDisplayName,
            record.UserEmail,
            record.Status,
            record.RegisteredAt,
            record.WaitlistPosition);
}
