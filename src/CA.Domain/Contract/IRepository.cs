using CA.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Domain.Contract
{
     public interface IRepository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        IUnitOfWork UnitOfWork { get; }

        IQueryable<TEntity> GetAll();

        void AddOrUpdate(TEntity entity);

        void Delete(TEntity entity);
    }
}
