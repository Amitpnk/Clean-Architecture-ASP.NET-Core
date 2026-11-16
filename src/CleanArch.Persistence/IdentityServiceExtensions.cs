using System.Text;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Services.Authentication;
using CleanArch.Persistence.Context;
using CleanArch.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CleanArch.Persistence;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // UserManager / SignInManager / RoleManager backed by ApplicationDbContext
        // (it derives from IdentityDbContext<ApplicationUser> via AuditableContext).
        // AddIdentity also registers the Identity cookie handler that SignInManager.PasswordSignInAsync needs.
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddTransient<IAuthenticationService, AuthenticationService>();

        // Registered after AddIdentity, so JWT (not the Identity cookie) is the default API scheme.
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
                };

                o.Events = new JwtBearerEvents
                {
                    // Invalid/expired token: treat as anonymous. OnChallenge then returns the 401
                    // (previously this wrote a 500 with the full exception text to the client).
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync(JsonConvert.SerializeObject("401 Not authorized"));
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync(JsonConvert.SerializeObject("403 Not authorized"));
                    },
                };
            });

        return services;
    }
}
