using AutoMapper;
using CA.Application.CardFeature.Commands;
using CA.CrossCuttingConcerns.Exceptions;
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
        private readonly IGenericRepositoryAsync<Card, Guid> _genericRepository;
        private readonly IMapper _mapper;
        public DeleteCardCommandHandler(IGenericRepositoryAsync<Card, Guid> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
        {
            var card = await _genericRepository.GetByIdAsync(request.CardId);
            if (card == null)
            {
                throw new NotFoundException(nameof(Card), request.CardId);
            }

            await _genericRepository.DeleteAsync(request.CardId);
            _genericRepository.SaveChanges();

            return Unit.Value;
        }
    }
}