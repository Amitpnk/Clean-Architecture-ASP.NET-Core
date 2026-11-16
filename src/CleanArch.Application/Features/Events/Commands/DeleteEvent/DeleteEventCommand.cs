using CleanArch.Application.CQRS;

namespace CleanArch.Application.Features.Events.Commands.DeleteEvent;

public class DeleteEventCommand : ICommand
{
    public Guid EventId { get; set; }
}