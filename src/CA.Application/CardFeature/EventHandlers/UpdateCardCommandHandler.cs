using AutoMapper;
using CA.Application.CardFeature.Commands;
using CA.Application.CardFeature.ViewModel;
using CA.Domain.Contract;
using CA.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CA.Application.CardFeature.EventHandlers
{
    public class UpdateCardCommandHandler : IRequestHandler<UpdateCardCommand, CardViewModel>
    {
        private readonly IGenericRepository<Card, Guid> _genericRepository;
        private readonly IMapper _mapper;
        public UpdateCardCommandHandler(IGenericRepository<Card, Guid> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }
        public async Task<CardViewModel> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
        {
            var entity = new Card
            {
                Id = request.Id,
                Description = request.Description,
                Synonmys = request.Synonmys,
                Meaning = request.Meaning,
                Chapter = request.Chapter,
                Verse = request.Verse
            };

            _genericRepository.Update(entity);
            _genericRepository.Save();

            return _mapper.Map<CardViewModel>(entity);
        }


    }
}