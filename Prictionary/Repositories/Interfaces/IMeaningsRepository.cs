using Prictionary.Models;

namespace Prictionary.Repositories.Interfaces;

public interface IMeaningsRepository
{
    public Task<Meaning> CreateMeaningAsync(Meaning meaning, CancellationToken cancellationToken = default);

    public Task<List<Meaning>> GetMeaningsOfLanguageUnitAsync(int languageUnitId, CancellationToken cancellationToken = default);

    public Task<Meaning?> GetByIdAsync(int meaningId, CancellationToken cancellationToken = default);

    public Task<RepositoryResult<Meaning>> UpdateByIdAsync(int meaningId, Meaning newValues, CancellationToken cancellationToken = default);

    public Task DeleteByIdAsync(int meaningId, CancellationToken cancellationToken = default);
}
