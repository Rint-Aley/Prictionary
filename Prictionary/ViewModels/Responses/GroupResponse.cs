namespace Prictionary.ViewModels.Responses;

public class GroupResponse
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
