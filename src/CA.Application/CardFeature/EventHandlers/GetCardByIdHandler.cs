using AutoMapper;
using CA.Application.CardFeature.Queries;
using CA.Application.CardFeature.ViewModel;
using CA.Domain.Contract;
using CA.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CA.Application.CardFeature.EventHandlers
{
    public class GetCardByIdHandler : IRequestHandler<GetCardByIdQuery, CardViewModel>
    {
        private readonly IGenericRepository<Card, Guid> _genericRepository;
        private readonly IMapper _mapper;

        public GetCardByIdHandler(IGenericRepository<Card, Guid> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<CardViewModel> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _genericRepository.GetByIdAsync(request.CardId);
            if (entity == null)
            {
                //throw new NotFoundException(nameof(Card), request.Id);
            }
            return _mapper.Map<CardViewModel>(entity);
        }
    }
}
