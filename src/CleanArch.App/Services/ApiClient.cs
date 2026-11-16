using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CleanArch.App.Models;

namespace CleanArch.App.Services;

/// <summary>Typed client for CleanArch.Api. Every failure surfaces as an <see cref="ApiRequestException"/>.</summary>
public sealed class ApiClient(HttpClient http)
{
    private const string CategoryRoute = "api/v1/category";
    private const string EventsRoute = "api/v1/events";
    private const string AccountRoute = "api/account";

    // Account

    public async Task<AuthenticationResponse> AuthenticateAsync(LoginModel model, CancellationToken cancellationToken = default)
    {
        var response = await SendAsync<BaseResponse<AuthenticationResponse>>(
            HttpMethod.Post, $"{AccountRoute}/authenticate", model, cancellationToken);
        return EnsureSucceeded(response).Data;
    }

    public async Task<string> RegisterAsync(RegisterModel model, CancellationToken cancellationToken = default)
    {
        var response = await SendAsync<BaseResponse<string>>(HttpMethod.Post, $"{AccountRoute}/register", model, cancellationToken);
        return EnsureSucceeded(response).Message;
    }

    public async Task<string> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken = default)
    {
        var uri = $"{AccountRoute}/confirm-email?userId={Uri.EscapeDataString(userId)}&code={Uri.EscapeDataString(code)}";
        var response = await SendAsync<BaseResponse<string>>(HttpMethod.Get, uri, null, cancellationToken);
        return EnsureSucceeded(response).Message;
    }

    // Categories

    public Task<List<CategoryListVm>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        SendAsync<List<CategoryListVm>>(HttpMethod.Get, $"{CategoryRoute}/all", null, cancellationToken);

    public Task<List<CategoryEventListVm>> GetCategoriesWithEventsAsync(bool includeHistory, CancellationToken cancellationToken = default) =>
        SendAsync<List<CategoryEventListVm>>(
            HttpMethod.Get, $"{CategoryRoute}/allwithevents?includeHistory={includeHistory}", null, cancellationToken);

    public async Task<CategoryListVm> CreateCategoryAsync(CategoryFormModel model, CancellationToken cancellationToken = default)
    {
        // The API reports category validation failures in a 200 response body rather than as a 400.
        var response = await SendAsync<CreateCategoryResponse>(HttpMethod.Post, CategoryRoute, model, cancellationToken);
        return EnsureSucceeded(response).Category;
    }

    // Events

    public Task<List<EventListVm>> GetEventsAsync(CancellationToken cancellationToken = default) =>
        SendAsync<List<EventListVm>>(HttpMethod.Get, EventsRoute, null, cancellationToken);

    public Task<EventDetailVm> GetEventAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<EventDetailVm>(HttpMethod.Get, $"{EventsRoute}/{id}", null, cancellationToken);

    public Task<Guid> CreateEventAsync(EventFormModel model, CancellationToken cancellationToken = default) =>
        SendAsync<Guid>(HttpMethod.Post, EventsRoute, model, cancellationToken);

    public Task UpdateEventAsync(Guid id, EventFormModel model, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Put, EventsRoute, new { EventId = id, model.Name, model.Date, model.CategoryId }, cancellationToken);

    public Task DeleteEventAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"{EventsRoute}/{id}", null, cancellationToken);

    public async Task<FileDownload> ExportEventsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await http.GetAsync($"{EventsRoute}/export", cancellationToken);
        await EnsureSuccessStatusCodeAsync(response, cancellationToken);

        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                       ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                       ?? $"Events_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "text/csv";
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        return new FileDownload(fileName, contentType, content);
    }

    // Plumbing

    private async Task<T> SendAsync<T>(HttpMethod method, string uri, object body, CancellationToken cancellationToken)
    {
        using var response = await SendRawAsync(method, uri, body, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<T>(cancellationToken))!;
    }

    private async Task SendAsync(HttpMethod method, string uri, object body, CancellationToken cancellationToken)
    {
        using var response = await SendRawAsync(method, uri, body, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendRawAsync(HttpMethod method, string uri, object body, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, uri);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, body.GetType());
        }

        HttpResponseMessage response;
        try
        {
            response = await http.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            // In the browser, CORS rejections and an API that isn't running both surface here.
            throw new ApiRequestException(0, $"Could not reach the API at {http.BaseAddress}. Is it running? ({ex.Message})");
        }

        try
        {
            await EnsureSuccessStatusCodeAsync(response, cancellationToken);
        }
        catch
        {
            response.Dispose();
            throw;
        }

        return response;
    }

    private static TResponse EnsureSucceeded<TResponse>(TResponse response) where TResponse : IApiResponse =>
        response.Success
            ? response
            : throw new ApiRequestException(
                HttpStatusCode.OK,
                response.Message ?? "The request was rejected.",
                response.ValidationErrors);

    /// <summary>Turns a non-success response into an exception, reading RFC 9457 problem details when present.</summary>
    internal static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new ApiRequestException(response.StatusCode, "Your session has expired. Please sign in again.");
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new ApiRequestException(response.StatusCode, "You don't have permission to do that.");
        }

        string message = null;
        var errors = new List<string>();

        try
        {
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                message = GetString(root, "detail") ?? GetString(root, "title");

                if (root.TryGetProperty("errors", out var errorsElement))
                {
                    switch (errorsElement.ValueKind)
                    {
                        // GlobalExceptionHandler: "errors": ["Name is required."]
                        case JsonValueKind.Array:
                            errors.AddRange(errorsElement.EnumerateArray().Select(e => e.GetString()));
                            break;
                        // ASP.NET Core model validation: "errors": { "Name": ["The Name field is required."] }
                        case JsonValueKind.Object:
                            errors.AddRange(errorsElement.EnumerateObject()
                                .SelectMany(p => p.Value.EnumerateArray().Select(e => e.GetString())));
                            break;
                    }
                }
            }
        }
        catch (JsonException)
        {
            // Not JSON; fall back to the status code below.
        }

        throw new ApiRequestException(
            response.StatusCode,
            message ?? $"The request failed ({(int)response.StatusCode} {response.ReasonPhrase}).",
            errors);
    }

    private static string GetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
}
