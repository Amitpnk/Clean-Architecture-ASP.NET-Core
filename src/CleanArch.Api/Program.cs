using CleanArch.Persistence.Context;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            applicationDbContext.Database.Migrate();

            var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            identityDbContext.Database.Migrate();
        }

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}