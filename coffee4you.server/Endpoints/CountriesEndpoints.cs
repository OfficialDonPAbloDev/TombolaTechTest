using Coffee4You.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace Coffee4You.Server.Endpoints;

public static class CountriesEndpoints
{
    public static IEndpointRouteBuilder MapCountriesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/countries", async (AppDbContext context, CancellationToken cancelToken) =>
        {
            var names = await context.Countries
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .ToListAsync(cancelToken);
            return Results.Ok(names);
        })
        .WithTags("Countries")
        .WithSummary("List all country names.")
        .WithDescription(
            "Returns the distinct country names of beans currently in the catalogue " + 
            ", sorted alphabetically")
        .Produces<List<string>>(StatusCodes.Status200OK);

        return app;
    }
}
