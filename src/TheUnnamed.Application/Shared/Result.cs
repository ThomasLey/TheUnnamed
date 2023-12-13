namespace TheUnnamed.Application.Shared;

public class Result
{
    protected Result(Error? error)
    {
        Error = error;
    }

    public Error? Error { get; init; }
    public bool IsSuccess => Error is null;
    public bool IsFailure => Error is not null;

    public static Result Success() => new(null);
    public static Result Failure(Error error) => new(error);
    public static Result<TValue> Success<TValue>(TValue entity) => Result<TValue>.Create(entity, null);
    public static Result<TValue> Failure<TValue>(Error error) => Result<TValue>.Create(default, error);
}

public sealed class Result<TValue> : Result
{
    private Result(TValue? value, Error? error) : base(error)
    {
        Value = value;
    }

    public TValue? Value { get; init; }

    public static Result<TValue> Create(TValue? value, Error? error)
    {
        if (value is null && error is null) throw new InvalidOperationException("The value and the error cannot be null at the same time. Use non-generic base class instead.");
        if (value is not null && error is not null) throw new InvalidOperationException("The value and the error cannot have values at the same time. Either it is an error or a success.");

        return new Result<TValue>(value, error);
    }
}
