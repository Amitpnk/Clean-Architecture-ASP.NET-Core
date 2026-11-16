using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Features.Events.Commands.CreateEvent;
using CleanArch.Application.Features.Events.Commands.UpdateEvent;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Events.Commands;

public class CreateEventCommandValidatorTests
{
    private Mock<IEventRepository> _repository;
    private CreateEventCommandValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IEventRepository>();
        _repository.Setup(r => r.IsEventNameAndDateUnique(It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(false);
        _validator = new CreateEventCommandValidator(_repository.Object);
    }

    private static CreateEventCommand Valid() => new() { Name = "Expo", Date = DateTime.Now.AddDays(1) };

    [Test]
    public async Task ValidCommand_IsValid()
    {
        var result = await _validator.ValidateAsync(Valid());

        Assert.That(result.IsValid, Is.True);
    }

    [TestCase(null)]
    [TestCase("")]
    public async Task MissingName_IsInvalid(string name)
    {
        var command = Valid();
        command.Name = name;

        var result = await _validator.ValidateAsync(command);

        Assert.That(result.Errors.Select(e => e.PropertyName), Does.Contain(nameof(CreateEventCommand.Name)));
    }

    [Test]
    public async Task NameLongerThan50_IsInvalid()
    {
        var command = Valid();
        command.Name = new string('x', 51);

        var result = await _validator.ValidateAsync(command);

        Assert.That(result.Errors.Select(e => e.ErrorMessage), Does.Contain("Name must not exceed 50 characters."));
    }

    [Test]
    public async Task PastDate_IsInvalid()
    {
        var command = Valid();
        command.Date = DateTime.Now.AddDays(-1);

        var result = await _validator.ValidateAsync(command);

        Assert.That(result.Errors.Select(e => e.PropertyName), Does.Contain(nameof(CreateEventCommand.Date)));
    }

    [Test]
    public async Task MissingDate_IsInvalid()
    {
        var command = Valid();
        command.Date = default;

        var result = await _validator.ValidateAsync(command);

        Assert.That(result.Errors.Select(e => e.PropertyName), Does.Contain(nameof(CreateEventCommand.Date)));
    }

    [Test]
    public async Task DuplicateNameAndDate_IsInvalid()
    {
        _repository.Setup(r => r.IsEventNameAndDateUnique("Expo", It.IsAny<DateTime>())).ReturnsAsync(true);

        var result = await _validator.ValidateAsync(Valid());

        Assert.That(result.Errors.Select(e => e.ErrorMessage), Does.Contain("An event with the same name and date already exists."));
    }
}

public class UpdateEventCommandValidatorTests
{
    private readonly UpdateEventCommandValidator _validator = new();

    [Test]
    public async Task ValidName_IsValid()
    {
        var result = await _validator.ValidateAsync(new UpdateEventCommand { Name = "Expo" });

        Assert.That(result.IsValid, Is.True);
    }

    [TestCase(null)]
    [TestCase("")]
    public async Task MissingName_IsInvalid(string name)
    {
        var result = await _validator.ValidateAsync(new UpdateEventCommand { Name = name });

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task NameLongerThan50_IsInvalid()
    {
        var result = await _validator.ValidateAsync(new UpdateEventCommand { Name = new string('x', 51) });

        Assert.That(result.IsValid, Is.False);
    }
}
