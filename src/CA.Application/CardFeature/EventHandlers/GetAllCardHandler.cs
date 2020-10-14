using AutoMapper;
using CA.Application.CardFeature.Queries;
using CA.Application.CardFeature.ViewModel;
using CA.Domain.Contract;
using CA.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CA.Application.CardFeature.EventHandlers
{
    public class GetAllCardHandler : IRequestHandler<GetAllCardsQuery, IEnumerable<CardViewModel>>
    {

        private readonly IGenericRepository<Card, Guid> _genericRepository;
        private readonly IMapper _mapper;

        public GetAllCardHandler(IGenericRepository<Card, Guid> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CardViewModel>> Handle(GetAllCardsQuery request, CancellationToken cancellationToken)
        {
            var cardsList = await _genericRepository.GetAll().ToListAsync(cancellationToken);
            var cardsListVm = _mapper.Map<IEnumerable<CardViewModel>>(cardsList);
            return cardsListVm;
        }
      
     
    }
}
