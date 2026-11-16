using CleanArch.Api.Account;
using CleanArch.Api.Controllers;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Common;
using CleanArch.Domain.Services.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CleanArch.Api.UnitTests.Controllers;

public class AccountControllerTests
{
    private Mock<IAuthenticationService> _auth;
    private AccountController _controller;
    private DefaultHttpContext _httpContext;

    [SetUp]
    public void SetUp()
    {
        _auth = new Mock<IAuthenticationService>();
        _httpContext = new DefaultHttpContext();
        _httpContext.Request.Scheme = "https";
        _httpContext.Request.Host = new HostString("api.example.com");
        _controller = new AccountController(_auth.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = _httpContext }
        };
    }

    [Test]
    public async Task Authenticate_ReturnsOkWithServiceResponse()
    {
        var request = new AuthenticationRequest { Email = "a@b.c", Password = "p" };
        var response = new BaseResponse<AuthenticationResponse>(new AuthenticationResponse { JWToken = "jwt" });
        _auth.Setup(a => a.AuthenticateAsync(request)).ReturnsAsync(response);

        var result = await _controller.AuthenticateAsync(request);

        Assert.That(((OkObjectResult)result).Value, Is.SameAs(response));
    }

    [Test]
    public async Task Register_UsesOriginHeader_WhenPresent()
    {
        _httpContext.Request.Headers.Origin = "https://app.example.com";
        var request = new RegistrationRequest();

        await _controller.RegisterAsync(request);

        _auth.Verify(a => a.RegisterAsync(request, "https://app.example.com"), Times.Once);
    }

    [Test]
    public async Task Register_FallsBackToRequestBaseUrl_WithoutOriginHeader()
    {
        _httpContext.Request.PathBase = "/v";
        var request = new RegistrationRequest();

        await _controller.RegisterAsync(request);

        _auth.Verify(a => a.RegisterAsync(request, "https://api.example.com/v"), Times.Once);
    }

    [Test]
    public async Task ConfirmEmail_ForwardsUserIdAndCode()
    {
        var response = new BaseResponse<string>("user-1", "ok");
        _auth.Setup(a => a.ConfirmEmailAsync("user-1", "code")).ReturnsAsync(response);

        var result = await _controller.ConfirmEmailAsync("user-1", "code");

        Assert.That(((OkObjectResult)result).Value, Is.SameAs(response));
    }

    [Test]
    public async Task ForgotPassword_ReturnsOk_AndPassesOrigin()
    {
        var request = new ForgotPasswordRequest { Email = "a@b.c" };

        var result = await _controller.ForgotPassword(request);

        Assert.That(result, Is.InstanceOf<OkResult>());
        _auth.Verify(a => a.ForgotPassword(request, "https://api.example.com"), Times.Once);
    }

    [Test]
    public async Task ResetPassword_ReturnsOkWithServiceResponse()
    {
        var request = new ResetPasswordRequest { Email = "a@b.c" };
        var response = new BaseResponse<string>("a@b.c", "reset");
        _auth.Setup(a => a.ResetPassword(request)).ReturnsAsync(response);

        var result = await _controller.ResetPassword(request);

        Assert.That(((OkObjectResult)result).Value, Is.SameAs(response));
    }
}

public class MetaControllerTests
{
    [Test]
    public void Info_ReturnsVersionAndLastUpdate()
    {
        var result = new MetaController().Info();

        Assert.That((string)((OkObjectResult)result.Result!).Value!, Does.StartWith("Version: ").And.Contains("Last Updated: "));
    }
}
