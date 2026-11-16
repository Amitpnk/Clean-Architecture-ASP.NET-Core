using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application.CQRS;

public static class CqrsServiceCollectionExtensions
{
    private static readonly Type[] HandlerInterfaces = [typeof(ICommandHandler<,>), typeof(IQueryHandler<,>)];

    /// <summary>Registers <see cref="IDispatcher"/> and every command/query handler found in <paramref name="assemblies"/>.</summary>
    public static IServiceCollection AddCqrs(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddScoped<IDispatcher, Dispatcher>();

        var registrations =
            from type in assemblies.SelectMany(a => a.DefinedTypes)
            where type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false }
            from serviceType in type.ImplementedInterfaces
            where serviceType.IsGenericType && HandlerInterfaces.Contains(serviceType.GetGenericTypeDefinition())
            select (serviceType, type);

        foreach (var (serviceType, implementationType) in registrations)
        {
            services.AddScoped(serviceType, implementationType);
        }

        return services;
    }
}
