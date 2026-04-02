namespace BaseAppMessaging.Application.Common.ApplicationServices.Messaging;

/// <summary>
/// Contains metadata for message tracing, correlation, and versioning.
/// </summary>
public class MetaData
{
    /// <summary>
    /// Gets or sets the unique identifier for the message.
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Gets or sets the version of the message schema.
    /// </summary>
    public string? MessageVersion { get; set; }

    /// <summary>
    /// Gets or sets the activity/correlation ID for distributed tracing.
    /// </summary>
    public string? ActivityId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the message was created.
    /// </summary>
    public DateTimeOffset? CreationDateTime { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the message was enqueued.
    /// </summary>
    public DateTimeOffset? EnqueuedDateTime { get; set; }
}
