using Prictionary.Models;

namespace Prictionary.Repositories.Interfaces;

public interface IGroupLanguageUnitReferencesRepository
{
    public Task<RepositoryResult<LanguageUnitGroup>> CreateAsync(int groupId, int languageUnitId);

    public Task DeleteAsync(int groupId, int languageUnitId);

    public Task<bool> CheckAsync(int groupId, int languageUnitId);

    public Task<int> GetRelationNumberAsync(int languageUnitId);
}
