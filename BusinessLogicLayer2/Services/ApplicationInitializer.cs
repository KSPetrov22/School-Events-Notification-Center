using DataAccessLayer3.Db;

namespace BusinessLogicLayer2.Services;

public sealed class ApplicationInitializer(DatabaseInitializer databaseInitializer) : IApplicationInitializer
{
    public Task EnsureReadyAsync(CancellationToken cancellationToken = default) =>
        databaseInitializer.EnsureCreatedAndSeededAsync(cancellationToken);
}
