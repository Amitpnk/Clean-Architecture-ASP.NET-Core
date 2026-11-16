using CleanArch.CrossCuttingConcerns.Identity;
using CleanArch.CrossCuttingConcerns.OS;
using CleanArch.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArch.Persistence.IntegrationTests;

/// <summary>
/// Gives each test a fresh <see cref="ApplicationDbContext"/> inside a transaction that is rolled back
/// afterwards, so tests share the migrated database without seeing each other's writes.
/// </summary>
public abstract class DatabaseTestBase
{
    protected const string TestUserId = "integration-test-user";
    protected static readonly DateTime FixedUtcNow = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private IDbContextTransaction _transaction;

    protected ApplicationDbContext DbContext { get; private set; }

    [SetUp]
    public async Task BeginTransactionAsync()
    {
        DbContext = CreateContext(SqlServerContainer.ConnectionString);
        _transaction = await DbContext.Database.BeginTransactionAsync();
    }

    [TearDown]
    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        await DbContext.DisposeAsync();
    }

    // No EnableRetryOnFailure here: the retrying execution strategy rejects user-initiated transactions.
    internal static ApplicationDbContext CreateContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new ApplicationDbContext(options, new FakeLoggedInUser(), new FakeClock());
    }

    private sealed class FakeLoggedInUser : ILoggedInUserService
    {
        public bool IsAuthenticated => true;
        public string UserId => TestUserId;
    }

    private sealed class FakeClock : IDateTimeProvider
    {
        public DateTime Now => FixedUtcNow;
        public DateTime UtcNow => FixedUtcNow;
        public DateTimeOffset OffsetNow => FixedUtcNow;
        public DateTimeOffset OffsetUtcNow => FixedUtcNow;
    }
}
