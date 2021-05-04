using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Entities;
using CleanArch.Persistence.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArch.Persistence.Repositories
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public Task<bool> IsEventNameAndDateUnique(string name, DateTime eventDate)
        {
            var matches = _dbContext.Events.Any(e => e.Name.Equals(name) && e.Date.Date.Equals(eventDate.Date));
            return Task.FromResult(matches);
        }
    }
}
