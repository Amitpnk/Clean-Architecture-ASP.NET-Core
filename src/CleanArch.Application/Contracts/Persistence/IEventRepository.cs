using CleanArch.Domain.Entities;

namespace CleanArch.Application.Contracts.Persistence;

public interface IEventRepository : IGenericRepositoryAsync<Event>
{
    Task<bool> IsEventNameAndDateUnique(string name, DateTime eventDate);
}