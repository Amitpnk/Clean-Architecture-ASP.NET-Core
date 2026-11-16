using CleanArch.Domain.Entities;
using CleanArch.Domain.Enum;
using CleanArch.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Persistence.IntegrationTests.Repositories;

public class MigrationTests : DatabaseTestBase
{
    [Test]
    public async Task Migrations_AreAllApplied()
    {
        var pending = await DbContext.Database.GetPendingMigrationsAsync();

        Assert.That(pending, Is.Empty);
    }

    [Test]
    public async Task Migrations_SeedApplicationAndIdentityData()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await DbContext.Categories.CountAsync(), Is.EqualTo(3));
            Assert.That(await DbContext.Events.CountAsync(), Is.EqualTo(4));
            Assert.That(await DbContext.Set<IdentityRole>().CountAsync(), Is.EqualTo(4));
            Assert.That(await DbContext.Users.CountAsync(), Is.EqualTo(2));
        }
    }
}

public class GenericRepositoryIntegrationTests : DatabaseTestBase
{
    [Test]
    public async Task AddAsync_PersistsEntity_StampsAuditColumns_AndWritesAuditTrail()
    {
        var repository = new GenericRepository<Category>(DbContext);

        var added = await repository.AddAsync(new Category { Name = "Workshops", Description = "Hands-on" });

        DbContext.ChangeTracker.Clear();
        var reloaded = await repository.GetByIdAsync(added.Id);
        var audit = await DbContext.AuditTrails.SingleAsync(a => a.TableName == nameof(Category));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(added.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(reloaded, Is.Not.Null);
            Assert.That(reloaded.Name, Is.EqualTo("Workshops"));
            Assert.That(reloaded.CreatedBy, Is.EqualTo(TestUserId));
            Assert.That(reloaded.CreatedDate, Is.EqualTo(FixedUtcNow));
            Assert.That(audit.Type, Is.EqualTo(AuditType.Create.ToString()));
            Assert.That(audit.UserId, Is.EqualTo(TestUserId));
        }
    }

    [Test]
    public async Task UpdateAsync_PersistsChanges_AndStampsModifiedColumns()
    {
        var repository = new GenericRepository<Category>(DbContext);
        var category = await repository.AddAsync(new Category { Name = "Before" });
        DbContext.ChangeTracker.Clear();

        category.Name = "After";
        await repository.UpdateAsync(category);

        DbContext.ChangeTracker.Clear();
        var reloaded = await repository.GetByIdAsync(category.Id);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(reloaded.Name, Is.EqualTo("After"));
            Assert.That(reloaded.LastModifiedBy, Is.EqualTo(TestUserId));
            Assert.That(reloaded.LastModifiedDate, Is.EqualTo(FixedUtcNow));
        }
    }

    [Test]
    public async Task DeleteAsync_RemovesEntity()
    {
        var repository = new GenericRepository<Category>(DbContext);
        var category = await repository.AddAsync(new Category { Name = "Temporary" });
        DbContext.ChangeTracker.Clear();

        await repository.DeleteAsync(category);

        DbContext.ChangeTracker.Clear();
        Assert.That(await repository.GetByIdAsync(category.Id), Is.Null);
    }

    [Test]
    public async Task GetPagedReponseAsync_ReturnsRequestedPage()
    {
        var repository = new GenericRepository<Category>(DbContext);

        var page = await repository.GetPagedReponseAsync(page: 2, size: 2);

        Assert.That(page, Has.Count.EqualTo(1));
    }
}

public class CategoryRepositoryIntegrationTests : DatabaseTestBase
{
    [Test]
    public async Task IsCategoryName_MatchesExistingName()
    {
        var repository = new CategoryRepository(DbContext);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await repository.IsCategoryName("Concert"), Is.True);
            Assert.That(await repository.IsCategoryName("Unknown"), Is.False);
        }
    }

    [Test]
    public async Task GetCategoriesWithEvents_ExcludesPastEvents_UnlessHistoryRequested()
    {
        var category = await DbContext.Categories.FirstAsync();
        DbContext.Events.Add(new Event { Name = "Past", Date = DateTime.Today.AddDays(-1), CategoryId = category.Id });
        DbContext.Events.Add(new Event { Name = "Future", Date = DateTime.Today.AddDays(1), CategoryId = category.Id });
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        var repository = new CategoryRepository(DbContext);

        var upcoming = (await repository.GetCategoriesWithEvents(includePassedEvents: false)).Single(c => c.Id == category.Id);
        var all = (await repository.GetCategoriesWithEvents(includePassedEvents: true)).Single(c => c.Id == category.Id);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(upcoming.Events.Select(e => e.Name), Does.Not.Contain("Past"));
            Assert.That(upcoming.Events.Select(e => e.Name), Does.Contain("Future"));
            Assert.That(all.Events.Select(e => e.Name), Does.Contain("Past"));
        }
    }
}

public class EventRepositoryIntegrationTests : DatabaseTestBase
{
    [Test]
    public async Task IsEventNameAndDateUnique_TranslatesDateComparison_OnSqlServer()
    {
        var seeded = await DbContext.Events.FirstAsync();
        var repository = new EventRepository(DbContext);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await repository.IsEventNameAndDateUnique(seeded.Name, seeded.Date.Date.AddHours(3)), Is.True);
            Assert.That(await repository.IsEventNameAndDateUnique(seeded.Name, seeded.Date.AddDays(1)), Is.False);
        }
    }
}
