using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;
using MediatR;

namespace CleanArch.Application.Features.Events.Commands.UpdateEvent;

public class UpdateEventCommandHandler(IMapper mapper, IGenericRepositoryAsync<Event> eventRepository)
    : IRequestHandler<UpdateEventCommand>
{
    public async Task<Unit> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {

        var eventToUpdate = await eventRepository.GetByIdAsync(request.EventId);

        if (eventToUpdate == null)
        {
            throw new NotFoundException(nameof(Event), request.EventId);
        }

        var validator = new UpdateEventCommandValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Count > 0)
            throw new ValidationException(validationResult);

        mapper.Map(request, eventToUpdate, typeof(UpdateEventCommand), typeof(Event));

        await eventRepository.UpdateAsync(eventToUpdate);

        return Unit.Value;
    }
}