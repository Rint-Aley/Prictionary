namespace Prictionary.Models;

public class LanguageUnitGroup
{
    public int LanguageUnitId { get; set; }
    public LanguageUnit LanguageUnit { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; }
    public DateTime AddedAt { get; set; }

    private LanguageUnitGroup()
    {
        LanguageUnit = null!;
        Group = null!;
    }

    public LanguageUnitGroup(LanguageUnit languageUnit, Group group)
    {
        LanguageUnit = languageUnit;
        Group = group;
        AddedAt = DateTime.Now;
    }
    public LanguageUnitGroup(LanguageUnit languageUnit, Group group, DateTime addedAt)
    {
        LanguageUnit = languageUnit;
        Group = group;
        AddedAt = addedAt;
    }
}