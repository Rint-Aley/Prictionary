using System.ComponentModel.DataAnnotations;

namespace Prictionary.ViewModels.Requests;

public record CreateLanguageUnitRequest
{
    [Required]
    public required string Content { get; set; }

    public string? AdditionalInformation { get; set; }
}
