using System.ComponentModel.DataAnnotations;

namespace Prictionary.DTOs;

public record MeaningValues
{
    public string? Content { get; set; }

    public Position? Position { get; set; }
}

public record Position
{
    [Required]
    public PlacementType Placement { get; set; }

    [Required]
    public int MeaningId { get; set; }

    public enum PlacementType
    {
        Above,
        Below,
    }
}
