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
public sealed class AuthenticationServiceTests : IDisposable
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

    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IFeatureManager> _featureManager = new();
    private readonly List<MailRequest> _sentEmails = [];
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _emailService.Setup(s => s.SendEmailAsync(It.IsAny<MailRequest>()))
            .Callback<MailRequest>(_sentEmails.Add)
            .Returns(Task.CompletedTask);
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

    public void Dispose()
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

    [Fact]
    public async Task AuthenticateAsync_SeededUser_ReturnsSignedJwtWithRoles()
    {
        var response = await _service.AuthenticateAsync(new AuthenticationRequest { Email = SeededEmail, Password = SeededPassword });

        var principal = new JwtSecurityTokenHandler().ValidateToken(response.Data.JWToken, new TokenValidationParameters
        {
            ValidIssuer = Jwt.Issuer,
            ValidAudience = Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.Key)),
        }, out _);
        // Raw token: the validating handler remaps "roles" to ClaimTypes.Role on the way in.
        var roleClaims = new JwtSecurityTokenHandler().ReadJwtToken(response.Data.JWToken).Claims
            .Where(c => c.Type == "roles").Select(c => c.Value);

        Assert.True(response.Success);
        Assert.Equal(Constants.SuperAdminUser, response.Data.Id);
        Assert.Equal(SeededEmail, response.Data.Email);
        Assert.Equal("superadmin", response.Data.UserName);
        Assert.True(response.Data.IsVerified);
        Assert.Equivalent(new[] { "SuperAdmin", "Admin", "Moderator", "Basic" }, response.Data.Roles);
        Assert.Equal(80, response.Data.RefreshToken.Length);
        Assert.Equal(Constants.SuperAdminUser, principal.FindFirst("uid")?.Value);
        Assert.Contains("SuperAdmin", roleClaims);
    }

    [Fact]
    public async Task AuthenticateAsync_UnknownEmail_Throws()
    {
        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            _service.AuthenticateAsync(new AuthenticationRequest { Email = "nobody@example.com", Password = SeededPassword }));

        Assert.StartsWith("No Accounts Registered", ex.Message);
    }

    [Fact]
    public async Task AuthenticateAsync_WrongPassword_Throws()
    {
        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            _service.AuthenticateAsync(new AuthenticationRequest { Email = SeededEmail, Password = "Wrong@123" }));

        Assert.StartsWith("Invalid Credentials", ex.Message);
    }

    [Fact]
    public async Task AuthenticateAsync_UnconfirmedUser_Throws()
    {
        var registration = NewRegistration();
        await _service.RegisterAsync(registration, Origin);

        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            _service.AuthenticateAsync(new AuthenticationRequest { Email = registration.Email, Password = registration.Password }));

        Assert.StartsWith("Account Not Confirmed", ex.Message);
    }

    [Fact]
    public async Task RegisterAsync_CreatesBasicUser_AndReturnsVerificationLink()
    {
        var registration = NewRegistration();

        var response = await _service.RegisterAsync(registration, Origin);

        var userManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync(registration.Email);
        Assert.NotNull(user);
        Assert.True(response.Success);
        Assert.Equal(user.Id, response.Data);
        Assert.Contains($"{Origin}/api/account/confirm-email/?userId={user.Id}", response.Message);
        Assert.Equal([Roles.Basic.ToString()], await userManager.GetRolesAsync(user));
        Assert.False(user.EmailConfirmed);
        _emailService.Verify(s => s.SendEmailAsync(It.IsAny<MailRequest>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithEmailFeatureEnabled_SendsConfirmationEmail()
    {
        _featureManager.Setup(f => f.IsEnabledAsync(nameof(FeatureManagement.EnableEmailService))).ReturnsAsync(true);

        await _service.RegisterAsync(NewRegistration(), Origin);

        Assert.Equal("Confirm Registration", Assert.Single(_sentEmails).Subject);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateUserName_Throws()
    {
        var registration = NewRegistration();
        registration.UserName = "superadmin";

        var ex = await Assert.ThrowsAsync<ApiException>(() => _service.RegisterAsync(registration, Origin));

        Assert.Equal("Username 'superadmin' is already taken.", ex.Message);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_Throws()
    {
        var registration = NewRegistration();
        registration.Email = SeededEmail;

        var ex = await Assert.ThrowsAsync<ApiException>(() => _service.RegisterAsync(registration, Origin));

        Assert.Equal($"Email {SeededEmail} is already registered.", ex.Message);
    }

    [Fact]
    public async Task RegisterAsync_WeakPassword_ThrowsIdentityError()
    {
        await Assert.ThrowsAsync<ApiException>(() => _service.RegisterAsync(NewRegistration(password: "weak"), Origin));
    }

    [Fact]
    public async Task ConfirmEmailAsync_WithIssuedCode_ConfirmsAccount_SoUserCanSignIn()
    {
        var registration = NewRegistration();
        var registered = await _service.RegisterAsync(registration, Origin);
        var (userId, code) = ParseVerificationUri(registered.Message);

        var confirmed = await _service.ConfirmEmailAsync(userId, code);
        var signIn = await _service.AuthenticateAsync(new AuthenticationRequest { Email = registration.Email, Password = registration.Password });

        Assert.Equal(userId, confirmed.Data);
        Assert.True(signIn.Data.IsVerified);
        Assert.Equal(["Basic"], signIn.Data.Roles);
    }

    [Fact]
    public async Task ConfirmEmailAsync_InvalidCode_Throws()
    {
        var registered = await _service.RegisterAsync(NewRegistration(), Origin);
        var (userId, _) = ParseVerificationUri(registered.Message);
        var bogus = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("not-a-token"));

        await Assert.ThrowsAsync<ApiException>(() => _service.ConfirmEmailAsync(userId, bogus));
    }

    [Fact]
    public async Task ForgotPassword_UnknownEmail_SendsNothing()
    {
        await _service.ForgotPassword(new ForgotPasswordRequest { Email = "nobody@example.com" }, Origin);

        Assert.Empty(_sentEmails);
    }

    [Fact]
    public async Task ForgotPassword_ThenResetPassword_ChangesThePassword()
    {
        await _service.ForgotPassword(new ForgotPasswordRequest { Email = SeededEmail }, Origin);
        var email = Assert.Single(_sentEmails);
        var token = email.Body["You reset token is - ".Length..];

        var reset = await _service.ResetPassword(new ResetPasswordRequest
        {
            Email = SeededEmail,
            Token = token,
            Password = "NewPassword@456",
            ConfirmPassword = "NewPassword@456"
        });
        var signIn = await _service.AuthenticateAsync(new AuthenticationRequest { Email = SeededEmail, Password = "NewPassword@456" });

        Assert.Equal(SeededEmail, email.ToEmail);
        Assert.Equal("Reset Password", email.Subject);
        Assert.True(reset.Success);
        Assert.True(signIn.Success);
    }

    [Fact]
    public async Task ResetPassword_UnknownEmail_Throws()
    {
        await Assert.ThrowsAsync<ApiException>(() => _service.ResetPassword(new ResetPasswordRequest
        {
            Email = "nobody@example.com",
            Token = "x",
            Password = "NewPassword@456"
        }));
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_Throws()
    {
        var ex = await Assert.ThrowsAsync<ApiException>(() => _service.ResetPassword(new ResetPasswordRequest
        {
            Email = SeededEmail,
            Token = "invalid",
            Password = "NewPassword@456"
        }));

        Assert.StartsWith("Error occured while reseting", ex.Message);
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
