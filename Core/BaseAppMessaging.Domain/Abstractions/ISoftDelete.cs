namespace BaseAppMessaging.Domain.Abstractions;

/// <summary>
/// Defines a contract for entities that support soft deletion.
/// </summary>
/// <remarks>
/// Soft delete allows records to be marked as deleted without physically removing them from the database,
/// enabling data recovery and maintaining referential integrity.
/// </remarks>
public interface ISoftDelete
{
    /// <summary>
    /// Gets or sets the date and time when the entity was deleted.
    /// </summary>
    /// <value>The deletion timestamp, or <c>null</c> if the entity has not been deleted.</value>
    DateTime? DeletedOn { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// </summary>
    /// <value>The deleting user's identifier, or <c>null</c> if the entity has not been deleted.</value>
    Guid? DeletedBy { get; set; }
}
