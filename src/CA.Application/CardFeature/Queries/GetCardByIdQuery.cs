using AutoMapper;
using CA.Application.CardFeature.ViewModel;
using CA.CrossCuttingConcerns.Exceptions;
using CA.Domain.Contract;
using CA.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CA.Application.CardFeature.Queries
{
    public class GetCardByIdQuery : IRequest<CardViewModel>
    {
        public Guid CardId { get; set; }
        public GetCardByIdQuery(Guid id)
        {
            CardId = id;
        }
    }

    public class GetCardByIdHandler : IRequestHandler<GetCardByIdQuery, CardViewModel>
    {
        private readonly IGenericRepositoryAsync<Card, Guid> _genericRepository;
        private readonly IMapper _mapper;

        public GetCardByIdHandler(IGenericRepositoryAsync<Card, Guid> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<CardViewModel> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _genericRepository.GetByIdAsync(request.CardId);
            if (entity == null)
            {
                throw new NotFoundException(nameof(Card), request.CardId);
            }
            return _mapper.Map<CardViewModel>(entity);
        }
    }
}
