using Prictionary.Models;

namespace Prictionary.ViewModels.Responses;

public record MeaningResponse
{
    public int Id { get; set; }

    public required string Content { get; set; }

    public int LanguageUnitId { get; set; }

    public int Priority { get; set; }

    public static explicit operator MeaningResponse(Meaning meaning)
    {
        return new MeaningResponse
        {
            Id = meaning.Id,
            Content = meaning.Content,
            LanguageUnitId = meaning.LanguageUnitId,
            Priority = meaning.Priority,
        };
    }
}
