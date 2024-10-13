using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Entities;
using MediatR;

namespace CleanArch.Application.Features.Events.Queries.GetEventsList;

public class GetEventsListQueryHandler(IMapper mapper, IGenericRepositoryAsync<Event> eventRepository)
    : IRequestHandler<GetEventsListQuery, List<EventListVm>>
{
    public async Task<List<EventListVm>> Handle(GetEventsListQuery request, CancellationToken cancellationToken)
    {
        var allEvents = (await eventRepository.ListAllAsync()).OrderBy(x => x.Date);
        return mapper.Map<List<EventListVm>>(allEvents);
    }
}