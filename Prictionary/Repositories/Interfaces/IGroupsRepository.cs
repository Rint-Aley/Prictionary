using System.Text.RegularExpressions;

namespace Prictionary.Repositories.Interfaces;

public interface IGroupsRepository
{
    public Task<Group> GetByIdAsync(int id);

    public Task<List<Group>> GetByIdsAsync(IEnumerable<int> ids);

    public Task<List<int>> GetWordsIdsAsync(int groupId);
}
