using Prictionary.Models;

namespace Prictionary.Repositories.Interfaces;

public interface ILanguageUnitsRepository
{
    public Task<LanguageUnit?> GetByIdAsync(int id);

    public Task<List<LanguageUnit>> GetByIdsAsync(IEnumerable<int> ids);

    public Task CreateAsync(LanguageUnit languageUnit);

    public Task CreateRangeAsync(IEnumerable<LanguageUnit> languageUnits);

    public Task DeleteAsync(int id);

    public Task<RepositoryResult<LanguageUnit>> UpdateAsync(int id, LanguageUnit newLanguageUnitValues);
}
