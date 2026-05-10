using Prictionary.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Prictionary.ViewModels.Requests;

public record MeaningRequest
{
    [Required]
    public required string Content { get; set; }

    public Position? Position { get; set; }

    public static explicit operator MeaningValues(MeaningRequest request)
        => new MeaningValues
        {
            Content = request.Content,
            Position = request.Position is null ?
                null :
                (DTOs.Position)request.Position,
        };
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

    public static explicit operator DTOs.Position(Position request)
        => new DTOs.Position
        {
            MeaningId = request.MeaningId,
            Placement = request.Placement switch
            {
                PlacementType.Above => DTOs.Position.PlacementType.Above,
                PlacementType.Below => DTOs.Position.PlacementType.Below,
                _ => throw new NotSupportedException()
            }
        };
}