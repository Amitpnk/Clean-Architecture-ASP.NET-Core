using System.Text.Json;
using CleanArch.Api.ExceptionHandling;
using CleanArch.Application.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace CleanArch.Api.UnitTests.ExceptionHandling;

public class GlobalExceptionHandlerTests
{
    private static async Task<(HttpContext Context, JsonElement Body, bool Handled)> HandleAsync(Exception exception)
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddProblemDetails()
            .BuildServiceProvider();

        var context = new DefaultHttpContext { RequestServices = services };
        context.Request.Path = "/api/v1/events";
        context.Response.Body = new MemoryStream();

        var handler = new GlobalExceptionHandler(
            services.GetRequiredService<IProblemDetailsService>(),
            NullLogger<GlobalExceptionHandler>.Instance);

        var handled = await handler.TryHandleAsync(context, exception, TestContext.Current.CancellationToken);

        context.Response.Body.Position = 0;
        var body = await JsonSerializer.DeserializeAsync<JsonElement>(
            context.Response.Body, cancellationToken: TestContext.Current.CancellationToken);
        return (context, body, handled);
    }

    [Fact]
    public async Task ValidationException_Returns400_WithErrorList()
    {
        var failures = new ValidationResult([new ValidationFailure("Name", "Name is required.")]);

        var (context, body, handled) = await HandleAsync(new ValidationException(failures));

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal(400, body.GetProperty("status").GetInt32());
        Assert.Equal("Name is required.", body.GetProperty("errors")[0].GetString());
    }

    [Fact]
    public async Task BadRequestException_Returns400_WithMessageAsDetail()
    {
        var (context, body, _) = await HandleAsync(new BadRequestException("bad input"));

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal("bad input", body.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task NotFoundException_Returns404_WithInstance()
    {
        var (context, body, _) = await HandleAsync(new NotFoundException("Event", 1));

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        Assert.Equal("Event (1) is not found", body.GetProperty("detail").GetString());
        Assert.Equal("/api/v1/events", body.GetProperty("instance").GetString());
    }

    [Fact]
    public async Task ApiException_Returns500_WithMessageAsDetail()
    {
        var (context, body, _) = await HandleAsync(new ApiException("Invalid Credentials"));

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("Invalid Credentials", body.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task UnknownException_Returns500_WithoutLeakingMessage()
    {
        var (context, body, _) = await HandleAsync(new InvalidOperationException("connection string secret"));

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.False(body.TryGetProperty("detail", out _));
        Assert.DoesNotContain("secret", body.GetRawText());
    }
}
