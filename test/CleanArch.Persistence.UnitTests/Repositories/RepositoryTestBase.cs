using CleanArch.CrossCuttingConcerns.Identity;
using CleanArch.CrossCuttingConcerns.OS;
using CleanArch.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Persistence.UnitTests.Repositories;

/// <summary>Fresh in-memory <see cref="ApplicationDbContext"/> per test, with HasData seed applied.</summary>
public abstract class RepositoryTestBase
{
    protected static readonly DateTime FixedUtcNow = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    protected ApplicationDbContext DbContext { get; private set; }

    [SetUp]
    public void CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"CleanArch-{Guid.NewGuid()}")
            .Options;

        DbContext = new ApplicationDbContext(options, new FakeLoggedInUser(), new FakeClock());
        DbContext.Database.EnsureCreated();
    }

    [TearDown]
    public void DisposeContext() => DbContext.Dispose();

    private sealed class FakeLoggedInUser : ILoggedInUserService
    {
        public bool IsAuthenticated => true;
        public string UserId => "unit-test-user";
    }

    private sealed class FakeClock : IDateTimeProvider
    {
        public DateTime Now => FixedUtcNow;
        public DateTime UtcNow => FixedUtcNow;
        public DateTimeOffset OffsetNow => FixedUtcNow;
        public DateTimeOffset OffsetUtcNow => FixedUtcNow;
    }
}
