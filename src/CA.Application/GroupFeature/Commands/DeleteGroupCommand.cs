using MediatR;
using System;

namespace CA.Application.GroupFeature.Commands
{
    public class DeleteGroupCommand : IRequest
    {
        public Guid GroupId { get; set; }

        public DeleteGroupCommand(Guid id)
        {
            GroupId = id;
        }

    }
}
