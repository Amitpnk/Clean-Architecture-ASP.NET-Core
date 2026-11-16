using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.CQRS;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;
using MapsterMapper;

namespace CleanArch.Application.Features.Events.Commands.UpdateEvent;

public class UpdateEventCommandHandler(IMapper mapper, IGenericRepositoryAsync<Event> eventRepository)
    : CommandHandler<UpdateEventCommand>
{
    protected override async Task HandleAsync(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var eventToUpdate = await eventRepository.GetByIdAsync(request.EventId)
            ?? throw new NotFoundException(nameof(Event), request.EventId);

        var validator = new UpdateEventCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Count > 0)
            throw new ValidationException(validationResult);

        mapper.Map(request, eventToUpdate);

        await eventRepository.UpdateAsync(eventToUpdate);
    }
}
