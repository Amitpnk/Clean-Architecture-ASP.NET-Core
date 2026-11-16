using System.Net.Http.Headers;
using System.Net.Http.Json;
using CleanArch.Api.IntegrationTests;
using CleanArch.Domain.Common;
using CleanArch.Domain.Services.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

// One SQL Server container and one in-process API host, shared by every test in the assembly.
[assembly: AssemblyFixture(typeof(CleanArchApiFactory))]

namespace CleanArch.Api.IntegrationTests;

/// <summary>
/// Hosts the real API in-process (<see cref="WebApplicationFactory{TEntryPoint}"/>) against a throwaway
/// SQL Server container. Program.cs applies the EF Core migrations on startup, so the database comes up
/// with the same schema and seed data as a real deployment. Requires a running Docker engine.
/// </summary>
public sealed class CleanArchApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Seeded by IdentityContext (see DefaultUser).
    public const string SeededEmail = "superadmin@gmail.com";
    public const string SeededPassword = "Password@123";

    private readonly MsSqlContainer _sqlServer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();

    private string _connectionString;

    public async ValueTask InitializeAsync()
    {
        await _sqlServer.StartAsync();

        _connectionString = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "CleanArchApiTests"
        }.ConnectionString;

        // Forces the host to start, which runs the migrations in Program.cs.
        using var _ = CreateClient();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // UseSetting (not ConfigureAppConfiguration) so the values are visible while Program.cs
        // builds its service collection, not only after the host is built.
        builder.UseSetting("ConnectionStrings:CleanArchConnectionString", _connectionString);
        builder.UseSetting("ConnectionStrings:CleanArchIdentityConnectionString", _connectionString);
        builder.UseEnvironment("Development");
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(CancellationToken cancellationToken = default)
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/api/account/authenticate",
            new AuthenticationRequest { Email = SeededEmail, Password = SeededPassword }, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<BaseResponse<AuthenticationResponse>>(cancellationToken);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Data.JWToken);
        return client;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _sqlServer.DisposeAsync();
    }
}
