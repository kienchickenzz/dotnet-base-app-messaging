using BaseAppMessaging.Domain.Abstractions;

namespace BaseAppMessaging.Domain.Primitives;

/// <summary>
/// Base class for all domain entities.
/// </summary>
/// <remarks>
/// Provides common properties for identity, auditing, and soft delete functionality.
/// All entities in the domain should inherit from this class.
/// </remarks>
public abstract class Entity : IAuditableEntity, ISoftDelete
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    public Guid? LastModifiedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    public DateTime? LastModifiedOn { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was soft deleted.
    /// </summary>
    public DateTime? DeletedOn { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// </summary>
    public Guid? DeletedBy { get; set; }
}
