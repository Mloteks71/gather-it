namespace JobReadApi.Application.Dtos;

public record DescriptionDto(
    int DescriptionId,
    string? DescriptionText,
    string? Requirements,
    string? Benefits,
    string? Workstyle,
    string? AboutProject);

