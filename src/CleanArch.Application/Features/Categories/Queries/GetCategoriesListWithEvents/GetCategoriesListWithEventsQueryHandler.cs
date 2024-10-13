using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using MediatR;

namespace CleanArch.Application.Features.Categories.Queries.GetCategoriesListWithEvents;

public class GetCategoriesListWithEventsQueryHandler(IMapper mapper, ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoriesListWithEventsQuery, List<CategoryEventListVm>>
{
    public async Task<List<CategoryEventListVm>> Handle(GetCategoriesListWithEventsQuery request, CancellationToken cancellationToken)
    {
        var list = await categoryRepository.GetCategoriesWithEvents(request.IncludeHistory);
        return mapper.Map<List<CategoryEventListVm>>(list);
    }
}