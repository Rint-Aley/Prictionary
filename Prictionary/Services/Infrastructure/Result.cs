namespace Prictionary.Services.Infrastructure;

public record Result<T, E>
{
    public bool Success { get; }
    public E? Error { get; }
    public T? Value { get; }

    public Result(E error)
    {
        Success = false;
        Error = error;
        Value = default;
    }

    public Result(T value)
    {
        Success = true;
        Error = default;
        Value = value;
    }

    public bool IsError(out E error)
    {
        if (!Success)
        {
            error = Error!;
            return true;
        }
        error = default!;
        return false;
    }

    public static implicit operator bool(Result<T, E> result) => result.Success;
}
