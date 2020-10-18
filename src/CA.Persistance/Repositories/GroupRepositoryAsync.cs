using CA.Application.GroupFeature.Service;
using CA.Domain.Entities;
using CA.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Persistance.Repositories
{
    public class GroupRepositoryAsync : GenericRepositoryAsync<Group, Guid>, IGroupRepositoryAsync
    {
        private readonly DbSet<Group> _group;
        public GroupRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _group = dbContext.Set<Group>();
        }
        public Task<bool> IsUniqueGroupNameAsync(string name)
        {
            return _group
               .AllAsync(p => p.Name != name);
        }
    }
}
