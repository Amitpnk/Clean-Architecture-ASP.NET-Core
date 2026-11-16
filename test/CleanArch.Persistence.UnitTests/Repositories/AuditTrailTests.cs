using CleanArch.CrossCuttingConcerns.Identity;
using CleanArch.CrossCuttingConcerns.OS;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Enum;
using CleanArch.Persistence.Context;
using CleanArch.Persistence.Repositories;
using CleanArch.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;

namespace CleanArch.Persistence.UnitTests.Repositories;

public class AuditTrailTests : RepositoryTestBase
{
    [Test]
    public async Task UpdateAsync_WritesUpdateAudit_WithOldAndNewValues()
    {
        var repository = new GenericRepository<Category>(DbContext);
        var category = await repository.AddAsync(new Category { Id = Guid.NewGuid(), Name = "Before" });
        DbContext.ChangeTracker.Clear();

        category.Name = "After";
        await repository.UpdateAsync(category);

        var audit = await DbContext.AuditTrails.SingleAsync(a => a.Type == AuditType.Update.ToString());
        var oldValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(audit.OldValues);
        var newValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(audit.NewValues);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(audit.TableName, Is.EqualTo(nameof(Category)));
            Assert.That(audit.PrimaryKey, Does.Contain(category.Id.ToString()));
            Assert.That(newValues["Name"], Is.EqualTo("After"));
            Assert.That(oldValues, Does.ContainKey("Name"));
            Assert.That(audit.AffectedColumns, Does.Contain("Name"));
        }
    }

    [Test]
    public async Task DeleteAsync_WritesDeleteAudit_WithOldValuesOnly()
    {
        var repository = new GenericRepository<Category>(DbContext);
        var category = await repository.AddAsync(new Category { Id = Guid.NewGuid(), Name = "Doomed" });
        DbContext.ChangeTracker.Clear();

        await repository.DeleteAsync(category);

        var audit = await DbContext.AuditTrails.SingleAsync(a => a.Type == AuditType.Delete.ToString());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(audit.OldValues, Does.Contain("Doomed"));
            Assert.That(audit.NewValues, Is.Null);
            Assert.That(audit.AffectedColumns, Is.Null);
        }
    }

    [Test]
    public async Task SaveChanges_WithoutLoggedInUser_SkipsAuditTrail()
    {
        var anonymous = new Mock<ILoggedInUserService>();
        var clock = new Mock<IDateTimeProvider>();
        clock.SetupGet(c => c.UtcNow).Returns(FixedUtcNow);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"anon-{Guid.NewGuid()}")
            .Options;
        await using var context = new ApplicationDbContext(options, anonymous.Object, clock.Object);

        var added = await new GenericRepository<Category>(context).AddAsync(new Category { Id = Guid.NewGuid(), Name = "Anon" });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await context.AuditTrails.CountAsync(), Is.Zero);
            Assert.That(added.CreatedBy, Is.Null);
            Assert.That(added.CreatedDate, Is.EqualTo(FixedUtcNow));
        }
    }
}

public class PersistenceServiceRegistrationTests
{
    [Test]
    public void AddPersistenceServices_RegistersRepositories()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { ["ConnectionStrings:CleanArchConnectionString"] = "Server=unused" })
            .Build();

        var services = new ServiceCollection()
            .AddPersistenceServices(configuration);

        var registered = services.Select(d => (d.ServiceType, d.ImplementationType)).ToList();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(registered, Does.Contain((typeof(ICategoryRepository), typeof(CategoryRepository))));
            Assert.That(registered, Does.Contain((typeof(IEventRepository), typeof(EventRepository))));
            Assert.That(registered, Does.Contain((typeof(IGenericRepositoryAsync<>), typeof(GenericRepository<>))));
            Assert.That(services.Any(d => d.ServiceType == typeof(ApplicationDbContext)), Is.True);
        }
    }

    [Test]
    public void AddIdentityServices_RegistersAuthenticationService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { ["JwtSettings:Key"] = "k" })
            .Build();

        var services = new ServiceCollection()
            .AddIdentityServices(configuration);

        Assert.That(services.Any(d =>
            d.ServiceType == typeof(IAuthenticationService) &&
            d.ImplementationType == typeof(AuthenticationService)), Is.True);
    }
}
