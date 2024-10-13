using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Entities;
using CleanArch.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Persistence.Repositories;

public class CategoryRepository(ApplicationDbContext dbContext)
    : GenericRepository<Category>(dbContext), ICategoryRepository
{
    public async Task<List<Category>> GetCategoriesWithEvents(bool includePassedEvents)
    {
        var allCategories = await _dbContext.Categories.Include(x => x.Events).ToListAsync();
        if (!includePassedEvents)
        {
            allCategories.ForEach(p => p.Events.ToList().RemoveAll(c => c.Date < DateTime.Today));
        }
        return allCategories;
    }

    public Task<bool> IsCategoryName(string name)
    {
        var matches = _dbContext.Categories.Any(e => e.Name.Equals(name));
        return Task.FromResult(matches);
    }
}