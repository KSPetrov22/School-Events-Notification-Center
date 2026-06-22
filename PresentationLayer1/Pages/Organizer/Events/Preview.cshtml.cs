using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Models;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages.Organizer.Events;

public sealed class PreviewModel(IMockApiClient api) : PageModel
{
    public EventSummary? Event { get; private set; }

    public async Task OnGetAsync(string id, CancellationToken cancellationToken)
    {
        Event = await api.GetEventAsync(id, cancellationToken);
    }
}

