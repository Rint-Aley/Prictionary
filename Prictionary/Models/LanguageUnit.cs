namespace Prictionary.Models;

public class LanguageUnit
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public string? Transcription { get; set; }
    public List<Translation> Translations { get; } = [];
    public List<LanguageUnitGroup> Groups { get; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}