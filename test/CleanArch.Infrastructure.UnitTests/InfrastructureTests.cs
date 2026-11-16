using System.Security.Claims;
using System.Text;
using CleanArch.Application.Contracts.Infrastructure;
using CleanArch.Application.Features.Events.Queries.GetEventsExport;
using CleanArch.Domain.Services.Mail;
using CleanArch.Infrastructure.FileExport;
using CleanArch.Infrastructure.Identity;
using CleanArch.Infrastructure.Mail;
using CleanArch.Infrastructure.OS;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace CleanArch.Infrastructure.UnitTests;

public class CsvExporterTests
{
    private static string[] ExportLines(List<EventExportDto> dtos) =>
        Encoding.UTF8.GetString(new CsvExporter().ExportEventsToCsv(dtos))
            .TrimStart('﻿')
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

    [Fact]
    public void ExportEventsToCsv_WritesHeaderAndOneRowPerEvent()
    {
        var id = Guid.NewGuid();

        var lines = ExportLines([new() { EventId = id, Name = "Rock night", Date = new DateTime(2027, 6, 1, 18, 30, 0) }]);

        Assert.Equal(2, lines.Length);
        Assert.Equal("EventId,Name,Date", lines[0]);
        Assert.StartsWith($"{id},Rock night,", lines[1]);
        Assert.Contains("2027", lines[1]);
    }

    [Fact]
    public void ExportEventsToCsv_QuotesFieldsContainingSeparators()
    {
        var lines = ExportLines([new() { EventId = Guid.NewGuid(), Name = "Rock, Pop", Date = DateTime.Today }]);

        Assert.Contains("\"Rock, Pop\"", lines[1]);
    }

    [Fact]
    public void ExportEventsToCsv_EmptyList_WritesAtMostTheHeader()
    {
        var lines = ExportLines([]);

        Assert.True(lines.Length == 0 || lines is ["EventId,Name,Date"]);
    }
}

public class LoggedInUserServiceTests
{
    private static LoggedInUserService Create(HttpContext context)
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.SetupGet(a => a.HttpContext).Returns(context);
        return new LoggedInUserService(accessor.Object);
    }

    private static HttpContext ContextWith(params Claim[] claims) => new DefaultHttpContext
    {
        User = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: claims.Length > 0 ? "Bearer" : null))
    };

    [Fact]
    public void NoHttpContext_IsAnonymous()
    {
        var service = Create(null);

        Assert.False(service.IsAuthenticated);
        Assert.Null(service.UserId);
        Assert.Null(service.Claims);
        Assert.Null(service.GetUserId());
    }

    [Fact]
    public void AuthenticatedUser_ExposesNameIdentifierAndClaims()
    {
        var service = Create(ContextWith(new Claim(ClaimTypes.NameIdentifier, "user-1"), new Claim("email", "a@b.c")));

        Assert.True(service.IsAuthenticated);
        Assert.Equal("user-1", service.UserId);
        Assert.Equal("user-1", service.GetUserId());
        Assert.Contains(new KeyValuePair<string, string>("email", "a@b.c"), service.Claims);
    }

    [Fact]
    public void SubClaim_IsUsedWhenNameIdentifierMissing()
    {
        var service = Create(ContextWith(new Claim("sub", "superadmin")));

        Assert.Equal("superadmin", service.UserId);
    }

    [Fact]
    public void UnauthenticatedPrincipal_IsNotAuthenticated()
    {
        var service = Create(ContextWith());

        Assert.False(service.IsAuthenticated);
        Assert.Null(service.UserId);
    }
}

public class DateTimeProviderTests
{
    [Fact]
    public void ReturnsCurrentTimes_InTheRightKind()
    {
        var provider = new DateTimeProvider();
        var tolerance = TimeSpan.FromSeconds(5);

        Assert.Equal(DateTime.Now, provider.Now, tolerance);
        Assert.Equal(DateTime.UtcNow, provider.UtcNow, tolerance);
        Assert.Equal(DateTimeKind.Utc, provider.UtcNow.Kind);
        Assert.Equal(DateTimeOffset.Now, provider.OffsetNow, tolerance);
        Assert.Equal(TimeSpan.Zero, provider.OffsetUtcNow.Offset);
    }
}

public class InfrastructureServiceRegistrationTests
{
    [Fact]
    public void AddInfrastructureServices_RegistersServices_AndBindsMailSettings()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["MailSettings:Mail"] = "noreply@example.com",
                ["MailSettings:Host"] = "smtp.example.com",
                ["MailSettings:Port"] = "2525"
            })
            .Build();

        using var provider = new ServiceCollection()
            .AddLogging()
            .AddInfrastructureServices(configuration)
            .BuildServiceProvider();

        var mail = provider.GetRequiredService<IOptions<MailSettings>>().Value;
        Assert.IsType<EmailService>(provider.GetService<IEmailService>());
        Assert.IsType<CsvExporter>(provider.GetService<ICsvExporter>());
        Assert.Equal("noreply@example.com", mail.Mail);
        Assert.Equal("smtp.example.com", mail.Host);
        Assert.Equal(2525, mail.Port);
    }
}
