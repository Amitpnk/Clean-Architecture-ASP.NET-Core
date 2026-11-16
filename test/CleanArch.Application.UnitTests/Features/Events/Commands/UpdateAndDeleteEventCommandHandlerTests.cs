using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Application.Features.Events.Commands.DeleteEvent;
using CleanArch.Application.Features.Events.Commands.UpdateEvent;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Domain.Entities;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Events.Commands;

public class UpdateEventCommandHandlerTests
{
    private Mock<IGenericRepositoryAsync<Event>> _repository;
    private UpdateEventCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IGenericRepositoryAsync<Event>>();
        _handler = new UpdateEventCommandHandler(MapperFactory.Create(), _repository.Object);
    }

    [Test]
    public async Task Handle_ExistingEvent_MapsFieldsAndUpdates()
    {
        var existing = new Event { Id = Guid.NewGuid(), Name = "Old", Description = "kept", Date = DateTime.Today, CategoryId = Guid.NewGuid() };
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        var command = new UpdateEventCommand
        {
            EventId = existing.Id,
            Name = "New",
            Date = DateTime.Today.AddDays(3),
            CategoryId = Guid.NewGuid()
        };

        await _handler.Handle(command, CancellationToken.None);

        _repository.Verify(r => r.UpdateAsync(It.Is<Event>(e =>
            e.Id == existing.Id &&
            e.Name == "New" &&
            e.Date == command.Date &&
            e.CategoryId == command.CategoryId &&
            e.Description == "kept")), Times.Once);
    }

    [Test]
    public void Handle_UnknownEvent_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Event)null);

        Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new UpdateEventCommand { EventId = Guid.NewGuid(), Name = "New" }, CancellationToken.None));
    }

    [Test]
    public void Handle_InvalidName_ThrowsValidationException_AndDoesNotUpdate()
    {
        var existing = new Event { Id = Guid.NewGuid(), Name = "Old" };
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(new UpdateEventCommand { EventId = existing.Id, Name = null }, CancellationToken.None));
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Event>()), Times.Never);
    }
}

public class DeleteEventCommandHandlerTests
{
    private Mock<IGenericRepositoryAsync<Event>> _repository;
    private DeleteEventCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IGenericRepositoryAsync<Event>>();
        _handler = new DeleteEventCommandHandler(_repository.Object);
    }

    [Test]
    public async Task Handle_ExistingEvent_DeletesIt()
    {
        var existing = new Event { Id = Guid.NewGuid() };
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        await _handler.Handle(new DeleteEventCommand { EventId = existing.Id }, CancellationToken.None);

        _repository.Verify(r => r.DeleteAsync(existing), Times.Once);
    }

    [Test]
    public void Handle_UnknownEvent_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Event)null);

        var ex = Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteEventCommand { EventId = Guid.NewGuid() }, CancellationToken.None));

        Assert.That(ex.Message, Does.StartWith("Event ("));
        _repository.Verify(r => r.DeleteAsync(It.IsAny<Event>()), Times.Never);
    }
}
