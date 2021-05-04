using Microsoft.AspNetCore.Builder;

namespace CleanArch.Application.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}

// Todo: move to cross cutting layer
//< PackageReference Include = "Microsoft.AspNetCore.Http.Abstractions" Version = "2.2.0" />