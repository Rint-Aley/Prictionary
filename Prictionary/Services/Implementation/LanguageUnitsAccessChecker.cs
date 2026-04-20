using Microsoft.EntityFrameworkCore;
using Prictionary.Database;
using Prictionary.Models;
using Prictionary.Services.Interfaces;

namespace Prictionary.Services.Implementation;

public class LanguageUnitsAccessChecker : IAccessChecker<LanguageUnit>
{
    private readonly PrictionaryContext _dbContext;

    public LanguageUnitsAccessChecker(PrictionaryContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool CanChange(LanguageUnit resource, string userId)
        => IsOwner(resource, userId);

    public bool CanDelete(LanguageUnit resource, string userId)
        => IsOwner(resource, userId);

    public bool CanRead(LanguageUnit resource, string userId)
        => IsOwner(resource, userId);

    private bool IsOwner(LanguageUnit resource, string userId)
    {
        return _dbContext.LanguageUnitGroups
            .Include(lug => lug.Group)
            .Any(lug => lug.LanguageUnitId == resource.Id && lug.Group.UserId == userId);
    }
}
