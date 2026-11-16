using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Application.Features.Categories.Commands.UpdateCategory;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Domain.Entities;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Categories.Commands;

public class UpdateCategoryCommandHandlerTests
{
    private Mock<IGenericRepositoryAsync<Category>> _repository;
    private UpdateCategoryCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IGenericRepositoryAsync<Category>>();
        _handler = new UpdateCategoryCommandHandler(MapperFactory.Create(), _repository.Object);
    }

    [Test]
    public async Task Handle_ExistingCategory_MapsNameAndUpdates()
    {
        var existing = new Category { Id = Guid.NewGuid(), Name = "Old", Description = "kept" };
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        await _handler.Handle(new UpdateCategoryCommand { CategoryId = existing.Id, Name = "New" }, CancellationToken.None);

        _repository.Verify(r => r.UpdateAsync(It.Is<Category>(c =>
            c.Id == existing.Id && c.Name == "New" && c.Description == "kept")), Times.Once);
    }

    [Test]
    public void Handle_UnknownCategory_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Category)null);

        Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new UpdateCategoryCommand { CategoryId = Guid.NewGuid(), Name = "New" }, CancellationToken.None));
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Test]
    public void Handle_InvalidName_ThrowsValidationException_AndDoesNotUpdate()
    {
        var existing = new Category { Id = Guid.NewGuid(), Name = "Old" };
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        var ex = Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(new UpdateCategoryCommand { CategoryId = existing.Id, Name = "" }, CancellationToken.None));

        Assert.That(ex.ValdationErrors, Is.Not.Empty);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }
}
