using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;
using MediatR;

namespace CleanArch.Application.Features.Events.Commands.DeleteEvent;

public class DeleteEventCommandHandler(IMapper mapper, IGenericRepositoryAsync<Event> eventRepository)
    : IRequestHandler<DeleteEventCommand>
{
    private readonly IMapper _mapper = mapper;

    public async Task<Unit> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var eventToDelete = await eventRepository.GetByIdAsync(request.EventId);

        if (eventToDelete == null)
        {
            throw new NotFoundException(nameof(Event), request.EventId);
        }

        await eventRepository.DeleteAsync(eventToDelete);

        return Unit.Value;
    }
}