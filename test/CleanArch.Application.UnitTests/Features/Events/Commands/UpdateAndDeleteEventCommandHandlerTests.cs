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
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private readonly Mock<IGenericRepositoryAsync<Event>> _repository = new();
    private readonly UpdateEventCommandHandler _handler;

    public UpdateEventCommandHandlerTests()
    {
        _handler = new UpdateEventCommandHandler(MapperFactory.Create(), _repository.Object);
    }

    [Fact]
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

        await _handler.Handle(command, Ct);

        _repository.Verify(r => r.UpdateAsync(It.Is<Event>(e =>
            e.Id == existing.Id &&
            e.Name == "New" &&
            e.Date == command.Date &&
            e.CategoryId == command.CategoryId &&
            e.Description == "kept")), Times.Once);
    }

    [Fact]
    public async Task Handle_UnknownEvent_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Event)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new UpdateEventCommand { EventId = Guid.NewGuid(), Name = "New" }, Ct));
    }

    [Fact]
    public async Task Handle_InvalidName_ThrowsValidationException_AndDoesNotUpdate()
    {
        var existing = new Event { Id = Guid.NewGuid(), Name = "Old" };
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(new UpdateEventCommand { EventId = existing.Id, Name = null }, Ct));
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Event>()), Times.Never);
    }
}

public class DeleteEventCommandHandlerTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private readonly Mock<IGenericRepositoryAsync<Event>> _repository = new();
    private readonly DeleteEventCommandHandler _handler;

    public DeleteEventCommandHandlerTests()
    {
        _handler = new DeleteEventCommandHandler(_repository.Object);
    }

    [Fact]
    public async Task Handle_ExistingEvent_DeletesIt()
    {
        var existing = new Event { Id = Guid.NewGuid() };
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        await _handler.Handle(new DeleteEventCommand { EventId = existing.Id }, Ct);

        _repository.Verify(r => r.DeleteAsync(existing), Times.Once);
    }

    [Fact]
    public async Task Handle_UnknownEvent_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Event)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteEventCommand { EventId = Guid.NewGuid() }, Ct));

        Assert.StartsWith("Event (", ex.Message);
        _repository.Verify(r => r.DeleteAsync(It.IsAny<Event>()), Times.Never);
    }
}
