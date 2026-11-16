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
        return Ok(await authenticationService.RegisterAsync(request, GetOrigin()));
    }
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
    {
        return Ok(await authenticationService.ConfirmEmailAsync(userId, code));
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
    {
        await authenticationService.ForgotPassword(model, GetOrigin());
        return Ok();
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {

        return Ok(await authenticationService.ResetPassword(model));
    }

    // Browsers send Origin on cross-origin POSTs; API clients (curl, Postman, services) often don't.
    // Fall back to this API's own base URL so confirmation/reset links are always absolute.
    private string GetOrigin() =>
        Request.Headers.Origin.FirstOrDefault() is { Length: > 0 } origin
            ? origin
            : $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

}