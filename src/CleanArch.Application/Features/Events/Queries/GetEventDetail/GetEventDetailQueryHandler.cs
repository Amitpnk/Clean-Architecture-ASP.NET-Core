using MapsterMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;
using CleanArch.Application.CQRS;

namespace CleanArch.Application.Features.Events.Queries.GetEventDetail;

public class GetEventDetailQueryHandler(
    IMapper mapper,
    IGenericRepositoryAsync<Event> eventRepository,
    IGenericRepositoryAsync<Category> categoryRepository)
    : IQueryHandler<GetEventDetailQuery, EventDetailVm>
{
    public async Task<EventDetailVm> Handle(GetEventDetailQuery request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Event), request.Id);
        var eventDetailDto = mapper.Map<EventDetailVm>(@event);

        var category = await categoryRepository.GetByIdAsync(@event.CategoryId)
            ?? throw new NotFoundException(nameof(Category), @event.CategoryId);

        eventDetailDto.Category = mapper.Map<CategoryDto>(category);

        return eventDetailDto;
    }
}