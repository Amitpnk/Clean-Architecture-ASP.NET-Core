using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Application.Features.Categories.Commands.UpdateCategory;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Domain.Entities;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Categories.Commands;

public class UpdateCategoryCommandHandlerTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private readonly Mock<IGenericRepositoryAsync<Category>> _repository = new();
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _handler = new UpdateCategoryCommandHandler(MapperFactory.Create(), _repository.Object);
    }

    [Fact]
    public async Task Handle_ExistingCategory_MapsNameAndUpdates()
    {
        var existing = new Category { Id = Guid.NewGuid(), Name = "Old", Description = "kept" };
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        await _handler.Handle(new UpdateCategoryCommand { CategoryId = existing.Id, Name = "New" }, Ct);

        _repository.Verify(r => r.UpdateAsync(It.Is<Category>(c =>
            c.Id == existing.Id && c.Name == "New" && c.Description == "kept")), Times.Once);
    }

    [Fact]
    public async Task Handle_UnknownCategory_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Category)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new UpdateCategoryCommand { CategoryId = Guid.NewGuid(), Name = "New" }, Ct));
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidName_ThrowsValidationException_AndDoesNotUpdate()
    {
        var existing = new Category { Id = Guid.NewGuid(), Name = "Old" };
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(new UpdateCategoryCommand { CategoryId = existing.Id, Name = "" }, Ct));

        Assert.NotEmpty(ex.ValidationErrors);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }
}
