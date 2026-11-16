using MapsterMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Entities;
using CleanArch.Application.CQRS;

namespace CleanArch.Application.Features.Events.Queries.GetEventsList;

public class GetEventsListQueryHandler(IMapper mapper, IGenericRepositoryAsync<Event> eventRepository)
    : IQueryHandler<GetEventsListQuery, List<EventListVm>>
{
    public async Task<List<EventListVm>> Handle(GetEventsListQuery request, CancellationToken cancellationToken)
    {
        var allEvents = (await eventRepository.ListAllAsync()).OrderBy(x => x.Date);
        return mapper.Map<List<EventListVm>>(allEvents);
    }
}