using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Models;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages.Organizer.Events;

public sealed class RegistrationsModel(IApiClient api, IAuthSession auth) : PageModel
{
    public EventSummary? Event { get; private set; }
    public IReadOnlyList<RegistrationSummary> Confirmed { get; private set; } = [];
    public IReadOnlyList<RegistrationSummary> Waitlist { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(string id, CancellationToken cancellationToken)
    {
        if (!auth.IsOrganizer)
        {
            TempData["Error"] = "Sign in as an organizer to view signups.";
            return RedirectToPage("/Login");
        }

        Event = await api.GetEventAsync(id, cancellationToken);
        Confirmed = await api.GetConfirmedRegistrationsAsync(id, cancellationToken);
        Waitlist = await api.GetWaitlistAsync(id, cancellationToken);
        return Page();
    }
}
