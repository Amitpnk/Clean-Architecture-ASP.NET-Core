using CA.Domain.Common;
using CA.Domain.Contract;
using CA.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CA.Persistance.Repositories
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey>
        where T : AggregateRoot<TKey>
    {
        private readonly ApplicationDbContext _dbContext;
        protected DbSet<T> DbSet => _dbContext.Set<T>();

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _dbContext;
            }
        }

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddOrUpdate(T entity)
        {
            if (entity.Id.Equals(default(TKey)))
            {
                DbSet.Add(entity);
            }
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public IQueryable<T> GetAll()
        {
            return _dbContext.Set<T>();
        }

        public Task<T> GetByIdAsync(TKey id)
        {
            //return _dbContext<T>.FindAsync(id);
            return null;
        }
    }
}
