namespace Prictionary.Services.Interfaces;

/// <summary>
/// Class for validating 
/// </summary>
/// <typeparam name="T">Type</typeparam>
public interface IAccessChecker<T>
{
    public Task<bool> CanReadAsync(T resource, string userId, CancellationToken cancellationToken = default);

    public Task<bool> CanChangeAsync(T resource, string userId, CancellationToken cancellationToken = default);

    public Task<bool> CanDeleteAsync(T resource, string userId, CancellationToken cancellationToken = default);
}