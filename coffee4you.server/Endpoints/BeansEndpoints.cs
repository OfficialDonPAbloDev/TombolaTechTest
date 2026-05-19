using System.ComponentModel;
using Coffee4You.Server.Data;
using Coffee4You.Server.Domain.Common;
using Coffee4You.Server.Dtos;
using Coffee4You.Server.Mappers;
using Coffee4You.Server.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Coffee4You.Server.Endpoints;

public static class BeansEndpoints
{
    private const int MINIMUM_BEAN_NAME_SEARCH_LENGTH = 2;

    public static IEndpointRouteBuilder MapBeansEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/beans").WithTags("Beans");

        group.MapGet("", GetBeans)
            .WithSummary("List beans, paged and optionally filtered.")
            .WithDescription(
                "Returns a page of beans ordered by name. " +
                "Filters: country (exact match) and beanName (substring, applied only when 2+ characters). ")
            .Produces<BeansPageDto>(StatusCodes.Status200OK);

        group.MapGet("/bean-of-the-day", GetBeanOfTheDay)
            .WithSummary("Get today's Bean of the Day.")
            .WithDescription(
                "Returns today's bean of the day." +
                "Returns 404 only if the catalogue is empty.")
            .Produces<BeanDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}", GetBean)
            .WithSummary("Get a single bean by id.")
            .WithDescription("Returns the bean with the given GUID, or 404 if it does not exist.")
            .Produces<BeanDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetBeans(
        AppDbContext context,
        IOptions<ConfigSettings> settings,
        CancellationToken cancelToken,
        [Description("Page number, 1-based. Defaults to 1.")]
        int page = 1,
        [Description("Items per page. Omit to use AppSettings.PagingPageSize. Clamped to 1..100.")]
        int? pageSize = null,
        [Description("Filter by exact country name, e.g. \"Brazil\". Omit for all countries.")]
        string? country = null,
        [Description("Filter by partial bean name (case-insensitive substring). Ignored when fewer than 2 characters.")]
        string? beanName = null)
    {
        var configPageSize = settings.Value.PagingPageSize;

        if (page < 1)
        {
            page = 1;
        }

        var effectivePageSize = pageSize ?? configPageSize;

        if (effectivePageSize < 1) effectivePageSize = configPageSize;
        if (effectivePageSize > 100) effectivePageSize = 100;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var todaysBotdBeanId = await GetTodaysBeanOfTheDayId(context, today, cancelToken);

        var query = context.Beans
            .Include(b => b.Colour)
            .Include(b => b.Country)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(country))
        {
            query = query.Where(b => b.Country.Name == country);
        }

        var trimmedQuery = beanName?.Trim() ?? string.Empty;

        if (trimmedQuery.Length >= MINIMUM_BEAN_NAME_SEARCH_LENGTH)
        {
            query = query.Where(b => b.Name.Contains(trimmedQuery));
        }

        var total = await query.CountAsync(cancelToken);
        var beans = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * effectivePageSize)
            .Take(effectivePageSize)
            .ToListAsync(cancelToken);

        var items = beans
            .Select(b => BeanMapper.ToDto(b, todaysBotdBeanId))
            .ToList();
        var hasMore = page * effectivePageSize < total;

        return Results.Ok(new BeansPageDto(items, total, page, effectivePageSize, hasMore));
    }

    private static async Task<IResult> GetBean(
        [Description("Bean id (GUID).")]
        Guid id,
        AppDbContext context,
        CancellationToken cancelToken)
    {
        var bean = await context.Beans
            .Include(b => b.Colour)
            .Include(b => b.Country)
            .SingleOrDefaultAsync(b => b.Id == id, cancelToken);

        if (bean is null)
        {
            return Results.NotFound();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        Guid? todaysBotdBeanId = await GetTodaysBeanOfTheDayId(context, today, cancelToken);

        return Results.Ok(BeanMapper.ToDto(bean, todaysBotdBeanId));
    }

    private static async Task<Guid?> GetTodaysBeanOfTheDayId(AppDbContext context, DateOnly today, CancellationToken cancelToken)
    {
        return await context.BeansOfTheDay
            .Where(b => b.SelectedFor == today)
            .Select(b => (Guid?)b.BeanId)
            .SingleOrDefaultAsync(cancelToken);
    }

    private static async Task<IResult> GetBeanOfTheDay(
        IBeanOfTheDayService beanOfTheDayService,
        CancellationToken cancelToken)
    {
        var bean = await beanOfTheDayService.GetOrSelectTodayAsync(cancelToken);

        return bean is null
            ? Results.NotFound()
            : Results.Ok(BeanMapper.ToDto(bean, bean.Id));
    }
}
