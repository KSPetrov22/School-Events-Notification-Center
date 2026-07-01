namespace BusinessLogicLayer2.Services;

public interface IApplicationInitializer
{
    Task EnsureReadyAsync(CancellationToken cancellationToken = default);
}
