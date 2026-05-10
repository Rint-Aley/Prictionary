using Prictionary.Models;

namespace Prictionary.ViewModels.Responses;

public record GroupResponse
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public static explicit operator GroupResponse(Group group)
    {
        return new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
        };
    }
}
