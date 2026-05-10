using Prictionary.DTOs;
using Prictionary.Models;
using Prictionary.Services.Infrastructure;

namespace Prictionary.Services.Interfaces;

public interface IMeaningsService
{
    public Task<Result<Meaning, ServiceErrors>> CreateMeaningAsync(
        int languageUnitId,
        string content,
        Position? position = null);

    /// <summary>
    /// Updates values of meaning.
    /// </summary>
    /// <param name="meaningId">Id of meaning that will be changed.</param>
    /// <param name="newValues">New values of meaning.</param>
    public Task<Result<MeaningsUpdateResult, ServiceErrors>> UpdateMeaningByIdAsync(
        int meaningId,
        MeaningValues newValues,
        string userId);
}
