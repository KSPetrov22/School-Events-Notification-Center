using BusinessLogicLayer2.Dtos;
using BusinessLogicLayer2.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer1.Controllers;

[ApiController]
[Route("api/notifications")]
public sealed class NotificationsController(
    INotificationService notifications,
    IAuthService authService) : ApiControllerBase(authService)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationLogSummary>>> GetRecent(CancellationToken cancellationToken)
    {
        var user = await RequireRoleAsync("ORGANIZER", cancellationToken);
        if (user.Result is not null)
        {
            return user.Result;
        }

        return Ok(await notifications.GetRecentLogsAsync(100, cancellationToken));
    }
}
