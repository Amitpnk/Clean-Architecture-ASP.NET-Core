using CleanArch.Domain.Entities;

namespace CleanArch.Application.Contracts.Persistence;

public interface ICategoryRepository : IGenericRepositoryAsync<Category>
{
    Task<List<Category>> GetCategoriesWithEvents(bool includePassedEvents);
    Task<bool> IsCategoryName(string name);
}