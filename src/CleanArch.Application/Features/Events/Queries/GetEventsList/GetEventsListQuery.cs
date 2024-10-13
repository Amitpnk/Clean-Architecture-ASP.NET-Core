using MediatR;

namespace CleanArch.Application.Features.Events.Queries.GetEventsList;

public class GetEventsListQuery : IRequest<List<EventListVm>>
{

}