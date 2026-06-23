using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Models;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages.Registrations;

public sealed class MeModel(IApiClient api) : PageModel
{
    public IReadOnlyList<RegistrationSummary> Registrations { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Registrations = await api.GetMyRegistrationsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostCancelAsync(string registrationId, CancellationToken cancellationToken)
    {
        await api.CancelRegistrationAsync(registrationId, cancellationToken);
        TempData["Message"] = "Registration cancelled.";
        return RedirectToPage();
    }
}

