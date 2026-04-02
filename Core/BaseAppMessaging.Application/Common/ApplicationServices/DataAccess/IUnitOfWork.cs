namespace BaseAppMessaging.Application.Common.ApplicationServices.DataAccess;


/// <summary>
/// Defines the contract for the Unit of Work pattern.
/// </summary>
/// <remarks>
/// Coordinates the work of multiple repositories by creating a single database context
/// and ensuring all changes are committed as a single transaction.
/// </remarks>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Persists all changes made in this unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
