using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesList;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Common.UnitTests.Mocks;
using MapsterMapper;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Categories.Queries;

public class GetCategoriesListQueryHandlerTests
{
    private IMapper _mapper;
    private Mock<ICategoryRepository> _mockCategoryRepository;

    [SetUp]
    public void SetUp()
    {
        _mockCategoryRepository = RepositoryMocks.GetCategoryRepository();
        _mapper = MapperFactory.Create();
    }

    [Test]
    public async Task GetCategoriesListTest()
    {
        var handler = new GetCategoriesListQueryHandler(_mapper, _mockCategoryRepository.Object);

        var result = await handler.Handle(new GetCategoriesListQuery(), CancellationToken.None);

        var categories = await _mockCategoryRepository.Object.ListAllAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.TypeOf<List<CategoryListVm>>());
            Assert.That(result, Has.Count.EqualTo(4));
            // Sorted by name, and Category.Id is mapped onto CategoryListVm.CategoryId.
            Assert.That(result.Select(c => c.Name), Is.Ordered);
            Assert.That(result.Select(c => c.CategoryId), Is.EquivalentTo(categories.Select(c => c.Id)));
        }
    }
}
