using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Models;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages.Organizer.Events;

public sealed class PreviewModel(IApiClient api, IAuthSession auth) : PageModel
{
    public EventSummary? Event { get; private set; }

    public async Task<IActionResult> OnGetAsync(string id, CancellationToken cancellationToken)
    {
        if (!auth.IsOrganizer)
        {
            TempData["Error"] = "Sign in as an organizer to preview events.";
            return RedirectToPage("/Login");
        }

        Event = await api.GetEventAsync(id, cancellationToken);
        return Page();
    }
}
