using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer1.Services;

namespace PresentationLayer1.Pages;

public sealed class LoginModel(IMockApiClient api, IAuthSession auth, IConfiguration config) : PageModel
{
    [BindProperty]
    public string Email { get; set; } = "student1@school.local";

    // Collected in real-login mode. Ignored by the mock API; wire to real auth when backend is ready.
    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public bool IsMockLogin { get; private set; }

    public void OnGet()
    {
        IsMockLogin = config.GetValue<bool>("MOCK_LOGIN", false);
    }

    public IActionResult OnGetLogout()
    {
        auth.SignOut();
        TempData["Message"] = "Signed out.";
        return RedirectToPage("/Login");
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        IsMockLogin = config.GetValue<bool>("MOCK_LOGIN", false);

        var login = await api.LoginAsync(Email, cancellationToken);
        if (login is null)
        {
            ModelState.AddModelError(string.Empty, IsMockLogin ? "Mock user not found." : "Invalid email or password.");
            return Page();
        }

        auth.SignIn(login);
        TempData["Message"] = $"Signed in as {login.User.DisplayName}.";
        return string.Equals(login.User.Role, "ORGANIZER", StringComparison.OrdinalIgnoreCase)
            ? RedirectToPage("/Organizer/Events/Index")
            : RedirectToPage("/Events/Index");
    }
}
