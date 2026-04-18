using Microsoft.EntityFrameworkCore;
using Prictionary.Database;
using Prictionary.Models;
using Prictionary.Repositories.Interfaces;

namespace Prictionary.Repositories;

public class GroupsRepository : IGroupsRepository
{
    private readonly PrictionaryContext _dbContext;

    public Task<List<Group>> GetGroupListForSpecificUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public GroupsRepository(PrictionaryContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Group?> GetByIdAsync(int id)
    {
        return _dbContext.Groups.FirstOrDefaultAsync(group => group.Id == id);
    }

    public Task<List<Group>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return _dbContext.Groups.Where(group => ids.Contains(group.Id)).ToListAsync();
    }

    public async Task<RepositoryResult<Group>> CreateGroupAsync(Group group)
    {
        _dbContext.Groups.Add(group);
        await _dbContext.SaveChangesAsync();
        return new(group);
    }

    public async Task<RepositoryResult<IEnumerable<Group>>> CreateGroupsAsync(IEnumerable<Group> groups)
    {
        _dbContext.Groups.AddRange(groups);
        await _dbContext.SaveChangesAsync();
        return new(groups);
    }

    public async Task<RepositoryResult<Group>> ChangeGroup(int groupId, Group updatedGroup)
    {
        try
        {
            var group = await _dbContext.Groups.FirstOrDefaultAsync(group => group.Id == groupId);
            if (group is null)
                return new(QueryError.NotFound);

            group.Name = updatedGroup.Name;
            group.Description = updatedGroup.Description;

            await _dbContext.SaveChangesAsync();

            return new(group);
        }
        catch (Exception ex)
        {
            return new(QueryError.ConstraintViolated, ex.Message);
        }
    }

    public async Task DeleteGroupAsync(int id)
    {
        var group = await GetByIdAsync(id);
        if (group is null) 
            return;
        _dbContext.Remove(group);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteGroupRangeAsync(IEnumerable<int> ids)
    {
        var groups = await GetByIdsAsync(ids);
        _dbContext.RemoveRange(groups);
        await _dbContext.SaveChangesAsync();
    }

    public Task<List<int>> GetWordsIdsAsync(int groupId)
    {
        return _dbContext.LanguageUnitGroups
            .Where(lug => lug.GroupId == groupId)
            .Select(lug => lug.LanguageUnitId)
            .ToListAsync();
    }
}
