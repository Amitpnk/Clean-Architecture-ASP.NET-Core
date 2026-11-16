using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CleanArch.App.Services;

/// <summary>
/// Reads the claims out of a JWT issued by CleanArch.Api. The signature is not checked here:
/// the browser can't hold the signing key, and the API validates every token it receives.
/// This only decides what the UI shows.
/// </summary>
public static class JwtParser
{
    public const string AuthenticationType = "jwt";
    public const string NameClaimType = "sub";
    public const string RoleClaimType = "roles";

    /// <summary>Returns an authenticated identity, or null when the token is missing, malformed or expired.</summary>
    public static ClaimsIdentity TryCreateIdentity(string token, DateTimeOffset now)
    {
        var claims = TryReadClaims(token);
        if (claims is null)
        {
            return null;
        }

        var expiry = claims.FirstOrDefault(c => c.Type == "exp");
        if (expiry is null || !long.TryParse(expiry.Value, out var expirySeconds) ||
            DateTimeOffset.FromUnixTimeSeconds(expirySeconds) <= now)
        {
            return null;
        }

        return new ClaimsIdentity(claims, AuthenticationType, NameClaimType, RoleClaimType);
    }

    private static List<Claim> TryReadClaims(string token)
    {
        var parts = token?.Split('.');
        if (parts is not { Length: 3 })
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(DecodeBase64Url(parts[1]));
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var claims = new List<Claim>();
            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    claims.AddRange(property.Value.EnumerateArray().Select(v => new Claim(property.Name, ToClaimValue(v))));
                }
                else
                {
                    claims.Add(new Claim(property.Name, ToClaimValue(property.Value)));
                }
            }

            return claims;
        }
        catch (Exception ex) when (ex is FormatException or JsonException or ArgumentException)
        {
            return null;
        }
    }

    private static string ToClaimValue(JsonElement value) =>
        value.ValueKind == JsonValueKind.String ? value.GetString() : value.GetRawText();

    private static byte[] DecodeBase64Url(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        base64 += (base64.Length % 4) switch
        {
            2 => "==",
            3 => "=",
            _ => string.Empty
        };
        return Convert.FromBase64String(base64);
    }
}
