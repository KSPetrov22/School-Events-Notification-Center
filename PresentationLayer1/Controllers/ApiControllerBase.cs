using System.Security.Claims;
using BusinessLogicLayer2.Dtos;
using BusinessLogicLayer2.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer1.Controllers;

public abstract class ApiControllerBase(IAuthService authService) : ControllerBase
{
    protected async Task<UserInfo?> CurrentUserAsync(CancellationToken cancellationToken)
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return sub is null ? null : await authService.GetUserAsync(sub, cancellationToken);
    }

    protected async Task<ActionResult<UserInfo>> RequireRoleAsync(string role, CancellationToken cancellationToken)
    {
        var user = await CurrentUserAsync(cancellationToken);
        if (user is null)
        {
            return Unauthorized();
        }

        return string.Equals(user.Role, role, StringComparison.OrdinalIgnoreCase)
            ? user
            : StatusCode(StatusCodes.Status403Forbidden);
    }

}
