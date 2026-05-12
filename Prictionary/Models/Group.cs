namespace Prictionary.Models;

public class Group
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public required string OwnerId { get; set; }

    public required AppUser Owner { get; set; }

    public List<LanguageUnit> LanguageUnits { get; set; } = [];

    public List<LanguageUnitGroup> LanguageUnitGroups { get; set; } = [];

    public DateTime CreatedAt { get; private set; }

    public DateTime LastWordAddedAt { get; private set; }
}