using MediatR;

namespace CleanArch.Application.Features.Categories.Queries.GetCategoriesListWithEvents;

public class GetCategoriesListWithEventsQuery : IRequest<List<CategoryEventListVm>>
{
    public bool IncludeHistory { get; set; }
}