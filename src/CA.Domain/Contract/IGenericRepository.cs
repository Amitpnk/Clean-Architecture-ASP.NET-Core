using CA.Domain.Common;
using System.Collections.Generic;

namespace CA.Domain.Contract
{
    public interface IGenericRepository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {

        IEnumerable<TEntity> GetAll();
        TEntity GetById(TKey id);
        void Add(TEntity obj);
        void Update(TEntity obj);
        void Delete(TKey id);
        bool Save();
    }
}
