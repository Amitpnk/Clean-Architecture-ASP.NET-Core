using AutoMapper;
using CA.Application.CardFeature.Commands;
using CA.Application.CardFeature.ViewModel;
using CA.CrossCuttingConcerns.Exceptions;
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
        private readonly IGenericRepositoryAsync<Card, Guid> _genericRepository;
        private readonly IMapper _mapper;
        public UpdateCardCommandHandler(IGenericRepositoryAsync<Card, Guid> genericRepository, IMapper mapper)
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

            var card = await _genericRepository.GetByIdAsync(request.Id);
            if (card == null)
            {
                throw new NotFoundException(nameof(Card), request.Id);
            }

            await _genericRepository.UpdateAsync(entity);
            _genericRepository.SaveChanges();

            return _mapper.Map<CardViewModel>(entity);
        }


    }
}