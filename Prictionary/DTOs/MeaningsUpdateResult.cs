using Prictionary.Models;

namespace Prictionary.DTOs;

public record MeaningsUpdateResult
{
    public enum UpdateStatus
    {
        PrioritiesWasntChanged,
        PrioritiesRebalanced,
    }

    public UpdateStatus Status { get; set; }

    public Meaning Value { get; set; }

    public List<Meaning> NewMeanings { get; set; }

    public MeaningsUpdateResult(Meaning meaning)
    {
        Status = UpdateStatus.PrioritiesWasntChanged;
        Value = meaning;
        NewMeanings = [];
    }

    public MeaningsUpdateResult(Meaning meaning, List<Meaning> newMeaningValues)
    {
        Status = UpdateStatus.PrioritiesRebalanced;
        Value = meaning;
        NewMeanings = newMeaningValues;
    }
}
