using CA.Domain.Common;
using CA.Domain.Contract;
using CA.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CA.Persistance.Repositories
{
    public class GenericRepositoryAsync<TEntity, TKey> : IGenericRepositoryAsync<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<TEntity> table;

        public GenericRepositoryAsync(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            table = _dbContext.Set<TEntity>();
        }

        public async Task AddAsync(TEntity obj)
        {
            await table.AddAsync(obj);
        }

        public async Task DeleteAsync(TKey id)
        {
            TEntity existing = await table.FindAsync(id);
            table.Remove(existing);
        }


        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await table.ToListAsync();
        }


        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await table.FindAsync(id);
        }


        public bool SaveChanges()
        {
            return (_dbContext.SaveChanges() >= 0);
        }

        public async Task UpdateAsync(TEntity obj)
        {
            table.Update(obj);
        }
    }
}