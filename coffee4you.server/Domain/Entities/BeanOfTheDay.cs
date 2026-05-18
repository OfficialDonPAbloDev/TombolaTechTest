using Coffee4You.Server.Domain.Common;

namespace Coffee4You.Server.Domain.Entities;

public class BeanOfTheDay : AuditableEntity
{
    public Guid BeanId { get; set; }
    public Bean Bean { get; set; } = null!;

    public DateOnly SelectedFor { get; set; }
    public DateTime SelectedAt { get; set; }
}
