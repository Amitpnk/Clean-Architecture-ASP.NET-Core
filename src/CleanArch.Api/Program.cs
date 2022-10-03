using CleanArch.Api;
using CleanArch.Application;
using CleanArch.Application.Middleware;
using CleanArch.Infrastructure;
using CleanArch.Persistence;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

//log.AddSerilog();
builder.Host.UseSerilog();
app.UseAuthentication();

app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanArch.Api v1"));

app.UseHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                },
}).UseHealthChecksUI(setup =>
{
    setup.ApiPath = "/healthcheck";
    setup.UIPath = "/healthcheck-ui";
});

app.UseCustomExceptionHandler();

app.UseCors("Open");

//app.UseCors(_appSettings.CORS.AllowAnyOrigin ? "AllowAnyOrigin" : "AllowedOrigins");
app.UseCors("AllowAnyOrigin");

app.MapControllers();

app.Run();

 