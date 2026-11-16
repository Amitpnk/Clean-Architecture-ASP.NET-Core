using System.Net;
using System.Net.Http.Headers;

namespace CleanArch.App.Services;

/// <summary>
/// Attaches the stored JWT as a bearer token, and signs the user out when the API rejects it
/// (for example after it expires), which sends protected pages back to the login screen.
/// </summary>
public sealed class AuthorizedHandler(ITokenStore tokenStore, JwtAuthenticationStateProvider authenticationStateProvider)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenStore.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(token))
        {
            await authenticationStateProvider.SignOutAsync();
        }

        return response;
    }
}
