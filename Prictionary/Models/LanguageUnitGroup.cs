namespace Prictionary.Models;

public class LanguageUnitGroup
{
    public int LanguageUnitID { get; set; }
    public required LanguageUnit LanguageUnit { get; set; }
    public int GroupID { get; set; }
    public required Group Group { get; set; }
    public DateTime AddedAt { get; set; }

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