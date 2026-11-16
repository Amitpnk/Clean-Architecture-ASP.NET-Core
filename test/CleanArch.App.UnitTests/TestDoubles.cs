using System.Net;
using System.Text;
using System.Text.Json;
using CleanArch.App.Services;

namespace CleanArch.App.UnitTests;

internal sealed class InMemoryTokenStore(string token = null) : ITokenStore
{
    public string Token { get; private set; } = token;

    public ValueTask<string> GetTokenAsync() => ValueTask.FromResult(Token);

    public ValueTask SetTokenAsync(string token)
    {
        Token = token;
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveTokenAsync()
    {
        Token = null;
        return ValueTask.CompletedTask;
    }
}

/// <summary>Returns a canned response and records the request it was given.</summary>
internal sealed class StubHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
{
    public HttpRequestMessage LastRequest { get; private set; }

    public static StubHttpHandler Json(HttpStatusCode status, string json, string mediaType = "application/json") =>
        new(_ => new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, mediaType) });

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        return Task.FromResult(respond(request));
    }
}

internal sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}

internal static class TestJwt
{
    public static readonly DateTimeOffset Now = new(2026, 11, 16, 12, 0, 0, TimeSpan.Zero);

    /// <summary>Builds an unsigned JWT shaped like the ones CleanArch.Api issues.</summary>
    public static string Create(DateTimeOffset expires, params string[] roles)
    {
        var payload = new Dictionary<string, object>
        {
            ["sub"] = "superadmin",
            ["email"] = "superadmin@gmail.com",
            ["uid"] = "user-1",
            ["roles"] = roles,
            ["exp"] = expires.ToUnixTimeSeconds()
        };

        return $"{Encode("{\"alg\":\"HS256\",\"typ\":\"JWT\"}")}.{Encode(JsonSerializer.Serialize(payload))}.signature";
    }

    private static string Encode(string json) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(json)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
