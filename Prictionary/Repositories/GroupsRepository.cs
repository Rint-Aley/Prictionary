using Prictionary.Database;
using Prictionary.Repositories.Interfaces;

namespace Prictionary.Repositories;

public class GroupsRepository : IGroupsRepository
{
    private PrictionaryContext dbContext;
    public GroupsRepository(PrictionaryContext dbContext)
    {
        this.dbContext = dbContext;
    }

}
