using DataAccessLayer3;
using BusinessLogicLayer2.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicLayer2;

// Composition root for the Business layer. Hosts call ONLY this — it pulls in
// the Data layer, so Presentation/Worker never reference DataAccessLayer3.
public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLayer(
        this IServiceCollection services, string databaseUrl)
    {
        services.AddDataLayer(databaseUrl);
        services.AddScoped<IApplicationInitializer, ApplicationInitializer>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }
}
