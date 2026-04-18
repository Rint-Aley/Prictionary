using System.ComponentModel.DataAnnotations;

namespace Prictionary.ViewModels.Requests;

public class CreateGroupRequest
{
    [Required]
    public required string Name { get; set; }

    public string? Description { get; set; }
}
