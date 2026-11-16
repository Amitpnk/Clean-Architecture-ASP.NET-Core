using CleanArch.App;
using CleanArch.App.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// The API's address comes from wwwroot/appsettings{.Environment}.json.
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl is not configured in wwwroot/appsettings.json.");

// Authentication: JWT from the API, kept in localStorage.
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ITokenStore, LocalStorageTokenStore>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());

// HTTP: one client for the API, with the bearer token attached by AuthorizedHandler.
builder.Services.AddScoped(sp => new HttpClient(new AuthorizedHandler(
        sp.GetRequiredService<ITokenStore>(),
        sp.GetRequiredService<JwtAuthenticationStateProvider>())
    {
        InnerHandler = new HttpClientHandler()
    })
{
    BaseAddress = new Uri(apiBaseUrl)
});
builder.Services.AddScoped<ApiClient>();

await builder.Build().RunAsync();
