using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArch.App.Services;

/// <summary>Derives the signed-in user from the stored JWT and raises change notifications on sign-in/out.</summary>
public sealed class JwtAuthenticationStateProvider(ITokenStore tokenStore, TimeProvider timeProvider)
    : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await tokenStore.GetTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            return Anonymous;
        }

        var identity = JwtParser.TryCreateIdentity(token, timeProvider.GetUtcNow());
        if (identity is null)
        {
            // Expired or unreadable: drop it so it isn't sent to the API any more.
            await tokenStore.RemoveTokenAsync();
            return Anonymous;
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task SignInAsync(string token)
    {
        await tokenStore.SetTokenAsync(token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task SignOutAsync()
    {
        await tokenStore.RemoveTokenAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }
}
