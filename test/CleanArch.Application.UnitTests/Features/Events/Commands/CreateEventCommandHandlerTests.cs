using CleanArch.Application.Contracts.Infrastructure;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Application.Features.Events.Commands.CreateEvent;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Services.Mail;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Events.Commands;

public class CreateEventCommandHandlerTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private readonly Mock<IEventRepository> _repository = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<ILogger<CreateEventCommandHandler>> _logger = new();
    private readonly Guid _generatedId = Guid.NewGuid();
    private readonly CreateEventCommandHandler _handler;

    public CreateEventCommandHandlerTests()
    {
        _repository.Setup(r => r.IsEventNameAndDateUnique(It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(false);
        _repository.Setup(r => r.AddAsync(It.IsAny<Event>())).ReturnsAsync((Event e) =>
        {
            e.Id = _generatedId;
            return e;
        });
        _handler = new CreateEventCommandHandler(MapperFactory.Create(), _repository.Object, _emailService.Object, _logger.Object);
    }

    private static CreateEventCommand ValidCommand() => new()
    {
        Name = "Rock night",
        Description = "Loud",
        Date = DateTime.Now.AddDays(10),
        CategoryId = Guid.NewGuid()
    };

    [Fact]
    public async Task Handle_ValidCommand_AddsMappedEvent_AndReturnsId()
    {
        var command = ValidCommand();

        var id = await _handler.Handle(command, Ct);

        Assert.Equal(_generatedId, id);
        _repository.Verify(r => r.AddAsync(It.Is<Event>(e =>
            e.Name == command.Name &&
            e.Description == command.Description &&
            e.Date == command.Date &&
            e.CategoryId == command.CategoryId)), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_SendsNotificationEmail()
    {
        await _handler.Handle(ValidCommand(), Ct);

        _emailService.Verify(s => s.SendEmailAsync(It.Is<MailRequest>(m =>
            m.Subject == "A new event was created" && m.Body.Contains("Rock night"))), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailFailure_IsLogged_AndDoesNotFailTheCommand()
    {
        _emailService.Setup(s => s.SendEmailAsync(It.IsAny<MailRequest>())).ThrowsAsync(new InvalidOperationException("smtp down"));

        var id = await _handler.Handle(ValidCommand(), Ct);

        Assert.Equal(_generatedId, id);
        _logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("smtp down")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ThrowsValidationException_AndDoesNotPersist()
    {
        var command = ValidCommand();
        command.Name = string.Empty;

        var ex = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, Ct));

        Assert.NotEmpty(ex.ValidationErrors);
        _repository.Verify(r => r.AddAsync(It.IsAny<Event>()), Times.Never);
        _emailService.Verify(s => s.SendEmailAsync(It.IsAny<MailRequest>()), Times.Never);
    }
}
