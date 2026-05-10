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

    public Task CreateAsync(LanguageUnit languageUnit, CancellationToken cancellationToken = default)
    {
        _dbContext.LanguageUnits.Add(languageUnit);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task CreateRangeAsync(IEnumerable<LanguageUnit> languageUnits, CancellationToken cancellationToken = default)
    {
        _dbContext.LanguageUnits.AddRange(languageUnits);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var languageUnit = await _dbContext.LanguageUnits.FirstOrDefaultAsync(lu => lu.Id == id, cancellationToken);
        if (languageUnit is null)
            return;
        _dbContext.LanguageUnits.Remove(languageUnit);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<LanguageUnit?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.LanguageUnits.FirstOrDefaultAsync(lu => lu.Id == id, cancellationToken);
    }

    public Task<List<LanguageUnit>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return _dbContext.LanguageUnits.Where(lu => ids.Contains(lu.Id)).ToListAsync(cancellationToken);
    }

    public async Task<RepositoryResult<LanguageUnit>> UpdateAsync(
        int id,
        LanguageUnit newLanguageUnitValues,
        CancellationToken cancellationToken = default)
    {
        var languageUnit = await _dbContext.LanguageUnits.FirstOrDefaultAsync(lu => lu.Id == id, cancellationToken);
        if (languageUnit is null)
            return new(QueryError.NotFound);
        languageUnit.Content = newLanguageUnitValues.Content;
        languageUnit.AdditionalInformation = newLanguageUnitValues.AdditionalInformation;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new(languageUnit);
    }
}
