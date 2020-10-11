using System.Threading;
using System.Threading.Tasks;

namespace CA.Application.Common.Interface
{
    public interface IApplicationDbContext
    {

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
