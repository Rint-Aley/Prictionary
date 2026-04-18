namespace Prictionary.Services.Interfaces;

/// <summary>
/// Class for validating 
/// </summary>
/// <typeparam name="T">Type</typeparam>
public interface IAccessChecker<T>
{
    public bool CanRead(T resource, string userId);

    public bool CanChange(T resource, string userId);

    public bool CanDelete(T resource, string userId);
}
