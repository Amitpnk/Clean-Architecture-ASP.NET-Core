using Asp.Versioning;
using CleanArch.Application.CQRS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class BaseController : ControllerBase
{
    private IDispatcher _dispatcher;

    protected IDispatcher Dispatcher => _dispatcher ??= HttpContext.RequestServices.GetRequiredService<IDispatcher>();
}
