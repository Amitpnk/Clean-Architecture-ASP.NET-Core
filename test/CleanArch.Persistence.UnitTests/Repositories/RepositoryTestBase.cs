using CleanArch.CrossCuttingConcerns.Identity;
using CleanArch.CrossCuttingConcerns.OS;
using CleanArch.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Persistence.UnitTests.Repositories;

/// <summary>
/// xUnit creates a new instance per test, so each test gets a fresh in-memory
/// <see cref="ApplicationDbContext"/> with the HasData seed applied.
/// </summary>
public abstract class RepositoryTestBase : IDisposable
{
    protected static readonly DateTime FixedUtcNow = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    protected RepositoryTestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"CleanArch-{Guid.NewGuid()}")
            .Options;

        DbContext = new ApplicationDbContext(options, new FakeLoggedInUser(), new FakeClock());
        DbContext.Database.EnsureCreated();
    }

    protected ApplicationDbContext DbContext { get; }

    protected static CancellationToken Ct => TestContext.Current.CancellationToken;

    public void Dispose()
    {
        DbContext.Dispose();
        GC.SuppressFinalize(this);
    }

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
