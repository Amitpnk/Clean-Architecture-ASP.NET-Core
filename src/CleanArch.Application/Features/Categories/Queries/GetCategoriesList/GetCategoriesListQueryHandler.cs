using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Entities;
using MediatR;

namespace CleanArch.Application.Features.Categories.Queries.GetCategoriesList;

public class GetCategoriesListQueryHandler(IMapper mapper, IGenericRepositoryAsync<Category> categoryRepository)
    : IRequestHandler<GetCategoriesListQuery, List<CategoryListVm>>
{
    public async Task<List<CategoryListVm>> Handle(GetCategoriesListQuery request, CancellationToken cancellationToken)
    {
        var allCategories = (await categoryRepository.ListAllAsync()).OrderBy(x => x.Name);
        return mapper.Map<List<CategoryListVm>>(allCategories);
    }
}