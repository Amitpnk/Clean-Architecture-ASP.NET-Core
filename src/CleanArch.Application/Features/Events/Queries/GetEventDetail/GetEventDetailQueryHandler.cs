using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;
using MediatR;

namespace CleanArch.Application.Features.Events.Queries.GetEventDetail;

public class GetEventDetailQueryHandler(
    IMapper mapper,
    IGenericRepositoryAsync<Event> eventRepository,
    IGenericRepositoryAsync<Category> categoryRepository)
    : IRequestHandler<GetEventDetailQuery, EventDetailVm>
{
    public async Task<EventDetailVm> Handle(GetEventDetailQuery request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(request.Id);
        var eventDetailDto = mapper.Map<EventDetailVm>(@event);

        var category = await categoryRepository.GetByIdAsync(@event.CategoryId);

        if (category == null)
        {
            throw new NotFoundException(nameof(Event), request.Id);
        }
        eventDetailDto.Category = mapper.Map<CategoryDto>(category);

        return eventDetailDto;
    }
}