using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace CleanArch.Api.IntegrationTests;

/// <summary>
/// Hosts the real API in-process (<see cref="WebApplicationFactory{TEntryPoint}"/>) against a throwaway
/// SQL Server container. Program.cs applies the EF Core migrations on startup, so the database comes up
/// with the same schema and seed data as a real deployment. Requires a running Docker engine.
/// </summary>
public sealed class CleanArchApiFactory : WebApplicationFactory<Program>
{
    private readonly MsSqlContainer _sqlServer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();

    private string _connectionString;

    public async Task StartContainerAsync()
    {
        await _sqlServer.StartAsync();

        _connectionString = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "CleanArchApiTests"
        }.ConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // UseSetting (not ConfigureAppConfiguration) so the values are visible while Program.cs
        // builds its service collection, not only after the host is built.
        builder.UseSetting("ConnectionStrings:CleanArchConnectionString", _connectionString);
        builder.UseSetting("ConnectionStrings:CleanArchIdentityConnectionString", _connectionString);
        builder.UseEnvironment("Development");
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _sqlServer.DisposeAsync();
    }
}
