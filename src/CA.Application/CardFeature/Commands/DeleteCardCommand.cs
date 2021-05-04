using MediatR;
using System;

namespace CA.Application.CardFeature.Commands
{
    public class DeleteCardCommand : IRequest
    {
        public Guid CardId { get; set; }

        public DeleteCardCommand(Guid id)
        {
            CardId = id;
        }

    }
}
