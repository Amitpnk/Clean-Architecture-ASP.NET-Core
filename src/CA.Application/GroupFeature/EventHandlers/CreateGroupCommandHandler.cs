using AutoMapper;
using CA.Application.GroupFeature.Commands;
using CA.Application.GroupFeature.ViewModel;
using CA.Domain.Contract;
using CA.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CA.Application.GroupFeature.EventHandlers
{
    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, GroupViewModel>
    {
        private readonly IGenericRepositoryAsync<Group, Guid> _genericRepository;
        private readonly IMapper _mapper;
        public CreateGroupCommandHandler(IGenericRepositoryAsync<Group, Guid> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }
        public async Task<GroupViewModel> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var entity = new Group
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive

            };

            await _genericRepository.AddAsync(entity);
            _genericRepository.SaveChanges();

            return _mapper.Map<GroupViewModel>(entity);
        }
    }
}