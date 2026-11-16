using CleanArch.Domain.Entities;
using CleanArch.Domain.Enum;
using CleanArch.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Persistence.UnitTests.Repositories;

public class SeedDataTests : RepositoryTestBase
{
    [Fact]
    public async Task Model_SeedsApplicationAndIdentityData()
    {
        Assert.Equal(3, await DbContext.Categories.CountAsync(Ct));
        Assert.Equal(4, await DbContext.Events.CountAsync(Ct));
        Assert.Equal(4, await DbContext.Set<IdentityRole>().CountAsync(Ct));
        Assert.Equal(2, await DbContext.Users.CountAsync(Ct));
    }

    [Fact]
    public async Task SeededRoles_UseUpperCaseNormalizedNames_SoRoleManagerCanFindThem()
    {
        var basic = await DbContext.Set<IdentityRole>().SingleAsync(r => r.Id == Constants.Basic, Ct);

        Assert.Equal("BASIC", basic.NormalizedName);
    }

    [Fact]
    public async Task SeededUsers_HaveSecurityStamp()
    {
        var users = await DbContext.Users.ToListAsync(Ct);

        Assert.All(users, u => Assert.False(string.IsNullOrEmpty(u.SecurityStamp)));
    }
}

public class GenericRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task AddAsync_PersistsEntity_AndStampsAuditColumns()
    {
        var repository = new GenericRepository<Category>(DbContext);

        var added = await repository.AddAsync(new Category { Id = Guid.NewGuid(), Name = "Workshops" });

        var reloaded = await repository.GetByIdAsync(added.Id);
        var audit = await DbContext.AuditTrails.SingleAsync(Ct);
        Assert.NotNull(reloaded);
        Assert.Equal("unit-test-user", reloaded.CreatedBy);
        Assert.Equal(FixedUtcNow, reloaded.CreatedDate);
        Assert.Equal(nameof(Category), audit.TableName);
        Assert.Equal(AuditType.Create.ToString(), audit.Type);
        Assert.Equal("unit-test-user", audit.UserId);
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        var repository = new GenericRepository<Category>(DbContext);
        var category = await repository.AddAsync(new Category { Id = Guid.NewGuid(), Name = "Temporary" });

        await repository.DeleteAsync(category);

        Assert.Null(await repository.GetByIdAsync(category.Id));
    }

    [Fact]
    public async Task GetPagedReponseAsync_ReturnsRequestedPage()
    {
        var repository = new GenericRepository<Category>(DbContext);

        var page = await repository.GetPagedReponseAsync(page: 2, size: 2);

        Assert.Single(page);
    }

    [Fact]
    public async Task ListAllAsync_ReturnsEverySeededRow()
    {
        var repository = new GenericRepository<Event>(DbContext);

        Assert.Equal(4, (await repository.ListAllAsync()).Count);
    }
}

public class CategoryRepositoryTests : RepositoryTestBase
{
    [Theory]
    [InlineData("Concert", true)]
    [InlineData("Unknown", false)]
    public async Task IsCategoryName_MatchesExistingNames(string name, bool expected)
    {
        var repository = new CategoryRepository(DbContext);

        Assert.Equal(expected, await repository.IsCategoryName(name));
    }

    [Fact]
    public async Task GetCategoriesWithEvents_ExcludesPastEvents_UnlessHistoryRequested()
    {
        var category = await DbContext.Categories.FirstAsync(Ct);
        DbContext.Events.Add(new Event { Id = Guid.NewGuid(), Name = "Past", Date = DateTime.Today.AddDays(-1), CategoryId = category.Id });
        DbContext.Events.Add(new Event { Id = Guid.NewGuid(), Name = "Future", Date = DateTime.Today.AddDays(1), CategoryId = category.Id });
        await DbContext.SaveChangesAsync(Ct);
        DbContext.ChangeTracker.Clear();

        var repository = new CategoryRepository(DbContext);

        var upcoming = (await repository.GetCategoriesWithEvents(includePassedEvents: false)).Single(c => c.Id == category.Id);
        var all = (await repository.GetCategoriesWithEvents(includePassedEvents: true)).Single(c => c.Id == category.Id);

        Assert.DoesNotContain("Past", upcoming.Events.Select(e => e.Name));
        Assert.Contains("Future", upcoming.Events.Select(e => e.Name));
        Assert.Contains("Past", all.Events.Select(e => e.Name));
    }
}

public class EventRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task IsEventNameAndDateUnique_DetectsSameNameOnSameDay()
    {
        var seeded = await DbContext.Events.FirstAsync(Ct);
        var repository = new EventRepository(DbContext);

        Assert.True(await repository.IsEventNameAndDateUnique(seeded.Name, seeded.Date.AddHours(3)));
        Assert.False(await repository.IsEventNameAndDateUnique(seeded.Name, seeded.Date.AddDays(1)));
    }
}
