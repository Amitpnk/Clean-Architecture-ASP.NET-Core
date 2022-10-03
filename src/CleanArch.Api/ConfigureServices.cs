using CleanArch.CrossCuttingConcerns.Identity;
using CleanArch.CrossCuttingConcerns.OS;
using CleanArch.CrossCuttingConcerns.Utility;
using CleanArch.Infrastructure.Identity;
using CleanArch.Infrastructure.OS;
using CleanArch.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Api
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();



            services.AddScoped<ILoggedInUserService, LoggedInUserService>();

            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();





            services.AddCors(options =>
            {
                //options.AddPolicy("AllowedOrigins", builder => builder
                //    .WithOrigins(_appSettings.CORS.AllowedOrigins)
                //    .AllowAnyMethod()
                //    .AllowAnyHeader());

                options.AddPolicy("AllowAnyOrigin", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                //options.AddPolicy("CustomPolicy", builder => builder
                //    .AllowAnyOrigin()
                //    .WithMethods("Get")
                //    .WithHeaders("Content-Type"));
            });

            services.AddHealthChecks()
              .AddDbContextCheck<ApplicationDbContext>(name: "Application DB Context", failureStatus: HealthStatus.Degraded)
              //.AddUrlGroup(new Uri(_appSettings.ApplicationDetail.ContactWebsite), name: "My personal website", failureStatus: HealthStatus.Degraded)              //.AddUrlGroup(new Uri(_appSettings.ApplicationDetail.ContactWebsite), name: "My personal website", failureStatus: HealthStatus.Degraded)
              .AddSqlServer(configuration.GetConnectionString("CleanArchConnectionString"));

            services.AddHealthChecksUI(setupSettings: setup =>
            {
                setup.AddHealthCheckEndpoint("Basic Health Check", $"/healthz");
            }).AddInMemoryStorage();

            AddSwagger(services);

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            services.AddFeatureManagement();
            return services;
        }

        private static void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                    });

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CleanArch.Api v1",

                });

                c.OperationFilter<FileResultContentTypeOperationFilter>();
            });
        }
    }
}
