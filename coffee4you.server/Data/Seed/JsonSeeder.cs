using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Coffee4You.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coffee4You.Server.Data.Seed;

public static class JsonSeeder
{
    private static readonly Dictionary<string, string> CountryIsoMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Peru"] = "PE",
        ["Vietnam"] = "VN",
        ["Colombia"] = "CO",
        ["Brazil"] = "BR",
        ["Honduras"] = "HN",
    };

    private static readonly Dictionary<string, string> SymbolToCurrency = new()
    {
        ["£"] = "GBP",
        ["$"] = "USD",
        ["€"] = "EUR",
    };

    private static readonly Regex CostPattern = new(@"^\s*([£$€])\s*([\d.]+)\s*$", RegexOptions.Compiled);

    public static async Task SeedAsync(AppDbContext db, string seedFilePath, CancellationToken ct = default)
    {
        if (await db.Beans.AnyAsync(ct))
        {
            return;
        }

        if (!File.Exists(seedFilePath))
        {
            return;
        }

        await using var stream = File.OpenRead(seedFilePath);
        var raw = await JsonSerializer.DeserializeAsync<List<RawBean>>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            ct);
        if (raw is null || raw.Count == 0)
        {
            return;
        }

        var colours = raw
            .Select(r => r.Colour)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(name => new Colour { Name = name })
            .ToList();
        await db.Colours.AddRangeAsync(colours, ct);

        var countries = raw
            .Select(r => r.Country)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(name => new Country
            {
                Name = name,
                IsoCode = CountryIsoMap.TryGetValue(name, out var iso) ? iso : "??",
            })
            .ToList();
        await db.Countries.AddRangeAsync(countries, ct);

        await db.SaveChangesAsync(ct);

        var colourIdByName = colours.ToDictionary(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase);
        var countryIdByName = countries.ToDictionary(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase);

        var beans = raw.Select(r =>
        {
            var (cost, currency) = ParseCost(r.Cost);
            return new Bean
            {
                Name = r.Name,
                Description = r.Description,
                ImageUrl = r.Image,
                Cost = cost,
                CurrencyCode = currency,
                ColourId = colourIdByName[r.Colour],
                CountryId = countryIdByName[r.Country],
            };
        }).ToList();

        await db.Beans.AddRangeAsync(beans, ct);
        await db.SaveChangesAsync(ct);
    }

    internal static (decimal cost, string currency) ParseCost(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return (0m, "GBP");
        }

        var match = CostPattern.Match(raw);
        if (!match.Success)
        {
            return (0m, "GBP");
        }

        var symbol = match.Groups[1].Value;
        var amount = decimal.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
        return (amount, SymbolToCurrency.TryGetValue(symbol, out var c) ? c : "GBP");
    }

    private sealed class RawBean
    {
        [JsonPropertyName("Cost")]
        public string Cost { get; set; } = null!;

        [JsonPropertyName("Image")]
        public string Image { get; set; } = null!;

        [JsonPropertyName("colour")]
        public string Colour { get; set; } = null!;

        [JsonPropertyName("Name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("Description")]
        public string Description { get; set; } = null!;

        [JsonPropertyName("Country")]
        public string Country { get; set; } = null!;
    }
}
