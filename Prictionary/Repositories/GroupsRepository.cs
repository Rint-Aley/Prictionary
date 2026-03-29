using Prictionary.Database;

namespace Prictionary.Repositories;

public class GroupsRepository
{
    private PrictionaryContext dbContext;
    public GroupsRepository(PrictionaryContext dbContext)
    {
        this.dbContext = dbContext;
    }

}
