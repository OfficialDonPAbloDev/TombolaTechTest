namespace Coffee4You.Server.Dtos;

public record BeanDto(
    Guid Id,
    string Name,
    string Description,
    string ImageUrl,
    decimal Cost,
    string Currency,
    string Colour,
    string Country,
    bool IsBeanOfTheDay
);
