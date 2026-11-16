using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Features.Categories.Commands.CreateCategory;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Common.UnitTests.Mocks;
using MapsterMapper;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Categories.Commands;

public class CreateCategoryTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private readonly IMapper _mapper = MapperFactory.Create();
    private readonly Mock<ICategoryRepository> _mockCategoryRepository = RepositoryMocks.GetCategoryRepository();

    [Fact]
    public async Task Handle_ValidCategory_AddedToCategoriesRepo()
    {
        var handler = new CreateCategoryCommandHandler(_mapper, _mockCategoryRepository.Object);

        var response = await handler.Handle(new CreateCategoryCommand { Name = "Test" }, Ct);

        var allCategories = await _mockCategoryRepository.Object.ListAllAsync();
        Assert.Equal(5, allCategories.Count);
        Assert.True(response.Success);
        Assert.Equal("Test", response.Category.Name);
    }

    [Fact]
    public async Task Handle_EmptyName_ReturnsValidationErrors()
    {
        var handler = new CreateCategoryCommandHandler(_mapper, _mockCategoryRepository.Object);

        var response = await handler.Handle(new CreateCategoryCommand { Name = string.Empty }, Ct);

        var allCategories = await _mockCategoryRepository.Object.ListAllAsync();
        Assert.False(response.Success);
        Assert.NotEmpty(response.ValidationErrors);
        Assert.Equal(4, allCategories.Count);
    }
}
