using AutoMapper;
using CA.Application.GroupFeature.ViewModel;
using CA.Domain.Contract;
using CA.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CA.Application.GroupFeature.Queries
{
    public class GetAllGroupsQuery : IRequest<IEnumerable<GroupViewModel>>
    {
        //    public int PageNumber { get; set; }
        //    public int PageSize { get; set; }
    }

    public class GetAllGroupHandler : IRequestHandler<GetAllGroupsQuery, IEnumerable<GroupViewModel>>
    {

        private readonly IGenericRepositoryAsync<Group, Guid> _genericRepository;
        private readonly IMapper _mapper;

        public GetAllGroupHandler(IGenericRepositoryAsync<Group, Guid> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GroupViewModel>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
        {
            var GroupsList = await _genericRepository.GetAllAsync();
            var GroupsListVm = _mapper.Map<IEnumerable<GroupViewModel>>(GroupsList);
            return GroupsListVm;
        }
    }
}
