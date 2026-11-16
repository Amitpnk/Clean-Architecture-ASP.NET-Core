using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CleanArch.Application.Contracts.Infrastructure;
using CleanArch.Application.Exceptions;
using CleanArch.CrossCuttingConcerns.Identity;
using CleanArch.CrossCuttingConcerns.OS;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Enum;
using CleanArch.Domain.Services.Authentication;
using CleanArch.Domain.Services.Mail;
using CleanArch.Persistence.Context;
using CleanArch.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace CleanArch.Persistence.UnitTests.Repositories;

/// <summary>
/// Exercises <see cref="AuthenticationService"/> with the real ASP.NET Core Identity stack
/// (UserManager, SignInManager, token providers) over an EF Core in-memory store with the seed data.
/// </summary>
public class AuthenticationServiceTests
{
    private const string Origin = "https://cleanarch.test";
    private const string SeededEmail = "superadmin@gmail.com";
    private const string SeededPassword = "Password@123";

    private static readonly JwtSettings Jwt = new()
    {
        Key = "unit-test-signing-key-that-is-long-enough-1234567890",
        Issuer = "unit-test-issuer",
        Audience = "unit-test-audience",
        DurationInMinutes = 30
    };

    private ServiceProvider _provider;
    private IServiceScope _scope;
    private Mock<IEmailService> _emailService;
    private Mock<IFeatureManager> _featureManager;
    private List<MailRequest> _sentEmails;
    private AuthenticationService _service;

    [SetUp]
    public void SetUp()
    {
        _sentEmails = [];
        _emailService = new Mock<IEmailService>();
        _emailService.Setup(s => s.SendEmailAsync(It.IsAny<MailRequest>()))
            .Callback<MailRequest>(_sentEmails.Add)
            .Returns(Task.CompletedTask);
        _featureManager = new Mock<IFeatureManager>();
        _featureManager.Setup(f => f.IsEnabledAsync(It.IsAny<string>())).ReturnsAsync(false);

        var databaseName = $"auth-{Guid.NewGuid()}";
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddSingleton<ILoggedInUserService, NoUser>();
        services.AddSingleton<IDateTimeProvider, SystemClock>();
        services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase(databaseName));
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        services.Configure<JwtSettings>(o =>
        {
            o.Key = Jwt.Key;
            o.Issuer = Jwt.Issuer;
            o.Audience = Jwt.Audience;
            o.DurationInMinutes = Jwt.DurationInMinutes;
        });
        services.AddSingleton(_emailService.Object);
        services.AddSingleton(_featureManager.Object);
        services.AddScoped<AuthenticationService>();

        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();
        _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();

        // SignInManager needs an HttpContext to issue its (cookie) sign-in.
        _scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext =
            new DefaultHttpContext { RequestServices = _scope.ServiceProvider };

