using System.Security.Claims;
using System.Text;
using CleanArch.Application.Contracts.Infrastructure;
using CleanArch.Application.Features.Events.Queries.GetEventsExport;
using CleanArch.Domain.Services.Mail;
using CleanArch.Infrastructure;
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
    [Test]
    public void ExportEventsToCsv_WritesHeaderAndOneRowPerEvent()
    {
        var id = Guid.NewGuid();
        var dtos = new List<EventExportDto>
        {
            new() { EventId = id, Name = "Rock night", Date = new DateTime(2027, 6, 1, 18, 30, 0) }
        };

        var csv = Encoding.UTF8.GetString(new CsvExporter().ExportEventsToCsv(dtos)).TrimStart('﻿');
        var lines = csv.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("EventId,Name,Date"));
            Assert.That(lines[1], Does.StartWith($"{id},Rock night,"));
            Assert.That(lines[1], Does.Contain("2027"));
        }
    }

    [Test]
    public void ExportEventsToCsv_QuotesFieldsContainingSeparators()
    {
        var dtos = new List<EventExportDto> { new() { EventId = Guid.NewGuid(), Name = "Rock, Pop", Date = DateTime.Today } };

        var csv = Encoding.UTF8.GetString(new CsvExporter().ExportEventsToCsv(dtos));

        Assert.That(csv, Does.Contain("\"Rock, Pop\""));
    }

    [Test]
    public void ExportEventsToCsv_EmptyList_WritesHeaderOnly()
    {
        var csv = Encoding.UTF8.GetString(new CsvExporter().ExportEventsToCsv([])).TrimStart('﻿');

        Assert.That(csv.Trim(), Is.EqualTo("EventId,Name,Date").Or.Empty);
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

    [Test]
    public void NoHttpContext_IsAnonymous()
    {
        var service = Create(null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(service.IsAuthenticated, Is.False);
            Assert.That(service.UserId, Is.Null);
            Assert.That(service.Claims, Is.Null);
            Assert.That(service.GetUserId(), Is.Null);
        }
    }

    [Test]
    public void AuthenticatedUser_ExposesNameIdentifierAndClaims()
    {
        var service = Create(ContextWith(new Claim(ClaimTypes.NameIdentifier, "user-1"), new Claim("email", "a@b.c")));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(service.IsAuthenticated, Is.True);
            Assert.That(service.UserId, Is.EqualTo("user-1"));
            Assert.That(service.GetUserId(), Is.EqualTo("user-1"));
            Assert.That(service.Claims, Does.Contain(new KeyValuePair<string, string>("email", "a@b.c")));
        }
    }

    [Test]
    public void SubClaim_IsUsedWhenNameIdentifierMissing()
    {
        var service = Create(ContextWith(new Claim("sub", "superadmin")));

        Assert.That(service.UserId, Is.EqualTo("superadmin"));
    }

    [Test]
    public void UnauthenticatedPrincipal_IsNotAuthenticated()
    {
        var service = Create(ContextWith());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(service.IsAuthenticated, Is.False);
            Assert.That(service.UserId, Is.Null);
        }
    }
}

public class DateTimeProviderTests
{
    [Test]
    public void ReturnsCurrentTimes_InTheRightKind()
    {
        var provider = new DateTimeProvider();
        var tolerance = TimeSpan.FromSeconds(5);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(provider.Now, Is.EqualTo(DateTime.Now).Within(tolerance));
            Assert.That(provider.UtcNow, Is.EqualTo(DateTime.UtcNow).Within(tolerance));
            Assert.That(provider.UtcNow.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(provider.OffsetNow, Is.EqualTo(DateTimeOffset.Now).Within(tolerance));
            Assert.That(provider.OffsetUtcNow.Offset, Is.EqualTo(TimeSpan.Zero));
        }
    }
}

public class InfrastructureServiceRegistrationTests
{
    [Test]
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(provider.GetService<IEmailService>(), Is.InstanceOf<EmailService>());
            Assert.That(provider.GetService<ICsvExporter>(), Is.InstanceOf<CsvExporter>());
            Assert.That(mail.Mail, Is.EqualTo("noreply@example.com"));
            Assert.That(mail.Host, Is.EqualTo("smtp.example.com"));
            Assert.That(mail.Port, Is.EqualTo(2525));
        }
    }
}
