namespace Prictionary.Services.Infrastructure;

public sealed record Result<T>
{
    public bool Success { get; }
    public string? Error { get; }
    public T? Value { get; }

    public Result(string error)
    {
        Success = false;
        Error = error;
        Value = default;
    }

    public Result(T value)
    {
        Success = true;
        Error = null;
        Value = value;
    }

    public static implicit operator bool(Result<T> result) => result.Success;
}
