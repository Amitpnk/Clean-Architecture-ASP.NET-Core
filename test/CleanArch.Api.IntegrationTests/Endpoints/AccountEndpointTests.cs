using System.Net.Http.Json;
using CleanArch.Domain.Common;
using CleanArch.Domain.Services.Authentication;

namespace CleanArch.Api.IntegrationTests.Endpoints;

public class AccountEndpointTests(CleanArchApiFactory factory)
{
    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Authenticate_WithSeededUser_ReturnsJwt()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/account/authenticate",
            new AuthenticationRequest { Email = CleanArchApiFactory.SeededEmail, Password = CleanArchApiFactory.SeededPassword },
            CancellationToken);

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<BaseResponse<AuthenticationResponse>>(CancellationToken);
        Assert.True(body!.Success);
        Assert.False(string.IsNullOrEmpty(body.Data.JWToken));
        Assert.Contains("SuperAdmin", body.Data.Roles);
    }

    [Fact]
    public async Task Authenticate_WithWrongPassword_Fails()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/account/authenticate",
            new AuthenticationRequest { Email = CleanArchApiFactory.SeededEmail, Password = "wrong-password" },
            CancellationToken);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Register_CreatesUser_ThatCannotSignInUntilConfirmed()
    {
        using var client = factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var registration = new RegistrationRequest
        {
            FirstName = "Integration",
            LastName = "Test",
            UserName = $"user{suffix}",
            Email = $"user{suffix}@example.com",
            Password = "Password@123"
        };

        var registered = await client.PostAsJsonAsync("/api/account/register", registration, CancellationToken);
        var signIn = await client.PostAsJsonAsync("/api/account/authenticate",
            new AuthenticationRequest { Email = registration.Email, Password = registration.Password }, CancellationToken);

        Assert.True(registered.IsSuccessStatusCode, await registered.Content.ReadAsStringAsync(CancellationToken));
        Assert.False(signIn.IsSuccessStatusCode);
    }
}
