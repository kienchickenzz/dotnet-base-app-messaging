namespace BaseAppMessaging.Domain.Abstractions;

/// <summary>
/// Defines a contract for entities that track audit information.
/// </summary>
/// <remarks>
/// Audit information includes creation and modification timestamps along with user identifiers,
/// enabling tracking of who made changes and when.
/// </remarks>
public interface IAuditableEntity : ISoftDelete
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    Guid CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    /// <value>The modifying user's identifier, or <c>null</c> if the entity has never been modified.</value>
    Guid? LastModifiedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    /// <value>The modification timestamp, or <c>null</c> if the entity has never been modified.</value>
    DateTime? LastModifiedOn { get; set; }
}
