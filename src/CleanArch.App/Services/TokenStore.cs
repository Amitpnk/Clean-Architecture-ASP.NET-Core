using Microsoft.JSInterop;

namespace CleanArch.App.Services;

public interface ITokenStore
{
    ValueTask<string> GetTokenAsync();
    ValueTask SetTokenAsync(string token);
    ValueTask RemoveTokenAsync();
}

/// <summary>
/// Keeps the JWT in the browser's localStorage so a sign-in survives a page reload.
/// localStorage is readable by any script on the page, so the API's token lifetime (60 minutes by default)
/// is what limits exposure; switch to sessionStorage or a cookie-based BFF if that matters for your app.
/// </summary>
public sealed class LocalStorageTokenStore(IJSRuntime js) : ITokenStore
{
    private const string Key = "cleanarch.jwt";

    public ValueTask<string> GetTokenAsync() => js.InvokeAsync<string>("localStorage.getItem", Key);

    public ValueTask SetTokenAsync(string token) => js.InvokeVoidAsync("localStorage.setItem", Key, token);

    public ValueTask RemoveTokenAsync() => js.InvokeVoidAsync("localStorage.removeItem", Key);
}
