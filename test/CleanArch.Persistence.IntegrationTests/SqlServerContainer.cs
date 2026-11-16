using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace CleanArch.Persistence.IntegrationTests;

/// <summary>
/// Starts one SQL Server container for the whole assembly and applies the EF Core migrations to it,
/// so every test runs against the real schema the API ships with. Requires a running Docker engine.
/// </summary>
[SetUpFixture]
public sealed class SqlServerContainer
{
    private MsSqlContainer _container;

    public static string ConnectionString { get; private set; }

    [OneTimeSetUp]
    public async Task StartAsync()
    {
        _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();

        await _container.StartAsync();

        ConnectionString = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = "CleanArchPersistenceTests"
        }.ConnectionString;

        await using var context = DatabaseTestBase.CreateContext(ConnectionString);
        await context.Database.MigrateAsync();
    }

    [OneTimeTearDown]
    public async Task StopAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }
}
