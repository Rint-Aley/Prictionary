namespace Prictionary.Models;

public class Translation
{
    public int Id { get; set; }
    public int LanguageUnitID { get; set; }
    public required LanguageUnit LanguageUnit { get; set; }
    public required string Content { get; set; }
    public ushort Priority { get; set; }
}