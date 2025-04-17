namespace Application.Dtos.Api.Requests;
public record AddDescriptionRequestDto
{
    public required int Id { get; init; }
    public required string Description { get; init; }
}
