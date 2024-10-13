using CleanArch.CrossCuttingConcerns.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CleanArch.Infrastructure.Identity;


// refer CurrentWebUser
public class LoggedInUserService(IHttpContextAccessor httpContextAccessor) : ILoggedInUserService
{
    //UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    //Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();

    public bool IsAuthenticated => httpContextAccessor.HttpContext is { User.Identity.IsAuthenticated: true };

    public string UserId
    {
        get
        {
            if (httpContextAccessor.HttpContext != null)
            {
                var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? httpContextAccessor.HttpContext.User.FindFirst("sub")?.Value;

                return userId;
            }

            return null;
        }
    }

    public List<KeyValuePair<string, string>> Claims
    {
        get
        {
            return httpContextAccessor.HttpContext?.User.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(
                item.Type,
                item.Value)).ToList();
        }
    }

}