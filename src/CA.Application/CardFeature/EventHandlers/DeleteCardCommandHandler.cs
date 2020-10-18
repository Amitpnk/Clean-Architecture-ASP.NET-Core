using AutoMapper;
using CA.Application.CardFeature.Commands;
using CA.Domain.Contract;
using CA.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CA.Application.CardFeature.EventHandlers
{
    public class DeleteCardCommandHandler : IRequestHandler<DeleteCardCommand>
    {
        private readonly IGenericRepository<Card, Guid> _genericRepository;
        private readonly IMapper _mapper;
        public DeleteCardCommandHandler(IGenericRepository<Card, Guid> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        // TODO
        Task<Unit> IRequestHandler<DeleteCardCommand, Unit>.Handle(DeleteCardCommand request, CancellationToken cancellationToken)
        {
            _genericRepository.Delete(request.CardId);
            _genericRepository.Save();
            // TODO 
            return null;
        }
    }
}