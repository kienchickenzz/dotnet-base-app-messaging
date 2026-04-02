using System.Diagnostics.CodeAnalysis;

namespace BaseAppMessaging.Domain.Primitives;

/// <summary>
/// Represents the outcome of an operation that can either succeed or fail.
/// </summary>
/// <remarks>
/// Implements the Result pattern to handle failures without exceptions,
/// making error handling explicit and composable.
/// </remarks>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation succeeded.</param>
    /// <param name="error">The error details if the operation failed.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when success is true but error is not <see cref="Error.None"/>,
    /// or when success is false but error is <see cref="Error.None"/>.
    /// </exception>
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error details. Returns <see cref="Error.None"/> if the operation succeeded.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="Result"/>.</returns>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A successful <see cref="Result{TValue}"/> containing the value.</returns>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <typeparam name="TValue">The type of the expected value.</typeparam>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed <see cref="Result{TValue}"/>.</returns>
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    /// <summary>
    /// Creates a result from a nullable value. Returns success if the value is not null,
    /// otherwise returns a failure with <see cref="Error.NullValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The nullable value to wrap.</param>
    /// <returns>A <see cref="Result{TValue}"/> based on whether the value is null.</returns>
    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}

/// <summary>
/// Represents the outcome of an operation that returns a value on success.
/// </summary>
/// <typeparam name="TValue">The type of the value returned on success.</typeparam>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="value">The value if the operation succeeded.</param>
    /// <param name="isSuccess">Indicates whether the operation succeeded.</param>
    /// <param name="error">The error details if the operation failed.</param>
    public Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value of a successful result.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessing the value of a failed result.</exception>
    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    /// <summary>
    /// Implicitly converts a value to a successful result, or a failure if the value is null.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}
