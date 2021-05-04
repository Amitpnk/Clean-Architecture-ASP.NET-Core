using CA.Domain.Contract;
using CA.Domain.Entities;
using System;

namespace CA.Application.CardFeature.Service
{
    public interface ICardRepositoryAsync : IGenericRepositoryAsync<Card, Guid>
    {

    }
}
