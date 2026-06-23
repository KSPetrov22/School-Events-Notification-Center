using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Models;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages.Events;

public sealed class DetailsModel(IMockApiClient api, IAuthSession auth) : PageModel
{
    public EventSummary? Event { get; private set; }
    public IAuthSession Auth => auth;

    public async Task OnGetAsync(string id, CancellationToken cancellationToken)
    {
        Event = await api.GetEventAsync(id, cancellationToken);
    }

    public async Task<IActionResult> OnPostRegisterAsync(string id, CancellationToken cancellationToken)
    {
        await api.RegisterAsync(id, cancellationToken);
        TempData["Message"] = "Registration updated.";
        return RedirectToPage("/Events/Details", new { id });
    }

    public async Task<IActionResult> OnPostCancelAsync(string id, string registrationId, CancellationToken cancellationToken)
    {
        await api.CancelRegistrationAsync(registrationId, cancellationToken);
        TempData["Message"] = "Registration cancelled.";
        return RedirectToPage("/Events/Details", new { id });
    }
}
