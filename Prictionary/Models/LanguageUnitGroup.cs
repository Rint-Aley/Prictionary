namespace Prictionary.Models;

public class LanguageUnitGroup
{
    public int LanguageUnitID { get; set; }
    public int GroupID { get; set; }
    public required LanguageUnit LanguageUnit { get; set; }
    public required Group Group { get; set; }
}