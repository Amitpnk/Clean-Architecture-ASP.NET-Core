using CA.Domain.Common;
using CA.Domain.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CA.Application
{
    public class GetEntititesQuery<TEntity> : IQuery<List<TEntity>>
         where TEntity : AggregateRoot<Guid>
    {
    }

    internal class GetEntititesQueryHandler<TEntity> : IQueryHandler<GetEntititesQuery<TEntity>, List<TEntity>>
    where TEntity : AggregateRoot<Guid>
    {
        private readonly IGenericRepository<TEntity, Guid> _repository;

        public GetEntititesQueryHandler(IGenericRepository<TEntity, Guid> repository)
        {
            _repository = repository;
        }

        public List<TEntity> Handle(GetEntititesQuery<TEntity> query)
        {
            return _repository.GetAll().ToList();
        }
    }
}
