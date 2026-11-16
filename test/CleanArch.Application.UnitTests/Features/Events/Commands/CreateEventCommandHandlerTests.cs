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
    private Mock<IEventRepository> _repository;
    private Mock<IEmailService> _emailService;
    private Mock<ILogger<CreateEventCommandHandler>> _logger;
    private CreateEventCommandHandler _handler;
    private Guid _generatedId;

    [SetUp]
    public void SetUp()
    {
        _generatedId = Guid.NewGuid();
        _repository = new Mock<IEventRepository>();
        _repository.Setup(r => r.IsEventNameAndDateUnique(It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(false);
        _repository.Setup(r => r.AddAsync(It.IsAny<Event>())).ReturnsAsync((Event e) =>
        {
            e.Id = _generatedId;
            return e;
        });
        _emailService = new Mock<IEmailService>();
        _logger = new Mock<ILogger<CreateEventCommandHandler>>();
        _handler = new CreateEventCommandHandler(MapperFactory.Create(), _repository.Object, _emailService.Object, _logger.Object);
    }

    private static CreateEventCommand ValidCommand() => new()
    {
        Name = "Rock night",
        Description = "Loud",
        Date = DateTime.Now.AddDays(10),
        CategoryId = Guid.NewGuid()
    };

    [Test]
    public async Task Handle_ValidCommand_AddsMappedEvent_AndReturnsId()
    {
        var command = ValidCommand();

        var id = await _handler.Handle(command, CancellationToken.None);

        Assert.That(id, Is.EqualTo(_generatedId));
        _repository.Verify(r => r.AddAsync(It.Is<Event>(e =>
            e.Name == command.Name &&
            e.Description == command.Description &&
            e.Date == command.Date &&
            e.CategoryId == command.CategoryId)), Times.Once);
    }

    [Test]
    public async Task Handle_ValidCommand_SendsNotificationEmail()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        _emailService.Verify(s => s.SendEmailAsync(It.Is<MailRequest>(m =>
            m.Subject == "A new event was created" && m.Body.Contains("Rock night"))), Times.Once);
    }

    [Test]
    public async Task Handle_EmailFailure_IsLogged_AndDoesNotFailTheCommand()
    {
        _emailService.Setup(s => s.SendEmailAsync(It.IsAny<MailRequest>())).ThrowsAsync(new InvalidOperationException("smtp down"));

        var id = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.That(id, Is.EqualTo(_generatedId));
        _logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("smtp down")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Test]
    public void Handle_InvalidCommand_ThrowsValidationException_AndDoesNotPersist()
    {
        var command = ValidCommand();
        command.Name = string.Empty;

        var ex = Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));

        Assert.That(ex.ValdationErrors, Is.Not.Empty);
        _repository.Verify(r => r.AddAsync(It.IsAny<Event>()), Times.Never);
        _emailService.Verify(s => s.SendEmailAsync(It.IsAny<MailRequest>()), Times.Never);
    }
}
