using CA.Domain.Contract;
using CA.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CA.Application.GroupFeature.Service
{
    public interface IGroupRepositoryAsync : IGenericRepositoryAsync<Group, Guid>
    {
        Task<bool> IsUniqueGroupNameAsync(string name);
    }
}
