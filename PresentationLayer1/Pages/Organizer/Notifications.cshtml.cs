using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Models;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages.Organizer;

public sealed class NotificationsModel(IApiClient api, IAuthSession auth) : PageModel
{
    public IReadOnlyList<NotificationLogSummary> Logs { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (!auth.IsOrganizer)
        {
            TempData["Error"] = "Sign in as an organizer to view notifications.";
            return RedirectToPage("/Login");
        }

        Logs = await api.GetNotificationLogsAsync(cancellationToken);
        return Page();
    }
}
