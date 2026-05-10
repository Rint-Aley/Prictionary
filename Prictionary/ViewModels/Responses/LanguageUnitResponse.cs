namespace Prictionary.ViewModels.Responses;

public record LanguageUnitResponse
{
    public int Id { get; set; }

    public required string Content { get; set; }

    public string? AdditionalInformation { get; set; }

    public DateTime CreatedAt { get; set; }

    DateTime LastModifiedAt { get; set; }
}
