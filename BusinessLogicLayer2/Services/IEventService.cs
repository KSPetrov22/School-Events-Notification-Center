using BusinessLogicLayer2.Dtos;

namespace BusinessLogicLayer2.Services;

public interface IEventService
{
    Task<IReadOnlyList<EventSummary>> GetEventsAsync(UserInfo? user, CancellationToken cancellationToken = default);
    Task<EventSummary?> GetEventAsync(string id, UserInfo? user, CancellationToken cancellationToken = default);
    Task<EventSummary?> CreateEventAsync(UserInfo user, EventUpsertRequest request, CancellationToken cancellationToken = default);
    Task<EventSummary?> UpdateEventAsync(string id, UserInfo user, EventUpsertRequest request, CancellationToken cancellationToken = default);
    Task<EventSummary?> PublishEventAsync(string id, UserInfo user, CancellationToken cancellationToken = default);
    Task<EventSummary?> CancelEventAsync(string id, UserInfo user, CancellationToken cancellationToken = default);
    Task<RegistrationSummary?> RegisterAsync(string eventId, UserInfo user, CancellationToken cancellationToken = default);
    Task<bool> CancelRegistrationAsync(string registrationId, UserInfo user, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistrationSummary>> GetMyRegistrationsAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistrationSummary>> GetConfirmedRegistrationsAsync(string eventId, UserInfo user, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistrationSummary>> GetWaitlistAsync(string eventId, UserInfo user, CancellationToken cancellationToken = default);
}
