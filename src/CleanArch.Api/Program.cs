using Asp.Versioning;
using CleanArch.Application;
using CleanArch.Application.Middleware;
using CleanArch.CrossCuttingConcerns.Identity;
using CleanArch.CrossCuttingConcerns.OS;
using CleanArch.CrossCuttingConcerns.Utility;
using CleanArch.Domain.Services.Configuration;
using CleanArch.Infrastructure;
using CleanArch.Infrastructure.Identity;
using CleanArch.Infrastructure.OS;
using CleanArch.Persistence;
using CleanArch.Persistence.Context;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);

// Logging
builder.Services.AddSerilog((services, logger) => logger
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));

// Application layers
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationServices();
builder.Services.AddScoped<ILoggedInUserService, LoggedInUserService>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddFeatureManagement();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy => policy
        .WithOrigins(appSettings.CORS.AllowedOrigins)
        .AllowAnyMethod()
        .AllowAnyHeader());

    options.AddPolicy("AllowAnyOrigin", policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    options.AddPolicy("CustomPolicy", policy => policy
        .AllowAnyOrigin()
        .WithMethods("GET")
        .WithHeaders("Content-Type"));
});

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>(name: "Application DB Context", failureStatus: HealthStatus.Degraded)
    .AddUrlGroup(new Uri(appSettings.ApplicationDetail.ContactWebsite), name: "My personal website", failureStatus: HealthStatus.Degraded)
    .AddSqlServer(builder.Configuration.GetConnectionString("CleanArchConnectionString") ?? string.Empty);

// A relative URI is resolved against the server's bound address, which is unusable ([::]) inside a
// container, so docker-compose sets HealthChecks:SelfUri to an absolute URL.
var healthCheckUri = builder.Configuration["HealthChecks:SelfUri"] ?? "/healthz";
builder.Services.AddHealthChecksUI(setup => setup.AddHealthCheckEndpoint("Basic Health Check", healthCheckUri))
    .AddInMemoryStorage();

// API versioning
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        // Versioned routes use "api/v{version:apiVersion}/..."; "?api-version=" is still accepted.
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new QueryStringApiVersionReader());
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// Swagger / OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "CleanArch.Api v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Paste the token only (no 'Bearer ' prefix).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    options.OperationFilter<FileResultContentTypeOperationFilter>();
});

var app = builder.Build();

// Apply pending EF Core migrations (Identity + application tables live in ApplicationDbContext).
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCustomExceptionHandler();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(appSettings.CORS.AllowAnyOrigin ? "AllowAnyOrigin" : "AllowedOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanArch.Api v1"));

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
    },
});

app.MapHealthChecksUI(setup =>
{
    setup.ApiPath = "/healthcheck";
    setup.UIPath = "/healthcheck-ui";
});

app.MapControllers();

await app.RunAsync();

// Exposed for WebApplicationFactory-based integration tests and MetaController.
public partial class Program;
