using System.Net.Http.Headers;
using System.Net.Http.Json;
using CleanArch.Domain.Common;
using CleanArch.Domain.Services.Authentication;

namespace CleanArch.Api.IntegrationTests;

/// <summary>One SQL Server container and one in-process API host for the whole test assembly.</summary>
[SetUpFixture]
public sealed class ApiTestFixture
{
    // Seeded by IdentityContext (see DefaultUser): superadmin@gmail.com / Password@123.
    public const string SeededEmail = "superadmin@gmail.com";
    public const string SeededPassword = "Password@123";

    public static CleanArchApiFactory Factory { get; private set; }

    [OneTimeSetUp]
    public async Task StartAsync()
    {
        Factory = new CleanArchApiFactory();
        await Factory.StartContainerAsync();

        // Forces the host to start, which runs the migrations in Program.cs.
        using var _ = Factory.CreateClient();
    }

    [OneTimeTearDown]
    public async Task StopAsync()
    {
        if (Factory is not null)
        {
            await Factory.DisposeAsync();
        }
    }

    public static HttpClient CreateAnonymousClient() => Factory.CreateClient();

    public static async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/account/authenticate",
            new AuthenticationRequest { Email = SeededEmail, Password = SeededPassword });
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<BaseResponse<AuthenticationResponse>>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Data.JWToken);
        return client;
    }
}
