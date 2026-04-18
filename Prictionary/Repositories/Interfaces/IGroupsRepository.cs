using Prictionary.Models;

namespace Prictionary.Repositories.Interfaces;

public interface IGroupsRepository
{
    /// <summary>
    /// Returns all groups for specified user.
    /// </summary>
    public Task<List<Group>> GetGroupListForSpecificUserAsync(string userId);

    /// <summary>
    /// Returns <see cref="Group"/> by Id. If not found, returns <see langword="null"/>.
    /// </summary>
    public Task<Group?> GetByIdAsync(int id);

    public Task<List<Group>> GetByIdsAsync(IEnumerable<int> ids);

    public Task<RepositoryResult<Group>> CreateGroupAsync(Group group);

    public Task<RepositoryResult<IEnumerable<Group>>> CreateGroupsAsync(IEnumerable<Group> groups);

    public Task<RepositoryResult<Group>> ChangeGroup(int groupId, Group updatedGroup);

    public Task DeleteGroupAsync(int id);

    public Task DeleteGroupRangeAsync(IEnumerable<int> ids);

    /// <summary>
    /// Returns Ids of words in the specified Group.
    /// </summary>
    /// <returns>List of Ids if group was found. <see cref="RepositoryError"/>, if group wasn't found</returns>
    public Task<List<int>> GetWordsIdsAsync(int groupId);
}
