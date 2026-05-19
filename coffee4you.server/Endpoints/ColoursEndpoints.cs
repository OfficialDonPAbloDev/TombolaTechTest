using Coffee4You.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace Coffee4You.Server.Endpoints;

public static class ColoursEndpoints
{
    public static IEndpointRouteBuilder MapColoursEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/colours", async (AppDbContext context, CancellationToken cancelToken) =>
        {
            var names = await context.Colours
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .ToListAsync(cancelToken);
            return Results.Ok(names);
        })
        .WithTags("Colours")
        .WithSummary("List all colour names.")
        .WithDescription(
            "Returns the distinct roast/colour names available in our catalogue " + 
            "(e.g. \"dark roast\", \"medium roast\"), sorted alphabetically.")
        .Produces<List<string>>(StatusCodes.Status200OK);

        return app;
    }
}
