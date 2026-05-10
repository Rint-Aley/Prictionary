using System.ComponentModel.DataAnnotations;

namespace Prictionary.ViewModels.Responses;

public class UpdateMeaningResponse
{
    [Required]
    public bool PrioritiesRebalanced { get; set; }

    [Required]
    public required MeaningResponse UpdatedMeaning { get; set;  }
}
