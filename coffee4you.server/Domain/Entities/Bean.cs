using Coffee4You.Server.Domain.Common;

namespace Coffee4You.Server.Domain.Entities;

public class Bean : AuditableEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public decimal Cost { get; set; }
    public string CurrencyCode { get; set; } = "GBP";

    public Guid ColourId { get; set; }
    public Colour Colour { get; set; } = null!;

    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;
}
