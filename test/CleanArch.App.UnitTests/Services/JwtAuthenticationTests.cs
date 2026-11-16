using System.Net;
using CleanArch.App.Services;

namespace CleanArch.App.UnitTests.Services;

public class JwtParserTests
{
    [Fact]
    public void ValidToken_ProducesIdentityWithNameAndRoles()
    {
        var token = TestJwt.Create(TestJwt.Now.AddHours(1), "SuperAdmin", "Admin");

        var identity = JwtParser.TryCreateIdentity(token, TestJwt.Now);

        Assert.NotNull(identity);
        Assert.True(identity.IsAuthenticated);
        Assert.Equal("superadmin", identity.Name);
        Assert.Equal(["SuperAdmin", "Admin"], identity.FindAll(JwtParser.RoleClaimType).Select(c => c.Value));
    }

    [Fact]
    public void ExpiredToken_ReturnsNull()
    {
        var token = TestJwt.Create(TestJwt.Now.AddSeconds(-1));

        Assert.Null(JwtParser.TryCreateIdentity(token, TestJwt.Now));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-jwt")]
    [InlineData("a.%%%.c")]
    [InlineData("a.bm90LWpzb24.c")]
    public void MalformedToken_ReturnsNull(string token)
    {
        Assert.Null(JwtParser.TryCreateIdentity(token, TestJwt.Now));
    }
}

public class JwtAuthenticationStateProviderTests
{
    [Fact]
    public async Task NoToken_IsAnonymous()
    {
        var provider = new JwtAuthenticationStateProvider(new InMemoryTokenStore(), new FixedTimeProvider(TestJwt.Now));

        var state = await provider.GetAuthenticationStateAsync();

        Assert.False(state.User.Identity!.IsAuthenticated);
    }

    [Fact]
    public async Task ExpiredToken_IsAnonymous_AndRemoved()
    {
        var store = new InMemoryTokenStore(TestJwt.Create(TestJwt.Now.AddMinutes(-5)));
        var provider = new JwtAuthenticationStateProvider(store, new FixedTimeProvider(TestJwt.Now));

        var state = await provider.GetAuthenticationStateAsync();

        Assert.False(state.User.Identity!.IsAuthenticated);
        Assert.Null(store.Token);
    }

    [Fact]
    public async Task SignInThenSignOut_UpdatesStateAndNotifies()
    {
        var store = new InMemoryTokenStore();
        var provider = new JwtAuthenticationStateProvider(store, new FixedTimeProvider(TestJwt.Now));
        var notifications = 0;
        provider.AuthenticationStateChanged += _ => notifications++;

        await provider.SignInAsync(TestJwt.Create(TestJwt.Now.AddHours(1)));
        var signedIn = await provider.GetAuthenticationStateAsync();
        await provider.SignOutAsync();
        var signedOut = await provider.GetAuthenticationStateAsync();

        Assert.Equal("superadmin", signedIn.User.Identity!.Name);
        Assert.False(signedOut.User.Identity!.IsAuthenticated);
        Assert.Null(store.Token);
        Assert.Equal(2, notifications);
    }
}

public class AuthorizedHandlerTests
{
    private static (HttpClient Client, StubHttpHandler Inner, InMemoryTokenStore Store) Create(string token, HttpStatusCode status)
    {
        var store = new InMemoryTokenStore(token);
        var provider = new JwtAuthenticationStateProvider(store, new FixedTimeProvider(TestJwt.Now));
        var inner = new StubHttpHandler(_ => new HttpResponseMessage(status));
        var client = new HttpClient(new AuthorizedHandler(store, provider) { InnerHandler = inner })
        {
            BaseAddress = new Uri("https://api.test/")
        };
        return (client, inner, store);
    }

    [Fact]
    public async Task WithToken_SendsBearerHeader()
    {
        var token = TestJwt.Create(TestJwt.Now.AddHours(1));
        var (client, inner, _) = Create(token, HttpStatusCode.OK);

        await client.GetAsync("api/v1/events", TestContext.Current.CancellationToken);

        Assert.Equal("Bearer", inner.LastRequest.Headers.Authorization?.Scheme);
        Assert.Equal(token, inner.LastRequest.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task WithoutToken_SendsNoAuthorizationHeader()
    {
        var (client, inner, _) = Create(null, HttpStatusCode.OK);

        await client.GetAsync("api/v1/events", TestContext.Current.CancellationToken);

        Assert.Null(inner.LastRequest.Headers.Authorization);
    }

    [Fact]
    public async Task Unauthorized_SignsOut()
    {
        var (client, _, store) = Create(TestJwt.Create(TestJwt.Now.AddHours(1)), HttpStatusCode.Unauthorized);

        await client.GetAsync("api/v1/events", TestContext.Current.CancellationToken);

        Assert.Null(store.Token);
    }
}
