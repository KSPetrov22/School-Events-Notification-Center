using DataAccessLayer3;
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
        // TODO: register services (IEventService, INotificationService, ...) here.
        return services;
    }
}
