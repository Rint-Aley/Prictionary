using Microsoft.EntityFrameworkCore;
using Prictionary.Database;
using Prictionary.Models;
using Prictionary.Services.Interfaces;

namespace Prictionary.Services.Implementation.AccessCheckers;

public class LanguageUnitsAccessChecker : IAccessChecker<LanguageUnit>
{
    private readonly PrictionaryContext _dbContext;

    public LanguageUnitsAccessChecker(PrictionaryContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> CanChangeAsync(
        LanguageUnit resource,
        string userId,
        CancellationToken cancellationToken)
        => IsOwner(resource, userId, cancellationToken);

    public Task<bool> CanDeleteAsync(
        LanguageUnit resource,
        string userId,
        CancellationToken cancellationToken)
        => IsOwner(resource, userId, cancellationToken);

    public Task<bool> CanReadAsync(
        LanguageUnit resource,
        string userId,
        CancellationToken cancellationToken)
        => IsOwner(resource, userId, cancellationToken);

    private Task<bool> IsOwner(
        LanguageUnit resource,
        string userId,
        CancellationToken cancellationToken)
    {
        return _dbContext.LanguageUnitGroups
            .Include(lug => lug.Group)
            .AnyAsync(lug => lug.LanguageUnitId == resource.Id && lug.Group.UserId == userId, cancellationToken);
    }
}
