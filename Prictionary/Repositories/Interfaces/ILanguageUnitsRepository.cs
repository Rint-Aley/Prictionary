using Prictionary.Models;

namespace Prictionary.Repositories.Interfaces;

public interface ILanguageUnitsRepository
{
    public Task<LanguageUnit?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    public Task<List<LanguageUnit>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    public Task CreateAsync(LanguageUnit languageUnit, CancellationToken cancellationToken = default);

    public Task CreateRangeAsync(IEnumerable<LanguageUnit> languageUnits, CancellationToken cancellationToken = default);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    public Task<RepositoryResult<LanguageUnit>> UpdateAsync(
        int id,
        LanguageUnit newLanguageUnitValues,
        CancellationToken cancellationToken = default);
}
