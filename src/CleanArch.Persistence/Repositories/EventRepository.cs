using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Entities;
using CleanArch.Persistence.Context;

namespace CleanArch.Persistence.Repositories;

public class EventRepository(ApplicationDbContext dbContext) : GenericRepository<Event>(dbContext), IEventRepository
{
    public Task<bool> IsEventNameAndDateUnique(string name, DateTime eventDate)
    {
        var matches = _dbContext.Events.Any(e => e.Name.Equals(name) && e.Date.Date.Equals(eventDate.Date));
        return Task.FromResult(matches);
    }
}