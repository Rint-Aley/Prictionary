using Prictionary.Services.Interfaces;
using Prictionary.Models;

namespace Prictionary.Services.Implementation;

public class GroupAccessChecker : IAccessChecker<Group>
{
    public bool CanChange(Group resource, string userId)
    {
        return resource.UserId == userId;
    }

    public bool CanDelete(Group resource, string userId)
    {
        return resource.UserId == userId;
    }

    public bool CanRead(Group resource, string userId)
    {
        return resource.UserId == userId;
    }
}
