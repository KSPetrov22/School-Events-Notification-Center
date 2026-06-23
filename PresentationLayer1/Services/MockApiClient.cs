using System.Net.Http.Json;
using PresentationLayer1.Models;

namespace PresentationLayer1.Services;

public interface IMockApiClient
{
    Task<LoginResponse?> LoginAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventSummary>> GetEventsAsync(CancellationToken cancellationToken = default);
    Task<EventSummary?> GetEventAsync(string id, CancellationToken cancellationToken = default);
    Task<EventSummary?> CreateEventAsync(EventUpsertRequest request, CancellationToken cancellationToken = default);
    Task<EventSummary?> UpdateEventAsync(string id, EventUpsertRequest request, CancellationToken cancellationToken = default);
    Task<EventSummary?> PublishEventAsync(string id, CancellationToken cancellationToken = default);
    Task<EventSummary?> CancelEventAsync(string id, CancellationToken cancellationToken = default);
    Task<RegistrationSummary?> RegisterAsync(string eventId, CancellationToken cancellationToken = default);
    Task CancelRegistrationAsync(string registrationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistrationSummary>> GetMyRegistrationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistrationSummary>> GetConfirmedRegistrationsAsync(string eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistrationSummary>> GetWaitlistAsync(string eventId, CancellationToken cancellationToken = default);
}

public sealed class MockApiClient(HttpClient httpClient, IAuthSession authSession) : IMockApiClient
{
    public Task<LoginResponse?> LoginAsync(string email, CancellationToken cancellationToken = default) =>
        PostAsync<LoginRequest, LoginResponse>("login", new LoginRequest(email), cancellationToken);

    public Task<IReadOnlyList<EventSummary>> GetEventsAsync(CancellationToken cancellationToken = default) =>
        GetListAsync<EventSummary>("events", cancellationToken);

    public Task<EventSummary?> GetEventAsync(string id, CancellationToken cancellationToken = default) =>
        GetAsync<EventSummary>($"events/{Uri.EscapeDataString(id)}", cancellationToken);

    public Task<EventSummary?> CreateEventAsync(EventUpsertRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<EventUpsertRequest, EventSummary>("events", request, cancellationToken);

    public Task<EventSummary?> UpdateEventAsync(string id, EventUpsertRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<EventUpsertRequest, EventSummary>($"events/{Uri.EscapeDataString(id)}", request, cancellationToken);

    public Task<EventSummary?> PublishEventAsync(string id, CancellationToken cancellationToken = default) =>
        PostAsync<object, EventSummary>($"events/{Uri.EscapeDataString(id)}/publish", new { }, cancellationToken);

    public Task<EventSummary?> CancelEventAsync(string id, CancellationToken cancellationToken = default) =>
        PostAsync<object, EventSummary>($"events/{Uri.EscapeDataString(id)}/cancel", new { }, cancellationToken);

    public Task<RegistrationSummary?> RegisterAsync(string eventId, CancellationToken cancellationToken = default) =>
        PostAsync<object, RegistrationSummary>($"events/{Uri.EscapeDataString(eventId)}/registrations", new { }, cancellationToken);

    public async Task CancelRegistrationAsync(string registrationId, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Delete, $"registrations/{Uri.EscapeDataString(registrationId)}");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public Task<IReadOnlyList<RegistrationSummary>> GetMyRegistrationsAsync(CancellationToken cancellationToken = default) =>
        GetListAsync<RegistrationSummary>("registrations/me", cancellationToken);

    public Task<IReadOnlyList<RegistrationSummary>> GetConfirmedRegistrationsAsync(string eventId, CancellationToken cancellationToken = default) =>
        GetListAsync<RegistrationSummary>($"events/{Uri.EscapeDataString(eventId)}/registrations", cancellationToken);

    public Task<IReadOnlyList<RegistrationSummary>> GetWaitlistAsync(string eventId, CancellationToken cancellationToken = default) =>
        GetListAsync<RegistrationSummary>($"events/{Uri.EscapeDataString(eventId)}/waitlist", cancellationToken);

    private async Task<IReadOnlyList<T>> GetListAsync<T>(string path, CancellationToken cancellationToken)
    {
        var value = await GetAsync<List<T>>(path, cancellationToken);
        return value ?? [];
    }

    private async Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var request = NewRequest(HttpMethod.Get, path);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    private async Task<TResponse?> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken)
    {
        using var request = NewRequest(HttpMethod.Post, path);
        request.Content = JsonContent.Create(body);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }

    private async Task<TResponse?> PutAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken)
    {
        using var request = NewRequest(HttpMethod.Put, path);
        request.Content = JsonContent.Create(body);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }

    private HttpRequestMessage NewRequest(HttpMethod method, string path)
    {
        var request = new HttpRequestMessage(method, path);
        if (authSession.CurrentUser is { } user)
        {
            request.Headers.Add("X-Mock-User-Id", user.Id);
        }

        if (!string.IsNullOrWhiteSpace(authSession.Token))
        {
            request.Headers.Add("Authorization", $"Bearer {authSession.Token}");
        }

        return request;
    }
}

