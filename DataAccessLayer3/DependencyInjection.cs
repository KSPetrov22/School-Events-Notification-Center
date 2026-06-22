using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer3;

// Composition root for the Data layer. BusinessLogicLayer2 calls this; the
// hosts never reference this project directly.
public static class DependencyInjection
{
    public static IServiceCollection AddDataLayer(
        this IServiceCollection services, string databaseUrl)
    {
        // TODO: register AppDbContext + repositories here.
        // SQLite (local): options.UseSqlite(databaseUrl).UseSnakeCaseNamingConvention();
        // PostgreSQL (prod): options.UseNpgsql(databaseUrl).UseSnakeCaseNamingConvention();
        return services;
    }
}
