using Microsoft.EntityFrameworkCore;
using Prictionary.Database;
using Prictionary.Models;
using Prictionary.Repositories.Interfaces;

namespace Prictionary.Repositories.Implementions;

public class GroupLanguageUnitReferencesRepository : IGroupLanguageUnitReferencesRepository
{
    private readonly PrictionaryContext _dbContext;

    public GroupLanguageUnitReferencesRepository(PrictionaryContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResult<LanguageUnitGroup>> CreateAsync(int groupId, int languageUnitId)
    {
        var languageUnit = await _dbContext.LanguageUnits.FirstOrDefaultAsync();
        if (languageUnit is null)
            return new(QueryError.NotFound);
        var group = await _dbContext.Groups.FirstOrDefaultAsync();
        if (group is null)
            return new(QueryError.NotFound);

        var newLanguageUnitGroup = new LanguageUnitGroup(languageUnit, group);
        _dbContext.LanguageUnitGroups.Add(newLanguageUnitGroup);
        await _dbContext.SaveChangesAsync();

        return new(newLanguageUnitGroup);
    }

    public Task<bool> CheckAsync(int groupId, int languageUnitId)
    {
        return _dbContext.LanguageUnitGroups.AnyAsync(lug => lug.GroupId == groupId && lug.LanguageUnitId == languageUnitId);
    }

    public async Task DeleteAsync(int groupId, int languageUnitId)
    {
        var targetLUG = await _dbContext.LanguageUnitGroups.FirstOrDefaultAsync(lug => 
            lug.GroupId == groupId && lug.LanguageUnitId == languageUnitId);
        if (targetLUG is null)
            return;
        _dbContext.LanguageUnitGroups.Remove(targetLUG);
        await _dbContext.SaveChangesAsync();
    }

    public Task<int> GetRelationNumberAsync(int languageUnitId)
    {
        return _dbContext.LanguageUnitGroups.Where(lug => lug.LanguageUnitId == languageUnitId).CountAsync();
    }
}
