using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages;

public sealed class LoginModel(IMockApiClient api, IAuthSession auth) : PageModel
{
    [BindProperty]
    public string Email { get; set; } = "student1@school.local";

    public void OnGet()
    {
    }

    public IActionResult OnGetLogout()
    {
        auth.SignOut();
        TempData["Message"] = "Signed out.";
        return RedirectToPage("/Login");
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var login = await api.LoginAsync(Email, cancellationToken);
        if (login is null)
        {
            ModelState.AddModelError(string.Empty, "Mock user was not found.");
            return Page();
        }

        auth.SignIn(login);
        TempData["Message"] = $"Signed in as {login.User.DisplayName}.";
        return string.Equals(login.User.Role, "ORGANIZER", StringComparison.OrdinalIgnoreCase)
            ? RedirectToPage("/Organizer/Events/Index")
            : RedirectToPage("/Events/Index");
    }
}

