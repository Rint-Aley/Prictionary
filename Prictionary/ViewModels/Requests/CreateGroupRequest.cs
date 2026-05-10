using System.ComponentModel.DataAnnotations;

namespace Prictionary.ViewModels.Requests;

public record CreateGroupRequest
{
    [Required]
    public required string Name { get; set; }

    public string? Description { get; set; }
}
