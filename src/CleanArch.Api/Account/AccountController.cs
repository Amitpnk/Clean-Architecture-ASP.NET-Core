using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.Api.Account;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("authenticate")]
    public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
    {
        return Ok(await authenticationService.AuthenticateAsync(request));
    }
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegistrationRequest request)
    {
        var origin = Request.Headers["origin"];
        return Ok(await authenticationService.RegisterAsync(request, origin));
    }
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
    {
        return Ok(await authenticationService.ConfirmEmailAsync(userId, code));
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
    {
        await authenticationService.ForgotPassword(model, Request.Headers["origin"]);
        return Ok();
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {

        return Ok(await authenticationService.ResetPassword(model));
    }

}