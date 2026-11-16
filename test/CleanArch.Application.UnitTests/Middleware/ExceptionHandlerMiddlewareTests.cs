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
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        return (context, body);
    }

    [Test]
    public async Task NoException_PassesThroughUntouched()
    {
        var called = false;

        var (context, body) = await InvokeAsync(_ =>
        {
            called = true;
            return Task.CompletedTask;
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(called, Is.True);
            Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(body, Is.Empty);
        }
    }

    [Test]
    public async Task ValidationException_Returns400_WithErrorList()
    {
        var failures = new ValidationResult([new ValidationFailure("Name", "Name is required.")]);

        var (context, body) = await InvokeAsync(_ => throw new ValidationException(failures));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(context.Response.ContentType, Is.EqualTo("application/json"));
            Assert.That(JsonConvert.DeserializeObject<List<string>>(body), Is.EqualTo(new[] { "Name is required." }));
        }
    }

    [Test]
    public async Task BadRequestException_Returns400_WithMessage()
    {
        var (context, body) = await InvokeAsync(_ => throw new BadRequestException("bad input"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(body, Is.EqualTo("bad input"));
        }
    }

    [Test]
    public async Task NotFoundException_Returns404()
    {
        var (context, body) = await InvokeAsync(_ => throw new NotFoundException("Event", 1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(body, Is.EqualTo("Event (1) is not found"));
        }
    }

    [Test]
    public async Task ApiException_Returns500_WithMessage()
    {
        var (context, body) = await InvokeAsync(_ => throw new ApiException("Invalid Credentials"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
            Assert.That(body, Is.EqualTo("Invalid Credentials"));
        }
    }

    [Test]
    public async Task UnknownException_Returns500()
    {
        var (context, body) = await InvokeAsync(_ => throw new InvalidOperationException("boom"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
            Assert.That(body, Is.EqualTo("boom"));
        }
    }

    [Test]
    public async Task ExceptionWithEmptyMessage_WritesJsonEnvelope()
    {
        var (context, body) = await InvokeAsync(_ => throw new BadRequestException(string.Empty));

        Assert.That(body, Does.Contain("\"StatusCode\":400"));
    }
}
