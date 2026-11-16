using CleanArch.Application.CQRS;

namespace CleanArch.Application.Features.Categories.Queries.GetCategoriesListWithEvents;

public class GetCategoriesListWithEventsQuery : IQuery<List<CategoryEventListVm>>
{
    public bool IncludeHistory { get; set; }
}