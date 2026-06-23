using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Models;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages.Organizer.Events;

public sealed class IndexModel(IApiClient api) : PageModel
{
    public IReadOnlyList<EventSummary> Events { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Events = await api.GetEventsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostPublishAsync(string id, CancellationToken cancellationToken)
    {
        await api.PublishEventAsync(id, cancellationToken);
        TempData["Message"] = "Event published.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelAsync(string id, CancellationToken cancellationToken)
    {
        await api.CancelEventAsync(id, cancellationToken);
        TempData["Message"] = "Event cancelled.";
        return RedirectToPage();
    }
}

