using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.CQRS;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;

namespace CleanArch.Application.Features.Events.Commands.DeleteEvent;

public class DeleteEventCommandHandler(IGenericRepositoryAsync<Event> eventRepository)
    : CommandHandler<DeleteEventCommand>
{
    protected override async Task HandleAsync(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var eventToDelete = await eventRepository.GetByIdAsync(request.EventId)
            ?? throw new NotFoundException(nameof(Event), request.EventId);

        await eventRepository.DeleteAsync(eventToDelete);
    }
}
