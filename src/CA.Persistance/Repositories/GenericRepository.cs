using CA.Domain.Common;
using CA.Domain.Contract;
using CA.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CA.Persistance.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<TEntity> table;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            table = _dbContext.Set<TEntity>();
        }

        public void Add(TEntity obj)
        {
            table.Add(obj);
        }

        public void Delete(TKey id)
        {
            TEntity existing = table.Find(id);
            table.Remove(existing);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return table.ToList();
        }

        public TEntity GetById(TKey id)
        {
            return table.Find(id);
        }

        public bool Save()
        {
            return (_dbContext.SaveChanges() >= 0);
        }

        public void Update(TEntity obj)
        {
            table.Update(obj);
        }
    }
}