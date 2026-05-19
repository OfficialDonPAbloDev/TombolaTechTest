using Coffee4You.Server.Data;
using Coffee4You.Server.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Coffee4You.Server.Tests.Integration;

internal static class TestDb
{
    public static (AppDbContext Context, SqliteConnection Connection) Create()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return (context, connection);
    }

    public static async Task<Bean> AddBeanAsync(
        AppDbContext db,
        string name,
        string colourName = "dark roast",
        string countryName = "Brazil",
        string countryIso = "BR")
    {
        var colour = await db.Colours.SingleOrDefaultAsync(c => c.Name == colourName)
            ?? db.Colours.Add(new Colour { Name = colourName }).Entity;
        var country = await db.Countries.SingleOrDefaultAsync(c => c.Name == countryName)
            ?? db.Countries.Add(new Country { Name = countryName, IsoCode = countryIso }).Entity;

        var bean = new Bean
        {
            Name = name,
            Description = $"{name} description",
            ImageUrl = "https://example.com/img.png",
            Cost = 10m,
            CurrencyCode = "GBP",
            Colour = colour,
            Country = country,
        };
        db.Beans.Add(bean);
        await db.SaveChangesAsync();
        return bean;
    }
}
