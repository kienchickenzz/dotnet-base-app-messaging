namespace BaseAppMessaging.Domain.Primitives;

/// <summary>
/// Represents a domain error with a code and descriptive message.
/// </summary>
/// <remarks>
/// Used in conjunction with <see cref="Result"/> to provide detailed error information
/// without throwing exceptions for expected failure scenarios.
/// </remarks>
/// <param name="Code">The unique error code for programmatic identification.</param>
/// <param name="Name">The human-readable error description.</param>
public record Error(string Code, string Name)
{
    /// <summary>
    /// Represents no error. Used for successful results.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Represents an error when a null value was provided where a value was expected.
    /// </summary>
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided");
}
