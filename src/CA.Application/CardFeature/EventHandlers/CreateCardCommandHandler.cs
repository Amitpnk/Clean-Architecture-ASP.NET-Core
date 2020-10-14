using AutoMapper;
using CA.Application.CardFeature.Commands;
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
    public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, CardViewModel>
    {
        private readonly IGenericRepository<Card, Guid> _genericRepository;
        private readonly IMapper _mapper;
        public CreateCardCommandHandler(IGenericRepository<Card, Guid> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }
        public async Task<CardViewModel> Handle(CreateCardCommand request, CancellationToken cancellationToken)
        {
            var entity = new Card
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                Synonmys = request.Synonmys,
                Meaning = request.Meaning,
                Chapter = request.Chapter,
                Verse = request.Verse
            };

             _mapper.Map<Card>(request);

            // inserting logic goes here

            return _mapper.Map<CardViewModel>(entity);
        }


    }
}