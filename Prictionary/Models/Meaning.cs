namespace Prictionary.Models;

public class Meaning
{
    public int Id { get; set; }

    public required string Content { get; set; }

    /// <summary>
    /// Defines the order of <see cref="Meaning"/>s for <see cref="Models.LanguageUnit"/>
    /// </summary>
    public int Priority { get; set; }

    public int LanguageUnitId { get; set; }

    /// <summary>
    /// <see cref="Models.LanguageUnit"/> for which this <see cref="Meaning"/> was created.
    /// </summary>
    public required LanguageUnit LanguageUnit { get; set; }
}