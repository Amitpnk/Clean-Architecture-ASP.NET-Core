using CA.Application.GroupFeature.ViewModel;
using MediatR;

namespace CA.Application.GroupFeature.Commands
{
    public class CreateGroupCommand : IRequest<GroupViewModel>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
