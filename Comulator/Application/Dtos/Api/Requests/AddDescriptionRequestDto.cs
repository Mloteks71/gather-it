namespace Application.Dtos.Api.Requests;
public record AddDescriptionRequestDto
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public string? Workstyle { get; set; }
    public string? AboutProject { get; set; }
}
