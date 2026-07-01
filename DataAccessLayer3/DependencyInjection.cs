using DataAccessLayer3.Db;
using DataAccessLayer3.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer3;

// Composition root for the Data layer. BusinessLogicLayer2 calls this; the
// hosts never reference this project directly.
public static class DependencyInjection
{
    public static IServiceCollection AddDataLayer(
        this IServiceCollection services, string databaseUrl)
    {
        EnsureSqliteDirectoryExists(databaseUrl);

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(databaseUrl));
        services.AddScoped<DatabaseInitializer>();
        services.AddScoped<ISchoolEventsRepository, SchoolEventsRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        return services;
    }

    private static void EnsureSqliteDirectoryExists(string databaseUrl)
    {
        var builder = new SqliteConnectionStringBuilder(databaseUrl);
        if (string.IsNullOrWhiteSpace(builder.DataSource) || builder.DataSource == ":memory:")
        {
            return;
        }

        var directory = Path.GetDirectoryName(Path.GetFullPath(builder.DataSource));
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
