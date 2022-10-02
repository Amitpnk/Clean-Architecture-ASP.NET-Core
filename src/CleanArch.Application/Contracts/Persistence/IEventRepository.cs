using CleanArch.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CleanArch.Application.Contracts.Persistence
{
    public interface IEventRepository : IGenericRepositoryAsync<Event>
    {
        Task<bool> IsEventNameAndDateUnique(string name, DateTime eventDate);
    }
}