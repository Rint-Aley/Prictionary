using Microsoft.EntityFrameworkCore;
using Prictionary.Database;
using Prictionary.Models;
using Prictionary.Repositories.Interfaces;

namespace Prictionary.Repositories.Implementions;

public class MeaningsRepository : IMeaningsRepository
{
    private readonly PrictionaryContext _dbContext;

    public MeaningsRepository(PrictionaryContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Meaning> CreateMeaningAsync(Meaning meaning, CancellationToken cancellationToken = default)
    {
        _dbContext.Meanings.Add(meaning);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return meaning;
    }

    public Task<List<Meaning>> GetMeaningsOfLanguageUnitAsync(int languageUnitId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Meanings
            .Where(m => m.LanguageUnitId == languageUnitId)
            .OrderByDescending(m => m.Priority)
            .ToListAsync(cancellationToken);
    }

    public Task<Meaning?> GetByIdAsync(int meaningId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Meanings.FirstOrDefaultAsync(m => m.Id == meaningId, cancellationToken);
    }

    public async Task<RepositoryResult<Meaning>> UpdateByIdAsync(int meaningId, Meaning newValues, CancellationToken cancellationToken = default)
    {
        var meaning = await _dbContext.Meanings.FirstOrDefaultAsync(m => m.Id == meaningId, cancellationToken);
        if (meaning is null)
            return new(QueryError.NotFound);

        meaning.Content = newValues.Content;
        meaning.Priority = newValues.Priority;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new(meaning);
    }

    public async Task DeleteByIdAsync(int meaningId, CancellationToken cancellationToken = default)
    {
        var meaning = await _dbContext.Meanings.FirstOrDefaultAsync(m => m.Id == meaningId, cancellationToken);
        if (meaning is null)
            return;

        _dbContext.Meanings.Remove(meaning);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
