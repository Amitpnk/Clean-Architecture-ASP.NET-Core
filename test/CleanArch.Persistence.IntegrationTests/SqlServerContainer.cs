using CleanArch.Persistence.IntegrationTests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

// One container for the whole assembly. Tests share one database (isolated by rolled-back transactions),
// so xunit.runner.json turns off parallel runs: concurrent transactions would block on each other's rows.
[assembly: AssemblyFixture(typeof(SqlServerContainer))]

namespace CleanArch.Persistence.IntegrationTests;

/// <summary>
/// Starts a SQL Server container and applies the EF Core migrations to it, so every test runs against
/// the real schema the API ships with. Requires a running Docker engine.
/// </summary>
public sealed class SqlServerContainer : IAsyncLifetime
{
    private readonly MsSqlContainer _container =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();

    public string ConnectionString { get; private set; }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        ConnectionString = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = "CleanArchPersistenceTests"
        }.ConnectionString;

        await using var context = DatabaseTestBase.CreateContext(ConnectionString);
        await context.Database.MigrateAsync();
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
