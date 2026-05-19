namespace Coffee4You.Server.Dtos;

public record BeansPageDto(
    IReadOnlyList<BeanDto> Items,
    int Total,
    int Page,
    int PageSize,
    bool HasMore
);
