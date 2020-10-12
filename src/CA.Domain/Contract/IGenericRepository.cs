using CA.Domain.Common;
using System.Linq;

namespace CA.Domain.Contract
{
    public interface IGenericRepository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        IUnitOfWork UnitOfWork { get; }

        IQueryable<TEntity> GetAll();

        void AddOrUpdate(TEntity entity);

        void Delete(TEntity entity);
    }
}
