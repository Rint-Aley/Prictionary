using Microsoft.EntityFrameworkCore;
using Prictionary.Database;
using Prictionary.DTOs;
using Prictionary.Models;
using Prictionary.Repositories.Interfaces;
using Prictionary.Services.Infrastructure;
using Prictionary.Services.Interfaces;

namespace Prictionary.Services.Implementation;

public class MeaningsService : IMeaningsService
{
    private readonly PrictionaryContext _dbContext;
    private readonly IMeaningsRepository _meaningsRepository;
    private readonly IAccessChecker<Meaning> _meaningsAccessChecker;

    public MeaningsService(
        PrictionaryContext dbContext,
        IMeaningsRepository meaningsRepository,
        IAccessChecker<Meaning> meaningsAccessChecker)
    {
        _meaningsRepository = meaningsRepository;
        _meaningsAccessChecker = meaningsAccessChecker;
        _dbContext = dbContext;
    }

    public async Task<Result<Meaning, ServiceErrors>> CreateMeaningAsync(
        int languageUnitId,
        string content,
        Position? position = null)
    {
        var meanings = await _dbContext.Meanings
            .Where(m => m.LanguageUnitId == languageUnitId)
            .OrderBy(m => m.Priority)
            .ToListAsync();

        var newMeaning = new Meaning
        {
            Content = content,
            LanguageUnitId = languageUnitId,
            LanguageUnit = null!,
        };

        if (position is null)
        {
            // If prioritiy wasn't specified, assignes lowest
            int? lowestPriority = meanings.Count > 0 ? meanings[0].Priority : null;
            var priority = FindPriorityValue(null, lowestPriority);
            if (priority.HasValue)
                newMeaning.Priority = priority.Value;
            else
            {
                meanings.Insert(0, newMeaning);
                RebalanceAll(meanings);
            }
        }
        else
        {
            var refMeaning = meanings.FirstOrDefault(m => m.Id == position.MeaningId);
            if (refMeaning is null)
                return new(ServiceErrors.NotFound);

            int? lower, upper;
            if (position.Placement == Position.PlacementType.Above)
            {
                lower = refMeaning.Priority;
                upper = meanings.FirstOrDefault(m => m.Priority > refMeaning.Priority)?.Priority;
            }
            else
            {
                lower = meanings.LastOrDefault(m => m.Priority < refMeaning.Priority)?.Priority;
                upper = refMeaning.Priority;
            }

            var priority = FindPriorityValue(lower, upper);
            if (priority.HasValue)
                newMeaning.Priority = priority.Value;
            else
                Rebalance(meanings, newMeaning, refMeaning, position.Placement);
        }

        _dbContext.Meanings.Add(newMeaning);
        await _dbContext.SaveChangesAsync();
        return new(newMeaning);
    }

    public async Task<Result<MeaningsUpdateResult, ServiceErrors>> UpdateMeaningByIdAsync(
        int meaningId,
        MeaningValues newValues,
        string userId)
    {
        var meaning = await _dbContext.Meanings
            .Include(m => m.LanguageUnit)
            .FirstOrDefaultAsync(m => m.Id == meaningId);
        if (meaning is null)
            return new(ServiceErrors.NotFound);
        if (!await _meaningsAccessChecker.CanChangeAsync(meaning, userId))
            return new(ServiceErrors.AccessDenied);

        if (newValues.Content is not null)
            meaning.Content = newValues.Content;

        if (newValues.Position is null)
        {
            await _dbContext.SaveChangesAsync();
            return new(new MeaningsUpdateResult(meaning));
        }

        var position = newValues.Position;

        // All sibling meanings except the one being moved, ordered by priority ascending
        var otherMeanings = await _dbContext.Meanings
            .Where(m => m.LanguageUnitId == meaning.LanguageUnitId && m.Id != meaningId)
            .OrderBy(m => m.Priority)
            .ToListAsync();

        var refMeaning = otherMeanings.FirstOrDefault(m => m.Id == position.MeaningId);
        if (refMeaning is null)
            return new(ServiceErrors.NotFound);

        int? lower, upper;
        if (position.Placement == Position.PlacementType.Above)
        {
            lower = refMeaning.Priority;
            upper = otherMeanings.FirstOrDefault(m => m.Priority > refMeaning.Priority)?.Priority;
        }
        else
        {
            lower = otherMeanings.LastOrDefault(m => m.Priority < refMeaning.Priority)?.Priority;
            upper = refMeaning.Priority;
        }

        var newPriority = FindPriorityValue(lower, upper);
        if (newPriority.HasValue)
        {
            meaning.Priority = newPriority.Value;
            await _dbContext.SaveChangesAsync();
            return new(new MeaningsUpdateResult(meaning));
        }

        var rebalanced = Rebalance(otherMeanings, meaning, refMeaning, position.Placement);
        await _dbContext.SaveChangesAsync();
        return new(new MeaningsUpdateResult(meaning, rebalanced));
    }

    /// <summary>
    /// Returns a priority value strictly between <paramref name="lower"/> and <paramref name="upper"/>,
    /// or null when there is no gap and a rebalance is required.
    /// </summary>
    private static int? FindPriorityValue(int? lower, int? upper)
    {
        int defaultInterval = Constants.MeaningPriorities.defaultInterval;

        if (lower is null && upper is null)
            return 0;

        if (lower is null)
        {
            if ((long)upper!.Value - defaultInterval >= int.MinValue)
                return upper.Value - defaultInterval;
            long gap = (long)upper.Value - int.MinValue;
            return gap >= 2 ? (int)(int.MinValue + gap / 2) : null;
        }

        if (upper is null)
        {
            if ((long)lower!.Value + defaultInterval <= int.MaxValue)
                return lower.Value + defaultInterval;
            long gap = (long)int.MaxValue - lower.Value;
            return gap >= 2 ? (int)(lower.Value + gap / 2) : null;
        }

        long span = (long)upper.Value - lower.Value;
        return span >= 2 ? lower.Value + (int)(span / 2) : null;
    }

    /// <summary>
    /// Redistributes priorities for all meanings with equal spacing,
    /// inserting <paramref name="target"/> above or below <paramref name="refMeaning"/>.
    /// Returns the full reordered list.
    /// </summary>
    private static List<Meaning> Rebalance(
        List<Meaning> otherMeanings,
        Meaning target,
        Meaning refMeaning,
        Position.PlacementType placement)
    {
        var refIndex = otherMeanings.IndexOf(refMeaning);

        var finalOrder = new List<Meaning>(otherMeanings.Count + 1);
        finalOrder.AddRange(otherMeanings);

        // In ASC order: inserting after refIndex places target above ref (higher priority value)
        int insertIndex = placement == Position.PlacementType.Above
            ? refIndex + 1
            : refIndex;

        finalOrder.Insert(insertIndex, target);
        RebalanceAll(finalOrder);
        return finalOrder;
    }

    private static void RebalanceAll(List<Meaning> orderedMeanings)
    {
        long totalRange = (long)int.MaxValue - int.MinValue;
        long spacing = totalRange / (orderedMeanings.Count + 1);

        if (spacing < 1)
            throw new InvalidOperationException("Too many meanings to rebalance.");

        for (int i = 0; i < orderedMeanings.Count; i++)
            orderedMeanings[i].Priority = (int)(int.MinValue + (long)(i + 1) * spacing);
    }
}
