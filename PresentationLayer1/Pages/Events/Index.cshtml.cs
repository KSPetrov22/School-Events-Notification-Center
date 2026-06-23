using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Models;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages.Events;

public sealed class IndexModel(IApiClient api) : PageModel
{
    public IReadOnlyList<EventSummary> Events { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Events = await api.GetEventsAsync(cancellationToken);
    }
}

