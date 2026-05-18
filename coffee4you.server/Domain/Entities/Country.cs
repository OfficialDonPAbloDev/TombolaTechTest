using Coffee4You.Server.Domain.Common;

namespace Coffee4You.Server.Domain.Entities;

public class Country : AuditableEntity
{
    public string Name { get; set; } = null!;
    public string IsoCode { get; set; } = null!;

    public ICollection<Bean> Beans { get; set; } = new List<Bean>();
}
