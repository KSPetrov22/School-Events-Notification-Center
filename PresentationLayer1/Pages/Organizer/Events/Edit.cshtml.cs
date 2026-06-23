using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Models;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages.Organizer.Events;

public sealed class EditModel(IApiClient api) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    [BindProperty]
    public EventUpsertRequest Input { get; set; } = new()
    {
        StartsAt = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ssZ"),
        EndsAt = DateTime.UtcNow.AddDays(7).AddHours(2).ToString("yyyy-MM-ddTHH:mm:ssZ"),
        Capacity = 20
    };

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (Id is null)
        {
            return Page();
        }

        var existing = await api.GetEventAsync(Id, cancellationToken);
        if (existing is null)
        {
            TempData["Error"] = "Event not found or cannot be edited.";
            return RedirectToPage("Index");
        }

        Input = new EventUpsertRequest
        {
            Title = existing.Title,
            Description = existing.Description,
            StartsAt = existing.StartsAt,
            EndsAt = existing.EndsAt,
            Capacity = existing.Capacity,
            Location = existing.Location
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Id is null)
        {
            await api.CreateEventAsync(Input, cancellationToken);
            TempData["Message"] = "Draft event created.";
        }
        else
        {
            await api.UpdateEventAsync(Id, Input, cancellationToken);
            TempData["Message"] = "Draft event saved.";
        }

        return RedirectToPage("/Organizer/Events/Index");
    }
}
