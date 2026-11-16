using System.Reflection;
using CleanArch.Application.CQRS;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Mapster: scan this assembly for IRegister configs and expose IMapper.
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(assembly);
        services.AddSingleton(mappingConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        // Custom CQRS dispatcher (replaces MediatR) and all command/query handlers.
        services.AddCqrs(assembly);

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
