using Prictionary.Services.Interfaces;
using Prictionary.Models;

namespace Prictionary.Services.Implementation.AccessCheckers;

public class GroupAccessChecker : IAccessChecker<Group>
{
    public async Task<bool> CanChangeAsync(
        Group resource,
        string userId,
        CancellationToken cancellationToken)
        => IsOwner(resource, userId);

    public async Task<bool> CanDeleteAsync(
        Group resource, 
        string userId,
        CancellationToken cancellationToken)
        => IsOwner(resource, userId);

    public async Task<bool> CanReadAsync(
        Group resource, 
        string userId,
        CancellationToken cancellationToken)
        => IsOwner(resource, userId);

    private bool IsOwner(Group resource, string userId) => resource.UserId == userId;
}
