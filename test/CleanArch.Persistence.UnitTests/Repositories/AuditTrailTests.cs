using CleanArch.Application.Contracts.Persistence;
using CleanArch.CrossCuttingConcerns.Identity;
using CleanArch.CrossCuttingConcerns.OS;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Enum;
using CleanArch.Persistence.Context;
using CleanArch.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;

namespace CleanArch.Persistence.UnitTests.Repositories;

public class AuditTrailTests : RepositoryTestBase
{
    [Fact]
    public async Task UpdateAsync_WritesUpdateAudit_WithOldAndNewValues()
    {
        var repository = new GenericRepository<Category>(DbContext);
        var category = await repository.AddAsync(new Category { Id = Guid.NewGuid(), Name = "Before" });
        DbContext.ChangeTracker.Clear();

        category.Name = "After";
        await repository.UpdateAsync(category);

        var audit = await DbContext.AuditTrails.SingleAsync(a => a.Type == AuditType.Update.ToString(), Ct);
        var oldValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(audit.OldValues);
        var newValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(audit.NewValues);
        Assert.Equal(nameof(Category), audit.TableName);
        Assert.Contains(category.Id.ToString(), audit.PrimaryKey);
        Assert.Equal("After", newValues["Name"]);
        Assert.Contains("Name", oldValues.Keys);
        Assert.Contains("Name", audit.AffectedColumns);
    }

    [Fact]
    public async Task DeleteAsync_WritesDeleteAudit_WithOldValuesOnly()
    {
        var repository = new GenericRepository<Category>(DbContext);
        var category = await repository.AddAsync(new Category { Id = Guid.NewGuid(), Name = "Doomed" });
        DbContext.ChangeTracker.Clear();

        await repository.DeleteAsync(category);

        var audit = await DbContext.AuditTrails.SingleAsync(a => a.Type == AuditType.Delete.ToString(), Ct);
        Assert.Contains("Doomed", audit.OldValues);
        Assert.Null(audit.NewValues);
        Assert.Null(audit.AffectedColumns);
    }

    [Fact]
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

        Assert.Equal(0, await context.AuditTrails.CountAsync(Ct));
        Assert.Null(added.CreatedBy);
        Assert.Equal(FixedUtcNow, added.CreatedDate);
    }
}

public class PersistenceServiceRegistrationTests
{
    [Fact]
    public void AddPersistenceServices_RegistersRepositories()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { ["ConnectionStrings:CleanArchConnectionString"] = "Server=unused" })
            .Build();

        var services = new ServiceCollection().AddPersistenceServices(configuration);

        var registered = services.Select(d => (d.ServiceType, d.ImplementationType)).ToList();
        Assert.Contains((typeof(ICategoryRepository), typeof(CategoryRepository)), registered);
        Assert.Contains((typeof(IEventRepository), typeof(EventRepository)), registered);
        Assert.Contains((typeof(IGenericRepositoryAsync<>), typeof(GenericRepository<>)), registered);
        Assert.Contains(services, d => d.ServiceType == typeof(ApplicationDbContext));
    }

    [Fact]
    public void AddIdentityServices_RegistersAuthenticationService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { ["JwtSettings:Key"] = "k" })
            .Build();

        var services = new ServiceCollection().AddIdentityServices(configuration);

        Assert.Contains(services, d =>
            d.ServiceType == typeof(IAuthenticationService) &&
            d.ImplementationType == typeof(AuthenticationService));
    }
}