        _service = _scope.ServiceProvider.GetRequiredService<AuthenticationService>();
    }

    [TearDown]
    public void TearDown()
    {
        _scope.Dispose();
        _provider.Dispose();
    }

    private static RegistrationRequest NewRegistration(string password = "Password@123")
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        return new RegistrationRequest
        {
            FirstName = "Unit",
            LastName = "Test",
            UserName = $"user{suffix}",
            Email = $"user{suffix}@example.com",
            Password = password
        };
    }

    private static (string UserId, string Code) ParseVerificationUri(string message)
    {
        var uri = new Uri(message[message.IndexOf(Origin, StringComparison.Ordinal)..]);
        var query = QueryHelpers.ParseQuery(uri.Query);
        return (query["userId"], query["code"]);
    }

    [Test]
    public async Task AuthenticateAsync_SeededUser_ReturnsSignedJwtWithRoles()
    {
        var response = await _service.AuthenticateAsync(new AuthenticationRequest { Email = SeededEmail, Password = SeededPassword });

        var principal = new JwtSecurityTokenHandler().ValidateToken(response.Data.JWToken, new TokenValidationParameters
        {
            ValidIssuer = Jwt.Issuer,
            ValidAudience = Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.Key)),
        }, out _);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Success, Is.True);
            Assert.That(response.Data.Id, Is.EqualTo(Constants.SuperAdminUser));
            Assert.That(response.Data.Email, Is.EqualTo(SeededEmail));
            Assert.That(response.Data.UserName, Is.EqualTo("superadmin"));
            Assert.That(response.Data.IsVerified, Is.True);
            Assert.That(response.Data.Roles, Is.EquivalentTo(new[] { "SuperAdmin", "Admin", "Moderator", "Basic" }));
            Assert.That(response.Data.RefreshToken, Has.Length.EqualTo(80));
            Assert.That(principal.FindFirst("uid")?.Value, Is.EqualTo(Constants.SuperAdminUser));
            // Raw token: the validating handler remaps "roles" to ClaimTypes.Role on the way in.
            Assert.That(new JwtSecurityTokenHandler().ReadJwtToken(response.Data.JWToken).Claims
                .Where(c => c.Type == "roles").Select(c => c.Value), Does.Contain("SuperAdmin"));
        }
    }

    [Test]
    public void AuthenticateAsync_UnknownEmail_Throws()
    {
        var ex = Assert.ThrowsAsync<ApiException>(() =>
            _service.AuthenticateAsync(new AuthenticationRequest { Email = "nobody@example.com", Password = SeededPassword }));

        Assert.That(ex.Message, Does.StartWith("No Accounts Registered"));
    }

    [Test]
    public void AuthenticateAsync_WrongPassword_Throws()
    {
        var ex = Assert.ThrowsAsync<ApiException>(() =>
            _service.AuthenticateAsync(new AuthenticationRequest { Email = SeededEmail, Password = "Wrong@123" }));

        Assert.That(ex.Message, Does.StartWith("Invalid Credentials"));
    }

    [Test]
    public async Task AuthenticateAsync_UnconfirmedUser_Throws()
    {
        var registration = NewRegistration();
        await _service.RegisterAsync(registration, Origin);

        var ex = Assert.ThrowsAsync<ApiException>(() =>
            _service.AuthenticateAsync(new AuthenticationRequest { Email = registration.Email, Password = registration.Password }));

        Assert.That(ex.Message, Does.StartWith("Account Not Confirmed"));
    }

    [Test]
    public async Task RegisterAsync_CreatesBasicUser_AndReturnsVerificationLink()
    {
        var registration = NewRegistration();

        var response = await _service.RegisterAsync(registration, Origin);

        var userManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync(registration.Email);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Success, Is.True);
            Assert.That(response.Data, Is.EqualTo(user!.Id));
            Assert.That(response.Message, Does.Contain($"{Origin}/api/account/confirm-email/?userId={user.Id}"));
            Assert.That(await userManager.GetRolesAsync(user), Is.EqualTo(new[] { Roles.Basic.ToString() }));
            Assert.That(user.EmailConfirmed, Is.False);
        }
        _emailService.Verify(s => s.SendEmailAsync(It.IsAny<MailRequest>()), Times.Never);
    }

    [Test]
    public async Task RegisterAsync_WithEmailFeatureEnabled_SendsConfirmationEmail()
    {
        _featureManager.Setup(f => f.IsEnabledAsync(nameof(FeatureManagement.EnableEmailService))).ReturnsAsync(true);

        await _service.RegisterAsync(NewRegistration(), Origin);

        Assert.That(_sentEmails.Single().Subject, Is.EqualTo("Confirm Registration"));
    }

    [Test]
    public void RegisterAsync_DuplicateUserName_Throws()
    {
        var registration = NewRegistration();
        registration.UserName = "superadmin";

        var ex = Assert.ThrowsAsync<ApiException>(() => _service.RegisterAsync(registration, Origin));

        Assert.That(ex.Message, Is.EqualTo("Username 'superadmin' is already taken."));
    }

    [Test]
    public void RegisterAsync_DuplicateEmail_Throws()
    {
        var registration = NewRegistration();
        registration.Email = SeededEmail;

        var ex = Assert.ThrowsAsync<ApiException>(() => _service.RegisterAsync(registration, Origin));

        Assert.That(ex.Message, Is.EqualTo($"Email {SeededEmail} is already registered."));
    }

    [Test]
    public void RegisterAsync_WeakPassword_ThrowsIdentityError()
    {
        Assert.ThrowsAsync<ApiException>(() => _service.RegisterAsync(NewRegistration(password: "weak"), Origin));
    }

    [Test]
    public async Task ConfirmEmailAsync_WithIssuedCode_ConfirmsAccount_SoUserCanSignIn()
    {
        var registration = NewRegistration();
        var registered = await _service.RegisterAsync(registration, Origin);
        var (userId, code) = ParseVerificationUri(registered.Message);

        var confirmed = await _service.ConfirmEmailAsync(userId, code);
        var signIn = await _service.AuthenticateAsync(new AuthenticationRequest { Email = registration.Email, Password = registration.Password });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(confirmed.Data, Is.EqualTo(userId));
            Assert.That(signIn.Data.IsVerified, Is.True);
            Assert.That(signIn.Data.Roles, Is.EqualTo(new[] { "Basic" }));
        }
    }

    [Test]
    public async Task ConfirmEmailAsync_InvalidCode_Throws()
    {
        var registered = await _service.RegisterAsync(NewRegistration(), Origin);
        var (userId, _) = ParseVerificationUri(registered.Message);
        var bogus = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("not-a-token"));

        Assert.ThrowsAsync<ApiException>(() => _service.ConfirmEmailAsync(userId, bogus));
    }

    [Test]
    public async Task ForgotPassword_UnknownEmail_SendsNothing()
    {
        await _service.ForgotPassword(new ForgotPasswordRequest { Email = "nobody@example.com" }, Origin);

        Assert.That(_sentEmails, Is.Empty);
    }

    [Test]
    public async Task ForgotPassword_ThenResetPassword_ChangesThePassword()
    {
        await _service.ForgotPassword(new ForgotPasswordRequest { Email = SeededEmail }, Origin);
        var email = _sentEmails.Single();
        var token = email.Body["You reset token is - ".Length..];

        var reset = await _service.ResetPassword(new ResetPasswordRequest
        {
            Email = SeededEmail,
            Token = token,
            Password = "NewPassword@456",
            ConfirmPassword = "NewPassword@456"
        });

        var signIn = await _service.AuthenticateAsync(new AuthenticationRequest { Email = SeededEmail, Password = "NewPassword@456" });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(email.ToEmail, Is.EqualTo(SeededEmail));
            Assert.That(email.Subject, Is.EqualTo("Reset Password"));
            Assert.That(reset.Success, Is.True);
            Assert.That(signIn.Success, Is.True);
        }
    }

    [Test]
    public void ResetPassword_UnknownEmail_Throws()
    {
        Assert.ThrowsAsync<ApiException>(() => _service.ResetPassword(new ResetPasswordRequest
        {
            Email = "nobody@example.com",
            Token = "x",
            Password = "NewPassword@456"
        }));
    }

    [Test]
    public void ResetPassword_InvalidToken_Throws()
    {
        var ex = Assert.ThrowsAsync<ApiException>(() => _service.ResetPassword(new ResetPasswordRequest
        {
            Email = SeededEmail,
            Token = "invalid",
            Password = "NewPassword@456"
        }));

        Assert.That(ex.Message, Does.StartWith("Error occured while reseting"));
    }

    private sealed class NoUser : ILoggedInUserService
    {
        public bool IsAuthenticated => false;
        public string UserId => null;
    }

    private sealed class SystemClock : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTimeOffset OffsetNow => DateTimeOffset.Now;
        public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
    }
}
