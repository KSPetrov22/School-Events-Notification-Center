using BusinessLogicLayer2.Dtos;
using BusinessLogicLayer2.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer1.Controllers;

[ApiController]
[Route("api")]
public sealed class EventsController(IEventService eventService, IAuthService authService) : ApiControllerBase(authService)
{
    [HttpGet("events")]
    public async Task<IReadOnlyList<EventSummary>> GetEvents(CancellationToken cancellationToken) =>
        await eventService.GetEventsAsync(await CurrentUserAsync(cancellationToken), cancellationToken);

    [HttpGet("events/{id}")]
    public async Task<ActionResult<EventSummary>> GetEvent(string id, CancellationToken cancellationToken)
    {
        var evt = await eventService.GetEventAsync(id, await CurrentUserAsync(cancellationToken), cancellationToken);
        return evt is null ? NotFound() : evt;
    }

    [HttpPost("events")]
    public async Task<ActionResult<EventSummary>> CreateEvent(EventUpsertRequest request, CancellationToken cancellationToken)
    {
        var user = await RequireRoleAsync("ORGANIZER", cancellationToken);
        if (user.Result is not null)
        {
            return user.Result;
        }

        var evt = await eventService.CreateEventAsync(user.Value!, request, cancellationToken);
        return evt is null ? Forbid() : evt;
    }

    [HttpPut("events/{id}")]
    public async Task<ActionResult<EventSummary>> UpdateEvent(string id, EventUpsertRequest request, CancellationToken cancellationToken)
    {
        var user = await RequireRoleAsync("ORGANIZER", cancellationToken);
        if (user.Result is not null)
        {
            return user.Result;
        }

        var evt = await eventService.UpdateEventAsync(id, user.Value!, request, cancellationToken);
        return evt is null ? NotFound() : evt;
    }

    [HttpPost("events/{id}/publish")]
    public async Task<ActionResult<EventSummary>> PublishEvent(string id, CancellationToken cancellationToken)
    {
        var user = await RequireRoleAsync("ORGANIZER", cancellationToken);
        if (user.Result is not null)
        {
            return user.Result;
        }

        var evt = await eventService.PublishEventAsync(id, user.Value!, cancellationToken);
        return evt is null ? NotFound() : evt;
    }

    [HttpPost("events/{id}/cancel")]
    public async Task<ActionResult<EventSummary>> CancelEvent(string id, CancellationToken cancellationToken)
    {
        var user = await RequireRoleAsync("ORGANIZER", cancellationToken);
        if (user.Result is not null)
        {
            return user.Result;
        }

        var evt = await eventService.CancelEventAsync(id, user.Value!, cancellationToken);
        return evt is null ? NotFound() : evt;
    }

    [HttpPost("events/{id}/registrations")]
    public async Task<ActionResult<RegistrationSummary>> Register(string id, CancellationToken cancellationToken)
    {
        var user = await RequireRoleAsync("STUDENT", cancellationToken);
        if (user.Result is not null)
        {
            return user.Result;
        }

        var registration = await eventService.RegisterAsync(id, user.Value!, cancellationToken);
        return registration is null ? BadRequest(new { error = "Registration could not be created." }) : registration;
    }

    [HttpDelete("registrations/{id}")]
    public async Task<IActionResult> CancelRegistration(string id, CancellationToken cancellationToken)
    {
        var user = await RequireRoleAsync("STUDENT", cancellationToken);
        if (user.Result is not null)
        {
            return user.Result;
        }

        return await eventService.CancelRegistrationAsync(id, user.Value!, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpGet("registrations/me")]
    public async Task<ActionResult<IReadOnlyList<RegistrationSummary>>> GetMyRegistrations(CancellationToken cancellationToken)
    {
        var user = await RequireRoleAsync("STUDENT", cancellationToken);
        if (user.Result is not null)
        {
            return user.Result;
        }

        return Ok(await eventService.GetMyRegistrationsAsync(user.Value!, cancellationToken));
    }

    [HttpGet("events/{id}/registrations")]
    public async Task<ActionResult<IReadOnlyList<RegistrationSummary>>> GetConfirmedRegistrations(string id, CancellationToken cancellationToken)
    {
        var user = await RequireRoleAsync("ORGANIZER", cancellationToken);
        if (user.Result is not null)
        {
            return user.Result;
        }

        return Ok(await eventService.GetConfirmedRegistrationsAsync(id, user.Value!, cancellationToken));
    }

    [HttpGet("events/{id}/waitlist")]
    public async Task<ActionResult<IReadOnlyList<RegistrationSummary>>> GetWaitlist(string id, CancellationToken cancellationToken)
    {
        var user = await RequireRoleAsync("ORGANIZER", cancellationToken);
        if (user.Result is not null)
        {
            return user.Result;
        }

        return Ok(await eventService.GetWaitlistAsync(id, user.Value!, cancellationToken));
    }
}
