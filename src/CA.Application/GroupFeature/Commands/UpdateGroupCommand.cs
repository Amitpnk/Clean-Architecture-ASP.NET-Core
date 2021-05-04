using CA.Application.GroupFeature.ViewModel;
using MediatR;
using System;

namespace CA.Application.GroupFeature.Commands
{
    public class UpdateGroupCommand : IRequest<GroupViewModel>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
