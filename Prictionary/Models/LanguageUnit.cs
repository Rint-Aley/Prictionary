namespace Prictionary.Models;

public class LanguageUnit
{
    public int Id { get; set; }

    public required string Content { get; set; }

    /// <summary>
    /// Can be used for some additinoal info for language unit (such as transcription).
    /// </summary>
    public string? AdditionalInformation { get; set; }

    public List<Meaning> Meanings { get; set; } = [];

    public List<Group> Groups { get; set; } = [];

    public List<LanguageUnitGroup> LanguageUnitGroups { get; set; } = [];

    public DateTime CreatedAt { get; private set; }

    public DateTime LastModifiedAt { get; private set; }
}