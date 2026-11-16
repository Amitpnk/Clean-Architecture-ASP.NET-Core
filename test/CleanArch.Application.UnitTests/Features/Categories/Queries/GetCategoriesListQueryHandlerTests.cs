using CleanArch.Application.Features.Categories.Queries.GetCategoriesList;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Common.UnitTests.Mocks;

namespace CleanArch.Application.UnitTests.Features.Categories.Queries;

public class GetCategoriesListQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllCategories_SortedByName()
    {
        var repository = RepositoryMocks.GetCategoryRepository();
        var handler = new GetCategoriesListQueryHandler(MapperFactory.Create(), repository.Object);

        var result = await handler.Handle(new GetCategoriesListQuery(), TestContext.Current.CancellationToken);

        var categories = await repository.Object.ListAllAsync();
        Assert.IsType<List<CategoryListVm>>(result);
        Assert.Equal(4, result.Count);
        // Sorted by name, and Category.Id is mapped onto CategoryListVm.CategoryId.
        Assert.Equal(result.Select(c => c.Name).Order(), result.Select(c => c.Name));
        Assert.Equal(categories.Select(c => c.Id).Order(), result.Select(c => c.CategoryId).Order());
    }
}
