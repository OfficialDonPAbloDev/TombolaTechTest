using Coffee4You.Server.Data;
using Coffee4You.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coffee4You.Server.Services;

public class BeanOfTheDayService : IBeanOfTheDayService
{
    private readonly AppDbContext _context;

    public BeanOfTheDayService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Bean?> GetOrSelectTodayAsync(CancellationToken cancelToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var existing = await _context.BeansOfTheDay
            .Include(b => b.Bean).ThenInclude(b => b.Colour)
            .Include(b => b.Bean).ThenInclude(b => b.Country)
            .SingleOrDefaultAsync(b => b.SelectedFor == today, cancelToken);

        if (existing is not null)
        {
            return existing.Bean;
        }

        var yesterday = today.AddDays(-1);
        var yesterdayBeanId = await _context.BeansOfTheDay
            .Where(b => b.SelectedFor == yesterday)
            .Select(b => (Guid?)b.BeanId)
            .SingleOrDefaultAsync(cancelToken);

        var pool = _context.Beans.AsQueryable();
        if (yesterdayBeanId.HasValue)
        {
            pool = pool.Where(b => b.Id != yesterdayBeanId.Value);
        }

        var poolCount = await pool.CountAsync(cancelToken);
        if (poolCount == 0)
        {
            // Only yesterday's bean exists; relax the rule rather than return nothing.
            if (!await _context.Beans.AnyAsync(cancelToken))
            {
                return null;
            }
            pool = _context.Beans.AsQueryable();
            poolCount = 1;
        }

        var skip = Random.Shared.Next(poolCount);
        var picked = await pool
            .Include(b => b.Colour)
            .Include(b => b.Country)
            .OrderBy(b => b.Id)
            .Skip(skip)
            .Take(1)
            .SingleAsync(cancelToken);

        try
        {
            _context.BeansOfTheDay.Add(new BeanOfTheDay
            {
                BeanId = picked.Id,
                SelectedFor = today,
                SelectedAt = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync(cancelToken);
            return picked;
        }
        catch (DbUpdateException)
        {
            _context.ChangeTracker.Clear();
            var winner = await _context.BeansOfTheDay
                .Include(b => b.Bean).ThenInclude(b => b.Colour)
                .Include(b => b.Bean).ThenInclude(b => b.Country)
                .SingleAsync(b => b.SelectedFor == today, cancelToken);
            return winner.Bean;
        }
    }
}
