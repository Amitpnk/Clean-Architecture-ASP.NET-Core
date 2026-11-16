using System.Net.Http.Json;
using CleanArch.Domain.Common;
using CleanArch.Domain.Services.Authentication;

namespace CleanArch.Api.IntegrationTests.Endpoints;

public class AccountEndpointTests
{
    [Test]
    public async Task Authenticate_WithSeededUser_ReturnsJwt()
    {
        using var client = ApiTestFixture.CreateAnonymousClient();

        var response = await client.PostAsJsonAsync("/api/account/authenticate",
            new AuthenticationRequest { Email = ApiTestFixture.SeededEmail, Password = ApiTestFixture.SeededPassword });

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<BaseResponse<AuthenticationResponse>>();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(body!.Success, Is.True);
            Assert.That(body.Data.JWToken, Is.Not.Null.And.Not.Empty);
            Assert.That(body.Data.Roles, Does.Contain("SuperAdmin"));
        }
    }

    [Test]
    public async Task Authenticate_WithWrongPassword_Fails()
    {
        using var client = ApiTestFixture.CreateAnonymousClient();

        var response = await client.PostAsJsonAsync("/api/account/authenticate",
            new AuthenticationRequest { Email = ApiTestFixture.SeededEmail, Password = "wrong-password" });

        Assert.That(response.IsSuccessStatusCode, Is.False);
    }

    [Test]
    public async Task Register_CreatesUser_ThatCannotSignInUntilConfirmed()
    {
        using var client = ApiTestFixture.CreateAnonymousClient();
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var registration = new RegistrationRequest
        {
            FirstName = "Integration",
            LastName = "Test",
            UserName = $"user{suffix}",
            Email = $"user{suffix}@example.com",
            Password = "Password@123"
        };

        var registered = await client.PostAsJsonAsync("/api/account/register", registration);
        var signIn = await client.PostAsJsonAsync("/api/account/authenticate",
            new AuthenticationRequest { Email = registration.Email, Password = registration.Password });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(registered.IsSuccessStatusCode, Is.True, await registered.Content.ReadAsStringAsync());
            Assert.That(signIn.IsSuccessStatusCode, Is.False);
        }
    }
}
