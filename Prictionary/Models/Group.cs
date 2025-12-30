namespace Prictionary.Models;

public class Group
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public List<LanguageUnitGroup> LanguageUnits { get; } = [];
}