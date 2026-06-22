using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Models;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages.Organizer.Events;

public sealed class RegistrationsModel(IMockApiClient api) : PageModel
{
    public EventSummary? Event { get; private set; }
    public IReadOnlyList<RegistrationSummary> Confirmed { get; private set; } = [];
    public IReadOnlyList<RegistrationSummary> Waitlist { get; private set; } = [];

    public async Task OnGetAsync(string id, CancellationToken cancellationToken)
    {
        Event = await api.GetEventAsync(id, cancellationToken);
        Confirmed = await api.GetConfirmedRegistrationsAsync(id, cancellationToken);
        Waitlist = await api.GetWaitlistAsync(id, cancellationToken);
    }
}

