using Coffee4You.Server.Data.Seed;
using FluentAssertions;

namespace Coffee4You.Server.Tests.Unit;

public class JsonSeederTests
{
    [Theory]
    [InlineData("£39.26", 39.26, "GBP")]
    [InlineData("£0.99", 0.99, "GBP")]
    [InlineData("$5", 5.00, "USD")]
    [InlineData("$12.50", 12.50, "USD")]
    [InlineData("€10.5", 10.50, "EUR")]
    [InlineData("  £18.57  ", 18.57, "GBP")]
    public void ParseCost_ValidInputs_ReturnsParsedAmountAndCurrency(
        string input,
        double expectedCost,
        string expectedCurrency)
    {
        var (cost, currency) = JsonSeeder.ParseCost(input);

        cost.Should().Be((decimal)expectedCost);
        currency.Should().Be(expectedCurrency);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("free")]
    [InlineData("£abc")]
    [InlineData("39.26")]   // missing symbol
    [InlineData("£ 39 .26")]   // stray spaces inside number
    public void ParseCost_InvalidInputs_ReturnsZeroGbp(string input)
    {
        var (cost, currency) = JsonSeeder.ParseCost(input);

        cost.Should().Be(0m);
        currency.Should().Be("GBP");
    }

    [Fact]
    public void ParseCost_Null_ReturnsZeroGbp()
    {
        var (cost, currency) = JsonSeeder.ParseCost(null);

        cost.Should().Be(0m);
        currency.Should().Be("GBP");
    }
}
