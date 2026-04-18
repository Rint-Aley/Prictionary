using Prictionary.Services.Infrastructure;

namespace Prictionary.Repositories;

public record RepositoryResult<T> : Result<T, RepositoryError>
{

    public RepositoryResult(T value) : base(value) { }

    public RepositoryResult(RepositoryError error) : base(error) { }

    public RepositoryResult(QueryError error) 
        : base(new RepositoryError(error)) { }

    public RepositoryResult(QueryError error, string details) 
        : base(new RepositoryError(error, details)) { }
}

public record RepositoryError
{
    public QueryError Error { get; }
    public string? Details { get; }

    public RepositoryError(QueryError error)
    {
        Error = error;
        Details = null;
    }

    public RepositoryError(QueryError error, string details)
    {
        Error = error;
        Details = details;
    }
}

public enum QueryError
{
    NotFound = 0,
    ConstraintViolated = 1,
}
