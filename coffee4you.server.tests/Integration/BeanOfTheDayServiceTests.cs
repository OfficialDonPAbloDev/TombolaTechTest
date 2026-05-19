using Coffee4You.Server.Domain.Entities;
using Coffee4You.Server.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Coffee4You.Server.Tests.Integration;

public class BeanOfTheDayServiceTests
{
    [Fact]
    public async Task GetOrSelectTodayAsync_ReturnsExistingPick_WhenTodayAlreadySelected()
    {
        var (db, conn) = TestDb.Create();
        using var _ = conn;
        await using var __ = db;

        var beanA = await TestDb.AddBeanAsync(db, "Alpha");
        var beanB = await TestDb.AddBeanAsync(db, "Beta");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        db.BeansOfTheDay.Add(new BeanOfTheDay
        {
            BeanId = beanA.Id,
            SelectedFor = today,
            SelectedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        var sut = new BeanOfTheDayService(db);
        var result = await sut.GetOrSelectTodayAsync();

        result.Should().NotBeNull();
        result!.Id.Should().Be(beanA.Id);

        // Service must not have inserted a duplicate row for today.
        var rowsForToday = await db.BeansOfTheDay.CountAsync(b => b.SelectedFor == today);
        rowsForToday.Should().Be(1);
    }

    [Fact]
    public async Task GetOrSelectTodayAsync_PicksAndPersists_WhenNoneToday()
    {
        var (db, conn) = TestDb.Create();
        using var _ = conn;
        await using var __ = db;

        var beanA = await TestDb.AddBeanAsync(db, "Alpha");
        var beanB = await TestDb.AddBeanAsync(db, "Beta");

        var sut = new BeanOfTheDayService(db);
        var result = await sut.GetOrSelectTodayAsync();

        result.Should().NotBeNull();
        new[] { beanA.Id, beanB.Id }.Should().Contain(result!.Id);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var saved = await db.BeansOfTheDay.SingleAsync(b => b.SelectedFor == today);
        saved.BeanId.Should().Be(result.Id);
    }

    [Fact]
    public async Task GetOrSelectTodayAsync_ExcludesYesterdaysBean_WhenAlternativesExist()
    {
        var (db, conn) = TestDb.Create();
        using var _ = conn;
        await using var __ = db;

        var beanA = await TestDb.AddBeanAsync(db, "Alpha");
        var beanB = await TestDb.AddBeanAsync(db, "Beta");
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
        db.BeansOfTheDay.Add(new BeanOfTheDay
        {
            BeanId = beanA.Id,
            SelectedFor = yesterday,
            SelectedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        var sut = new BeanOfTheDayService(db);
        var result = await sut.GetOrSelectTodayAsync();

        result.Should().NotBeNull();
        result!.Id.Should().Be(beanB.Id);
    }

    [Fact]
    public async Task GetOrSelectTodayAsync_RelaxesRule_WhenOnlyYesterdaysBeanExists()
    {
        var (db, conn) = TestDb.Create();
        using var _ = conn;
        await using var __ = db;

        var beanA = await TestDb.AddBeanAsync(db, "Alpha");
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
        db.BeansOfTheDay.Add(new BeanOfTheDay
        {
            BeanId = beanA.Id,
            SelectedFor = yesterday,
            SelectedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        var sut = new BeanOfTheDayService(db);
        var result = await sut.GetOrSelectTodayAsync();

        result.Should().NotBeNull();
        result!.Id.Should().Be(beanA.Id);
    }

    [Fact]
    public async Task GetOrSelectTodayAsync_ReturnsNull_WhenCatalogueEmpty()
    {
        var (db, conn) = TestDb.Create();
        using var _ = conn;
        await using var __ = db;

        var sut = new BeanOfTheDayService(db);
        var result = await sut.GetOrSelectTodayAsync();

        result.Should().BeNull();
    }
}
