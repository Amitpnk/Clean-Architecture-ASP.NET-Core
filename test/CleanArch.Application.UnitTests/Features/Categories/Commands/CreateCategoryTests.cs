using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Features.Categories.Commands.CreateCategory;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Common.UnitTests.Mocks;
using MapsterMapper;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Categories.Commands;

public class CreateCategoryTests
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
    public async Task Handle_ValidCategory_AddedToCategoriesRepo()
    {
        var handler = new CreateCategoryCommandHandler(_mapper, _mockCategoryRepository.Object);

        var response = await handler.Handle(new CreateCategoryCommand { Name = "Test" }, CancellationToken.None);

        var allCategories = await _mockCategoryRepository.Object.ListAllAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(allCategories, Has.Count.EqualTo(5));
            Assert.That(response.Success, Is.True);
            Assert.That(response.Category.Name, Is.EqualTo("Test"));
        }
    }

    [Test]
    public async Task Handle_EmptyName_ReturnsValidationErrors()
    {
        var handler = new CreateCategoryCommandHandler(_mapper, _mockCategoryRepository.Object);

        var response = await handler.Handle(new CreateCategoryCommand { Name = string.Empty }, CancellationToken.None);

        var allCategories = await _mockCategoryRepository.Object.ListAllAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Success, Is.False);
            Assert.That(response.ValidationErrors, Is.Not.Empty);
            Assert.That(allCategories, Has.Count.EqualTo(4));
        }
    }
}
