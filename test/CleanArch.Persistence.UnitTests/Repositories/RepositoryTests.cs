using CleanArch.Domain.Entities;
using CleanArch.Domain.Enum;
using CleanArch.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Persistence.UnitTests.Repositories;

public class SeedDataTests : RepositoryTestBase
{
    [Test]
    public async Task Model_SeedsApplicationAndIdentityData()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await DbContext.Categories.CountAsync(), Is.EqualTo(3));
            Assert.That(await DbContext.Events.CountAsync(), Is.EqualTo(4));
            Assert.That(await DbContext.Set<IdentityRole>().CountAsync(), Is.EqualTo(4));
            Assert.That(await DbContext.Users.CountAsync(), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task SeededRoles_UseUpperCaseNormalizedNames_SoRoleManagerCanFindThem()
    {
        var basic = await DbContext.Set<IdentityRole>().SingleAsync(r => r.Id == Constants.Basic);

        Assert.That(basic.NormalizedName, Is.EqualTo("BASIC"));
    }

    [Test]
    public async Task SeededUsers_HaveSecurityStamp()
    {
        var users = await DbContext.Users.ToListAsync();

        Assert.That(users.Select(u => u.SecurityStamp), Is.All.Not.Null.And.Not.Empty);
    }
}

public class GenericRepositoryTests : RepositoryTestBase
{
    [Test]
    public async Task AddAsync_PersistsEntity_AndStampsAuditColumns()
    {
        var repository = new GenericRepository<Category>(DbContext);

        var added = await repository.AddAsync(new Category { Id = Guid.NewGuid(), Name = "Workshops" });

        var reloaded = await repository.GetByIdAsync(added.Id);
        var audit = await DbContext.AuditTrails.SingleAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(reloaded, Is.Not.Null);
            Assert.That(reloaded.CreatedBy, Is.EqualTo("unit-test-user"));
            Assert.That(reloaded.CreatedDate, Is.EqualTo(FixedUtcNow));
            Assert.That(audit.TableName, Is.EqualTo(nameof(Category)));
            Assert.That(audit.Type, Is.EqualTo(AuditType.Create.ToString()));
            Assert.That(audit.UserId, Is.EqualTo("unit-test-user"));
        }
    }

    [Test]
    public async Task DeleteAsync_RemovesEntity()
    {
        var repository = new GenericRepository<Category>(DbContext);
        var category = await repository.AddAsync(new Category { Id = Guid.NewGuid(), Name = "Temporary" });

        await repository.DeleteAsync(category);

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

public class CategoryRepositoryTests : RepositoryTestBase
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
        var category = DbContext.Categories.First();
        DbContext.Events.Add(new Event { Id = Guid.NewGuid(), Name = "Past", Date = DateTime.Today.AddDays(-1), CategoryId = category.Id });
        DbContext.Events.Add(new Event { Id = Guid.NewGuid(), Name = "Future", Date = DateTime.Today.AddDays(1), CategoryId = category.Id });
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

public class EventRepositoryTests : RepositoryTestBase
{
    [Test]
    public async Task IsEventNameAndDateUnique_DetectsSameNameOnSameDay()
    {
        var seeded = await DbContext.Events.FirstAsync();
        var repository = new EventRepository(DbContext);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await repository.IsEventNameAndDateUnique(seeded.Name, seeded.Date.AddHours(3)), Is.True);
            Assert.That(await repository.IsEventNameAndDateUnique(seeded.Name, seeded.Date.AddDays(1)), Is.False);
        }
    }
}
