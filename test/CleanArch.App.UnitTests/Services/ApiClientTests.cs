using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CleanArch.App.Models;
using CleanArch.App.Services;

namespace CleanArch.App.UnitTests.Services;

public class ApiClientTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private static ApiClient Create(HttpMessageHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri("https://api.test/") });

    [Fact]
    public async Task Authenticate_PostsCredentials_AndReturnsToken()
    {
        var handler = StubHttpHandler.Json(HttpStatusCode.OK,
            """{"success":true,"message":"Authenticated superadmin","data":{"userName":"superadmin","jwToken":"abc.def.ghi"}}""");

        var result = await Create(handler).AuthenticateAsync(new LoginModel { Email = "a@b.c", Password = "pw" }, Ct);

        Assert.Equal("abc.def.ghi", result.JWToken);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Equal("https://api.test/api/account/authenticate", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task ProblemDetails_WithErrorArray_BecomesApiRequestException()
    {
        var handler = StubHttpHandler.Json(HttpStatusCode.BadRequest,
            """{"title":"Validation failed","status":400,"detail":"One or more validation errors occurred.","errors":["Name is required."]}""",
            "application/problem+json");

        var ex = await Assert.ThrowsAsync<ApiRequestException>(() => Create(handler).CreateEventAsync(new EventFormModel(), Ct));

        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("One or more validation errors occurred.", ex.Message);
        Assert.Equal(["Name is required."], ex.Errors);
    }

    [Fact]
    public async Task ModelValidationProblem_WithErrorDictionary_IsFlattened()
    {
        var handler = StubHttpHandler.Json(HttpStatusCode.BadRequest,
            """{"title":"One or more validation errors occurred.","status":400,"errors":{"Name":["The Name field is required."],"Date":["Bad date."]}}""",
            "application/problem+json");

        var ex = await Assert.ThrowsAsync<ApiRequestException>(() => Create(handler).CreateEventAsync(new EventFormModel(), Ct));

        Assert.Equal("One or more validation errors occurred.", ex.Message);
        Assert.Equal(["The Name field is required.", "Bad date."], ex.Errors);
    }

    [Fact]
    public async Task ApiException_DetailIsUsedAsMessage()
    {
        var handler = StubHttpHandler.Json(HttpStatusCode.InternalServerError,
            """{"title":"Request failed","status":500,"detail":"Invalid Credentials for 'a@b.c'."}""", "application/problem+json");

        var ex = await Assert.ThrowsAsync<ApiRequestException>(() => Create(handler).AuthenticateAsync(new LoginModel(), Ct));

        Assert.Equal("Invalid Credentials for 'a@b.c'.", ex.Message);
    }

    [Fact]
    public async Task Unauthorized_HasSessionExpiredMessage()
    {
        var handler = new StubHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));

        var ex = await Assert.ThrowsAsync<ApiRequestException>(() => Create(handler).GetEventsAsync(Ct));

        Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
        Assert.Contains("sign in again", ex.Message);
    }

    [Fact]
    public async Task NonJsonError_FallsBackToStatusCode()
    {
        var handler = new StubHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
            Content = new StringContent("<html>bad gateway</html>")
        });

        var ex = await Assert.ThrowsAsync<ApiRequestException>(() => Create(handler).GetEventsAsync(Ct));

        Assert.Contains("502", ex.Message);
    }

    [Fact]
    public async Task UnreachableApi_HasHelpfulMessage()
    {
        var handler = new StubHttpHandler(_ => throw new HttpRequestException("TypeError: Failed to fetch"));

        var ex = await Assert.ThrowsAsync<ApiRequestException>(() => Create(handler).GetEventsAsync(Ct));

        Assert.Contains("Could not reach the API at https://api.test/", ex.Message);
    }

    [Fact]
    public async Task CreateCategory_UnsuccessfulBody_ThrowsWithValidationErrors()
    {
        // The API reports category validation failures with 200 OK and success=false.
        var handler = StubHttpHandler.Json(HttpStatusCode.OK,
            """{"success":false,"validationErrors":["An category with the same name already exists."]}""");

        var ex = await Assert.ThrowsAsync<ApiRequestException>(() => Create(handler).CreateCategoryAsync(new CategoryFormModel { Name = "Concerts" }, Ct));

        Assert.Equal(["An category with the same name already exists."], ex.Errors);
    }

    [Fact]
    public async Task CreateCategory_Success_ReturnsCategory()
    {
        var id = Guid.NewGuid();
        var handler = StubHttpHandler.Json(HttpStatusCode.OK,
            $$$"""{"success":true,"category":{"categoryId":"{{{id}}}","name":"Concerts"}}""");

        var category = await Create(handler).CreateCategoryAsync(new CategoryFormModel { Name = "Concerts" }, Ct);

        Assert.Equal(id, category.CategoryId);
    }

    [Fact]
    public async Task UpdateEvent_PutsCommandWithEventId()
    {
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        string body = null;
        var handler = new StubHttpHandler(request =>
        {
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });

        await Create(handler).UpdateEventAsync(id, new EventFormModel { Name = "Gig", CategoryId = categoryId }, Ct);

        using var json = JsonDocument.Parse(body);
        Assert.Equal(HttpMethod.Put, handler.LastRequest.Method);
        Assert.Equal(id, json.RootElement.GetProperty("eventId").GetGuid());
        Assert.Equal(categoryId, json.RootElement.GetProperty("categoryId").GetGuid());
        Assert.False(json.RootElement.TryGetProperty("description", out _));
    }

    [Fact]
    public async Task CreateEvent_SendsDateWithoutUtcOffset()
    {
        string body = null;
        var handler = new StubHttpHandler(request =>
        {
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(Guid.NewGuid()) };
        });
        var model = new EventFormModel { Date = new DateTime(2027, 3, 15, 18, 0, 0, DateTimeKind.Local) };

        await Create(handler).CreateEventAsync(model, Ct);

        // The API stores zone-less dates; an offset here would shift the stored time by the browser's UTC offset.
        using var json = JsonDocument.Parse(body);
        Assert.Equal("2027-03-15T18:00:00", json.RootElement.GetProperty("date").GetString());
        Assert.Equal(DateTimeKind.Unspecified, new EventFormModel().Date.Kind);
    }

    [Fact]
    public async Task ExportEvents_ReadsFileNameAndContent()
    {
        var handler = new StubHttpHandler(_ =>
        {
            var content = new ByteArrayContent("Name,Date\n"u8.ToArray());
            content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Events_1.csv" };
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        });

        var file = await Create(handler).ExportEventsAsync(Ct);

        Assert.Equal("Events_1.csv", file.FileName);
        Assert.Equal("text/csv", file.ContentType);
        Assert.Equal("Name,Date\n"u8.ToArray(), file.Content);
    }
}
