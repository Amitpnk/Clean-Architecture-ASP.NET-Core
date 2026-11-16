using CleanArch.Application.CQRS;

namespace CleanArch.Application.Features.Events.Queries.GetEventDetail;

public class GetEventDetailQuery : IQuery<EventDetailVm>
{
    public Guid Id { get; set; }
}