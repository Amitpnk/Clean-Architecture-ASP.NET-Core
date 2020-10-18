using AutoMapper;
using CA.Application.GroupFeature.Commands;
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
        private readonly IGenericRepository<Group, Guid> _genericRepository;
        private readonly IMapper _mapper;
        public DeleteGroupCommandHandler(IGenericRepository<Group, Guid> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        // TODO
        Task<Unit> IRequestHandler<DeleteGroupCommand, Unit>.Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            _genericRepository.Delete(request.GroupId);
            _genericRepository.Save();
            // TODO 
            return null;
        }
    }
}