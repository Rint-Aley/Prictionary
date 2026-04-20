using Microsoft.EntityFrameworkCore;
using Prictionary.Database;
using Prictionary.Models;
using Prictionary.Repositories.Interfaces;

namespace Prictionary.Repositories.Implementions;

public class LanguageUnitsRepository : ILanguageUnitsRepository
{
    private readonly PrictionaryContext _dbContext;

    public LanguageUnitsRepository(PrictionaryContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task CreateAsync(LanguageUnit languageUnit)
    {
        _dbContext.LanguageUnits.Add(languageUnit);
        return _dbContext.SaveChangesAsync();
    }

    public Task CreateRangeAsync(IEnumerable<LanguageUnit> languageUnits)
    {
        _dbContext.LanguageUnits.AddRange(languageUnits);
        return _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var languageUnit = await _dbContext.LanguageUnits.FirstOrDefaultAsync(lu => lu.Id == id);
        if (languageUnit is null)
            return;
        _dbContext.LanguageUnits.Remove(languageUnit);
        await _dbContext.SaveChangesAsync();
    }

    public Task<LanguageUnit?> GetByIdAsync(int id)
    {
        return _dbContext.LanguageUnits.FirstOrDefaultAsync(lu => lu.Id == id);
    }

    public Task<List<LanguageUnit>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return _dbContext.LanguageUnits.Where(lu => ids.Contains(lu.Id)).ToListAsync();
    }

    public async Task<RepositoryResult<LanguageUnit>> UpdateAsync(int id, LanguageUnit newLanguageUnitValues)
    {
        var languageUnit = await _dbContext.LanguageUnits.FirstOrDefaultAsync(lu => lu.Id == id);
        if (languageUnit is null)
            return new(QueryError.NotFound);
        languageUnit.Content = newLanguageUnitValues.Content;
        languageUnit.AdditionalInformation = newLanguageUnitValues.AdditionalInformation;
        await _dbContext.SaveChangesAsync();
        return new(languageUnit);
    }
}
