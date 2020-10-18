using AutoMapper;
using CA.Application.CardFeature.ViewModel;
using CA.Domain.Contract;
using CA.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CA.Application.CardFeature.Queries
{
    public class GetAllCardsQuery : IRequest<IEnumerable<CardViewModel>>
    {
        //    public int PageNumber { get; set; }
        //    public int PageSize { get; set; }
    }

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
            var cardsList = _genericRepository.GetAll();  //.ToListAsync(cancellationToken);
            var cardsListVm = _mapper.Map<IEnumerable<CardViewModel>>(cardsList);
            return cardsListVm;
        }


    }
}
