using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Models;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages.Events;

public sealed class DetailsModel(IApiClient api, IAuthSession auth) : PageModel
{
    public EventSummary? Event { get; private set; }
    public IAuthSession Auth => auth;

    public async Task OnGetAsync(string id, CancellationToken cancellationToken)
    {
        Event = await api.GetEventAsync(id, cancellationToken);
    }

    public async Task<IActionResult> OnPostRegisterAsync(string id, CancellationToken cancellationToken)
    {
        if (!auth.IsStudent)
        {
            TempData["Error"] = "Sign in as a student to register.";
            return RedirectToPage("/Login");
        }

        await api.RegisterAsync(id, cancellationToken);
        TempData["Message"] = "Registration updated.";
        return RedirectToPage("/Events/Details", new { id });
    }

    public async Task<IActionResult> OnPostCancelAsync(string id, string registrationId, CancellationToken cancellationToken)
    {
        if (!auth.IsStudent)
        {
            TempData["Error"] = "Sign in as a student to cancel registrations.";
            return RedirectToPage("/Login");
        }

        await api.CancelRegistrationAsync(registrationId, cancellationToken);
        TempData["Message"] = "Registration cancelled.";
        return RedirectToPage("/Events/Details", new { id });
    }
}
