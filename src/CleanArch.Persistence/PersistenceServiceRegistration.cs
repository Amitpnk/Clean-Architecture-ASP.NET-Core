using CleanArch.Application.Contracts.Persistence;
using CleanArch.Persistence.Context;
using CleanArch.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Identity (UserManager/SignInManager over ApplicationDbContext) is registered in AddIdentityServices.
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("CleanArchConnectionString"),
                sql => sql.EnableRetryOnFailure()));

        services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepository<>));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IEventRepository, EventRepository>();

        return services;
    }
}