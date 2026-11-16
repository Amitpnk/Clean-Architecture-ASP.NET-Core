using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Application.Features.Categories.Commands.DeleteCategory;
using CleanArch.Domain.Entities;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Categories.Commands;

public class DeleteCategoryCommandHandlerTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private readonly Mock<IGenericRepositoryAsync<Category>> _repository = new();
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _handler = new DeleteCategoryCommandHandler(_repository.Object);
    }

    [Fact]
    public async Task Handle_ExistingCategory_DeletesIt()
    {
        var existing = new Category { Id = Guid.NewGuid(), Name = "Concert" };
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        await _handler.Handle(new DeleteCategoryCommand { CategoryId = existing.Id }, Ct);

        _repository.Verify(r => r.DeleteAsync(existing), Times.Once);
    }

    [Fact]
    public async Task Handle_UnknownCategory_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Category)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteCategoryCommand { CategoryId = Guid.NewGuid() }, Ct));
        _repository.Verify(r => r.DeleteAsync(It.IsAny<Category>()), Times.Never);
    }
}
