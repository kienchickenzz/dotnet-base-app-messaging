namespace BaseAppMessaging.Application.Common.ApplicationServices.Repositories;

using System.Linq.Expressions;

using BaseAppMessaging.Domain.Primitives;


/// <summary>
/// Generic repository interface for basic CRUD operations.
/// </summary>
/// <remarks>
/// Exposes IQueryable for flexible LINQ queries in handlers.
/// Infrastructure layer implements this with EF Core DbContext.
/// </remarks>
/// <typeparam name="TEntity">The entity type that inherits from BaseEntity.</typeparam>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    // === READ ===

    /// <summary>
    /// Gets the queryable collection for composing LINQ queries.
    /// </summary>
    IQueryable<TEntity> Query { get; }

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // === WRITE ===

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The added entity.</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities to the repository.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity as modified.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity from the repository (hard delete).
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity as deleted without removing it (soft delete).
    /// </summary>
    /// <param name="entity">The entity to soft delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    // === UTILITIES ===

    /// <summary>
    /// Counts entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter condition. If null, counts all entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of matching entities.</returns>
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity matches the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter condition.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if at least one entity matches; otherwise, false.</returns>
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}
