namespace JobReadApi.Application.Dtos;

public record PagedResultDto<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalCount);

