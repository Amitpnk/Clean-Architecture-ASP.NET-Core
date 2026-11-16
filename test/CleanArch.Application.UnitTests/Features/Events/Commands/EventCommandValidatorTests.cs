using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Features.Events.Commands.CreateEvent;
using CleanArch.Application.Features.Events.Commands.UpdateEvent;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Events.Commands;

public class CreateEventCommandValidatorTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private readonly Mock<IEventRepository> _repository = new();
    private readonly CreateEventCommandValidator _validator;

    public CreateEventCommandValidatorTests()
    {
        _repository.Setup(r => r.IsEventNameAndDateUnique(It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(false);
        _validator = new CreateEventCommandValidator(_repository.Object);
    }

    private static CreateEventCommand Valid() => new() { Name = "Expo", Date = DateTime.Now.AddDays(1) };

    [Fact]
    public async Task ValidCommand_IsValid()
    {
        var result = await _validator.ValidateAsync(Valid(), Ct);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task MissingName_IsInvalid(string name)
    {
        var command = Valid();
        command.Name = name;

        var result = await _validator.ValidateAsync(command, Ct);

        Assert.Contains(nameof(CreateEventCommand.Name), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task NameLongerThan50_IsInvalid()
    {
        var command = Valid();
        command.Name = new string('x', 51);

        var result = await _validator.ValidateAsync(command, Ct);

        Assert.Contains("Name must not exceed 50 characters.", result.Errors.Select(e => e.ErrorMessage));
    }

    [Fact]
    public async Task PastDate_IsInvalid()
    {
        var command = Valid();
        command.Date = DateTime.Now.AddDays(-1);

        var result = await _validator.ValidateAsync(command, Ct);

        Assert.Contains(nameof(CreateEventCommand.Date), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task MissingDate_IsInvalid()
    {
        var command = Valid();
        command.Date = default;

        var result = await _validator.ValidateAsync(command, Ct);

        Assert.Contains(nameof(CreateEventCommand.Date), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task DuplicateNameAndDate_IsInvalid()
    {
        _repository.Setup(r => r.IsEventNameAndDateUnique("Expo", It.IsAny<DateTime>())).ReturnsAsync(true);

        var result = await _validator.ValidateAsync(Valid(), Ct);

        Assert.Contains("An event with the same name and date already exists.", result.Errors.Select(e => e.ErrorMessage));
    }
}

public class UpdateEventCommandValidatorTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private readonly UpdateEventCommandValidator _validator = new();

    [Fact]
    public async Task ValidName_IsValid()
    {
        var result = await _validator.ValidateAsync(new UpdateEventCommand { Name = "Expo" }, Ct);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task MissingName_IsInvalid(string name)
    {
        var result = await _validator.ValidateAsync(new UpdateEventCommand { Name = name }, Ct);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task NameLongerThan50_IsInvalid()
    {
        var result = await _validator.ValidateAsync(new UpdateEventCommand { Name = new string('x', 51) }, Ct);

        Assert.False(result.IsValid);
    }
}
