using MediatR;
using System.Collections.Generic;

namespace CleanArch.Application.Features.Events.Queries.GetEventsList
{
    public class GetEventsListQuery : IRequest<List<EventListVm>>
    {

    }
}
