using CA.Domain.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CA.Domain.Contract
{
    public interface IGenericRepository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        IUnitOfWork UnitOfWork { get; }
        Task<TEntity> GetByIdAsync(TKey id);
        IQueryable<TEntity> GetAll();

        void AddOrUpdate(TEntity entity);

        void Delete(TEntity entity);
    }
}
