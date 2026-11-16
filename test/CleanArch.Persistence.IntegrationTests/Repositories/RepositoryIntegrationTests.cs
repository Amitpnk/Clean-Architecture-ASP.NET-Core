using CleanArch.Domain.Entities;
using CleanArch.Domain.Enum;
using CleanArch.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Persistence.IntegrationTests.Repositories;

public class MigrationTests(SqlServerContainer sqlServer) : DatabaseTestBase(sqlServer)
{
    [Fact]
    public async Task Migrations_AreAllApplied()
    {
        var pending = await DbContext.Database.GetPendingMigrationsAsync(CancellationToken);

        Assert.Empty(pending);
    }

    [Fact]
    public async Task Migrations_SeedApplicationAndIdentityData()
    {
        Assert.Equal(3, await DbContext.Categories.CountAsync(CancellationToken));
        Assert.Equal(4, await DbContext.Events.CountAsync(CancellationToken));
        Assert.Equal(4, await DbContext.Set<IdentityRole>().CountAsync(CancellationToken));
        Assert.Equal(2, await DbContext.Users.CountAsync(CancellationToken));
    }
}

public class GenericRepositoryIntegrationTests(SqlServerContainer sqlServer) : DatabaseTestBase(sqlServer)
{
    [Fact]
    public async Task AddAsync_PersistsEntity_StampsAuditColumns_AndWritesAuditTrail()
    {
        var repository = new GenericRepository<Category>(DbContext);

        var added = await repository.AddAsync(new Category { Name = "Workshops", Description = "Hands-on" });

        DbContext.ChangeTracker.Clear();
        var reloaded = await repository.GetByIdAsync(added.Id);
        var audit = await DbContext.AuditTrails.SingleAsync(a => a.TableName == nameof(Category), CancellationToken);

        Assert.NotEqual(Guid.Empty, added.Id);
        Assert.NotNull(reloaded);
        Assert.Equal("Workshops", reloaded.Name);
        Assert.Equal(TestUserId, reloaded.CreatedBy);
        Assert.Equal(FixedUtcNow, reloaded.CreatedDate);
        Assert.Equal(AuditType.Create.ToString(), audit.Type);
        Assert.Equal(TestUserId, audit.UserId);
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges_AndStampsModifiedColumns()
    {
        var repository = new GenericRepository<Category>(DbContext);
        var category = await repository.AddAsync(new Category { Name = "Before" });
        DbContext.ChangeTracker.Clear();

        category.Name = "After";
        await repository.UpdateAsync(category);

        DbContext.ChangeTracker.Clear();
        var reloaded = await repository.GetByIdAsync(category.Id);

        Assert.Equal("After", reloaded.Name);
        Assert.Equal(TestUserId, reloaded.LastModifiedBy);
        Assert.Equal(FixedUtcNow, reloaded.LastModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        var repository = new GenericRepository<Category>(DbContext);
        var category = await repository.AddAsync(new Category { Name = "Temporary" });
        DbContext.ChangeTracker.Clear();

        await repository.DeleteAsync(category);

        DbContext.ChangeTracker.Clear();
        Assert.Null(await repository.GetByIdAsync(category.Id));
    }

    [Fact]
    public async Task GetPagedReponseAsync_ReturnsRequestedPage()
    {
        var repository = new GenericRepository<Category>(DbContext);

        var page = await repository.GetPagedReponseAsync(page: 2, size: 2);

        Assert.Single(page);
    }
}

public class CategoryRepositoryIntegrationTests(SqlServerContainer sqlServer) : DatabaseTestBase(sqlServer)
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
        var category = await DbContext.Categories.FirstAsync(CancellationToken);
        DbContext.Events.Add(new Event { Name = "Past", Date = DateTime.Today.AddDays(-1), CategoryId = category.Id });
        DbContext.Events.Add(new Event { Name = "Future", Date = DateTime.Today.AddDays(1), CategoryId = category.Id });
        await DbContext.SaveChangesAsync(CancellationToken);
        DbContext.ChangeTracker.Clear();

        var repository = new CategoryRepository(DbContext);

        var upcoming = (await repository.GetCategoriesWithEvents(includePassedEvents: false)).Single(c => c.Id == category.Id);
        var all = (await repository.GetCategoriesWithEvents(includePassedEvents: true)).Single(c => c.Id == category.Id);

        Assert.DoesNotContain("Past", upcoming.Events.Select(e => e.Name));
        Assert.Contains("Future", upcoming.Events.Select(e => e.Name));
        Assert.Contains("Past", all.Events.Select(e => e.Name));
    }
}

public class EventRepositoryIntegrationTests(SqlServerContainer sqlServer) : DatabaseTestBase(sqlServer)
{
    [Fact]
    public async Task IsEventNameAndDateUnique_TranslatesDateComparison_OnSqlServer()
    {
        var seeded = await DbContext.Events.FirstAsync(CancellationToken);
        var repository = new EventRepository(DbContext);

        Assert.True(await repository.IsEventNameAndDateUnique(seeded.Name, seeded.Date.Date.AddHours(3)));
        Assert.False(await repository.IsEventNameAndDateUnique(seeded.Name, seeded.Date.AddDays(1)));
    }
}
