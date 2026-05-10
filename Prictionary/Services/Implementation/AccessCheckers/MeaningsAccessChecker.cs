using Prictionary.Models;
using Prictionary.Repositories.Interfaces;
using Prictionary.Services.Interfaces;

namespace Prictionary.Services.Implementation.AccessCheckers;

public class MeaningsAccessChecker : IAccessChecker<Meaning>
{
    private readonly IAccessChecker<LanguageUnit> _languageUnitsAccessChecker;
    private readonly ILanguageUnitsRepository _languageUnitsRepository;

    public MeaningsAccessChecker(
        IAccessChecker<LanguageUnit> languageUnitsAccessChecker,
        ILanguageUnitsRepository languageUnitsRepository)
    {
        _languageUnitsAccessChecker = languageUnitsAccessChecker;
        _languageUnitsRepository = languageUnitsRepository;
    }

    public async Task<bool> CanReadAsync(Meaning resource, string userId, CancellationToken cancellationToken)
    {
        if (resource.LanguageUnit is null)
        {
            var languageUnit = await _languageUnitsRepository.GetByIdAsync(resource.LanguageUnitId, cancellationToken);
            if (languageUnit is null) 
            {
                throw new InvalidOperationException($"Language Unit with id {resource.LanguageUnitId} doesn't exist.");
            }
            resource.LanguageUnit = languageUnit;
        }
        return await _languageUnitsAccessChecker.CanReadAsync(resource.LanguageUnit, userId, cancellationToken);
    }

    public Task<bool> CanChangeAsync(Meaning resource, string userId, CancellationToken cancellationToken)
        => CanReadAsync(resource, userId, cancellationToken);

    public Task<bool> CanDeleteAsync(Meaning resource, string userId, CancellationToken cancellationToken)
        => CanReadAsync(resource, userId, cancellationToken);
}
