namespace Prictionary.Models;

public class Group
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required AppUser User { get; set; }
    public List<LanguageUnit> LanguageUnits { get; set; } = [];
    public List<LanguageUnitGroup> LanguageUnitGroups { get; set; } = [];
    DateTime CreatedAt { get; set; }
    DateTime LastWordAddedAt { get; set; }
}