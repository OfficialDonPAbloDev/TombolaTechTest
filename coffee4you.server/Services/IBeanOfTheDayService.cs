using Coffee4You.Server.Domain.Entities;

namespace Coffee4You.Server.Services;

public interface IBeanOfTheDayService
{
    Task<Bean?> GetOrSelectTodayAsync(CancellationToken ct = default);
}
