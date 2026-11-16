using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Application.Features.Categories.Commands.DeleteCategory;
using CleanArch.Domain.Entities;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Categories.Commands;

public class DeleteCategoryCommandHandlerTests
{
    private Mock<IGenericRepositoryAsync<Category>> _repository;
    private DeleteCategoryCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IGenericRepositoryAsync<Category>>();
        _handler = new DeleteCategoryCommandHandler(_repository.Object);
    }

    [Test]
    public async Task Handle_ExistingCategory_DeletesIt()
    {
        var existing = new Category { Id = Guid.NewGuid(), Name = "Concert" };
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        await _handler.Handle(new DeleteCategoryCommand { CategoryId = existing.Id }, CancellationToken.None);

        _repository.Verify(r => r.DeleteAsync(existing), Times.Once);
    }

    [Test]
    public void Handle_UnknownCategory_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Category)null);

        Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteCategoryCommand { CategoryId = Guid.NewGuid() }, CancellationToken.None));
        _repository.Verify(r => r.DeleteAsync(It.IsAny<Category>()), Times.Never);
    }
}
