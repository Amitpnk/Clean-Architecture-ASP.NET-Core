using CA.Domain.Contract;
using CA.Persistance.Context;
using CA.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Persistance
{
    public static class PersistenceExtensions
    {
        public static void AddPersistence(this IServiceCollection serviceCollection, IConfiguration configuration, IConfigurationRoot configRoot)
        {
            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("CleanArchConn") ?? configRoot["ConnectionStrings:CleanArchConn"]
            , b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            serviceCollection.AddScoped(typeof(IGenericRepositoryAsync<,>), typeof(GenericRepositoryAsync<,>));
        }
    }
}
