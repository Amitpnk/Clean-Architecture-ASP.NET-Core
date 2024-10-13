using MediatR;

namespace CleanArch.Application.Features.Categories.Queries.GetCategoriesList;

public class GetCategoriesListQuery : IRequest<List<CategoryListVm>>
{

}