using CA.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;

namespace CA.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}

