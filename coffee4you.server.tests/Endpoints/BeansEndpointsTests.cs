using System.Net;
using System.Net.Http.Json;
using Coffee4You.Server.Data;
using Coffee4You.Server.Domain.Entities;
using Coffee4You.Server.Dtos;
using FluentAssertions;

namespace Coffee4You.Server.Tests.Endpoints;

public class BeansEndpointsTests
{
    [Fact]
    public async Task GET_Beans_ReturnsFirstPage_WithHasMore_WhenMorePagesExist()
    {
        using var factory = new TestWebApplicationFactory();
        await SeedBeansAsync(factory, count: 10);
        var client = factory.CreateClient();

        var page = await client.GetFromJsonAsync<BeansPageDto>("/api/beans?page=1&pageSize=3");

        page.Should().NotBeNull();
        page!.Items.Should().HaveCount(3);
        page.Total.Should().Be(10);
        page.Page.Should().Be(1);
        page.PageSize.Should().Be(3);
        page.HasMore.Should().BeTrue();
    }

    [Fact]
    public async Task GET_Beans_ReturnsLastPage_WithHasMoreFalse()
    {
        using var factory = new TestWebApplicationFactory();
        await SeedBeansAsync(factory, count: 7);
        var client = factory.CreateClient();

        var page = await client.GetFromJsonAsync<BeansPageDto>("/api/beans?page=3&pageSize=3");

        page.Should().NotBeNull();
        page!.Items.Should().HaveCount(1);  // 7 items, 3 per page -> last page has 1
        page.HasMore.Should().BeFalse();
    }

    [Fact]
    public async Task GET_Beans_FiltersByCountry_ExactMatch()
    {
        using var factory = new TestWebApplicationFactory();
        using (var db = await factory.CreateDbContextAsync())
        {
            await SeedSingleAsync(db, "Alpha", country: "Brazil", iso: "BR");
            await SeedSingleAsync(db, "Beta", country: "Brazil", iso: "BR");
            await SeedSingleAsync(db, "Gamma", country: "Peru", iso: "PE");
        }
        var client = factory.CreateClient();

        var page = await client.GetFromJsonAsync<BeansPageDto>("/api/beans?country=Brazil&pageSize=50");

        page!.Items.Should().HaveCount(2);
        page.Items.Should().OnlyContain(b => b.Country == "Brazil");
    }

    [Fact]
    public async Task GET_Beans_BeanNameFilter_IgnoredBelowTwoCharacters()
    {
        using var factory = new TestWebApplicationFactory();
        await SeedBeansAsync(factory, count: 5);
        var client = factory.CreateClient();

        // 1-char query → server should not filter, all 5 returned.
        var page = await client.GetFromJsonAsync<BeansPageDto>("/api/beans?beanName=B&pageSize=50");

        page!.Items.Should().HaveCount(5);
    }

    [Fact]
    public async Task GET_Beans_BeanNameFilter_AppliedAtTwoOrMoreCharacters()
    {
        using var factory = new TestWebApplicationFactory();
        using (var db = await factory.CreateDbContextAsync())
        {
            await SeedSingleAsync(db, "Ronbert");
            await SeedSingleAsync(db, "Turnabout");
            await SeedSingleAsync(db, "Zillan");
        }
        var client = factory.CreateClient();

        var page = await client.GetFromJsonAsync<BeansPageDto>("/api/beans?beanName=ro&pageSize=50");

        page!.Items.Should().HaveCount(1);
        page.Items[0].Name.Should().Be("Ronbert");
    }

    [Fact]
    public async Task GET_Beans_PageSizeOmitted_FallsBackToConfiguredValue()
    {
        // appsettings.json sets AppSettings:PagingPageSize = 6, so the response should clamp to 6.
        using var factory = new TestWebApplicationFactory();
        await SeedBeansAsync(factory, count: 12);
        var client = factory.CreateClient();

        var page = await client.GetFromJsonAsync<BeansPageDto>("/api/beans");

        page!.Items.Should().HaveCount(6);
        page.PageSize.Should().Be(6);
    }

    [Fact]
    public async Task GET_Beans_PageSizeOver100_IsClampedTo100()
    {
        using var factory = new TestWebApplicationFactory();
        await SeedBeansAsync(factory, count: 150);
        var client = factory.CreateClient();

        var page = await client.GetFromJsonAsync<BeansPageDto>("/api/beans?pageSize=500");

        page!.PageSize.Should().Be(100);
        page.Items.Should().HaveCount(100);
    }

    [Fact]
    public async Task GET_Bean_ById_ReturnsNotFound_ForUnknownId()
    {
        using var factory = new TestWebApplicationFactory();
        await factory.CreateDbContextAsync();   // creates schema
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/beans/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- helpers ----

    private static async Task SeedBeansAsync(TestWebApplicationFactory factory, int count)
    {
        using var db = await factory.CreateDbContextAsync();
        for (var i = 0; i < count; i++)
        {
            await SeedSingleAsync(db, $"Bean{i:000}");
        }
    }

    private static async Task SeedSingleAsync(
        AppDbContext db,
        string name,
        string colourName = "dark roast",
        string country = "Brazil",
        string iso = "BR")
    {
        var colour = db.Colours.Local.SingleOrDefault(c => c.Name == colourName)
            ?? db.Colours.SingleOrDefault(c => c.Name == colourName)
            ?? db.Colours.Add(new Colour { Name = colourName }).Entity;

        var countryEntity = db.Countries.Local.SingleOrDefault(c => c.Name == country)
            ?? db.Countries.SingleOrDefault(c => c.Name == country)
            ?? db.Countries.Add(new Country { Name = country, IsoCode = iso }).Entity;

        db.Beans.Add(new Bean
        {
            Name = name,
            Description = $"{name} description",
            ImageUrl = "https://example.com/img.png",
            Cost = 10m,
            CurrencyCode = "GBP",
            Colour = colour,
            Country = countryEntity,
        });
        await db.SaveChangesAsync();
    }
}
