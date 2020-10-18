using AutoMapper;
using CA.Application.GroupFeature.Commands;
using CA.CrossCuttingConcerns.Exceptions;
using CA.Domain.Contract;
using CA.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CA.Application.GroupFeature.EventHandlers
{
    public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand>
    {
        private readonly IGenericRepositoryAsync<Group, Guid> _genericRepository;
        private readonly IMapper _mapper;
        public DeleteGroupCommandHandler(IGenericRepositoryAsync<Group, Guid> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _genericRepository.GetByIdAsync(request.GroupId);
            if (group == null)
            {
                throw new NotFoundException(nameof(Card), request.GroupId);
            }

            await _genericRepository.DeleteAsync(request.GroupId);
            _genericRepository.SaveChanges();

            return Unit.Value;
        }
    }
}