using System.Net;
using CleanArch.Application.Exceptions;
using CleanArch.Application.Middleware;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace CleanArch.Application.UnitTests.Middleware;

public class ExceptionHandlerMiddlewareTests
{
    private static async Task<(HttpContext Context, string Body)> InvokeAsync(RequestDelegate next)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new ExceptionHandlerMiddleware(next, NullLogger<ExceptionHandlerMiddleware>.Instance);

        await middleware.Invoke(context);

        context.Response.Body.Position = 0;
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync(TestContext.Current.CancellationToken);
        return (context, body);
    }

    [Fact]
    public async Task NoException_PassesThroughUntouched()
    {
        var called = false;

        var (context, body) = await InvokeAsync(_ =>
        {
            called = true;
            return Task.CompletedTask;
        });

        Assert.True(called);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.Empty(body);
    }

    [Fact]
    public async Task ValidationException_Returns400_WithErrorList()
    {
        var failures = new ValidationResult([new ValidationFailure("Name", "Name is required.")]);

        var (context, body) = await InvokeAsync(_ => throw new ValidationException(failures));

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
        Assert.Equal(["Name is required."], JsonConvert.DeserializeObject<List<string>>(body));
    }

    [Fact]
    public async Task BadRequestException_Returns400_WithMessage()
    {
        var (context, body) = await InvokeAsync(_ => throw new BadRequestException("bad input"));

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("bad input", body);
    }

    [Fact]
    public async Task NotFoundException_Returns404()
    {
        var (context, body) = await InvokeAsync(_ => throw new NotFoundException("Event", 1));

        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        Assert.Equal("Event (1) is not found", body);
    }

    [Fact]
    public async Task ApiException_Returns500_WithMessage()
    {
        var (context, body) = await InvokeAsync(_ => throw new ApiException("Invalid Credentials"));

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("Invalid Credentials", body);
    }

    [Fact]
    public async Task UnknownException_Returns500()
    {
        var (context, body) = await InvokeAsync(_ => throw new InvalidOperationException("boom"));

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("boom", body);
    }

    [Fact]
    public async Task ExceptionWithEmptyMessage_WritesJsonEnvelope()
    {
        var (_, body) = await InvokeAsync(_ => throw new BadRequestException(string.Empty));

        Assert.Contains("\"StatusCode\":400", body);
    }
}
