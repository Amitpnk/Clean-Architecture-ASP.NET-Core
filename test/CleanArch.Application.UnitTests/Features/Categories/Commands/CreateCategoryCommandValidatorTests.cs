using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Features.Categories.Commands.CreateCategory;
using CleanArch.Application.Features.Categories.Commands.UpdateCategory;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Categories.Commands;

public class CreateCategoryCommandValidatorTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private readonly Mock<ICategoryRepository> _repository = new();
    private readonly CreateCategoryCommandValidator _validator;

    public CreateCategoryCommandValidatorTests()
    {
        _repository.Setup(r => r.IsCategoryName(It.IsAny<string>())).ReturnsAsync(false);
        _validator = new CreateCategoryCommandValidator(_repository.Object);
    }

    [Fact]
    public async Task ValidName_IsValid()
    {
        var result = await _validator.ValidateAsync(new CreateCategoryCommand { Name = "Workshops" }, Ct);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task MissingName_IsInvalid(string name)
    {
        var result = await _validator.ValidateAsync(new CreateCategoryCommand { Name = name }, Ct);

        Assert.Contains(nameof(CreateCategoryCommand.Name), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task NameLongerThan50_IsInvalid()
    {
        var result = await _validator.ValidateAsync(new CreateCategoryCommand { Name = new string('x', 51) }, Ct);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task DuplicateName_IsInvalid()
    {
        _repository.Setup(r => r.IsCategoryName("Concert")).ReturnsAsync(true);

        var result = await _validator.ValidateAsync(new CreateCategoryCommand { Name = "Concert" }, Ct);

        Assert.Contains("An category with the same name already exists.", result.Errors.Select(e => e.ErrorMessage));
    }
}

public class UpdateCategoryCommandValidatorTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private readonly UpdateCategoryCommandValidator _validator = new();

    [Fact]
    public async Task ValidName_IsValid()
    {
        var result = await _validator.ValidateAsync(new UpdateCategoryCommand { Name = "Plays" }, Ct);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task MissingName_IsInvalid(string name)
    {
        var result = await _validator.ValidateAsync(new UpdateCategoryCommand { Name = name }, Ct);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task NameLongerThan50_IsInvalid()
    {
        var result = await _validator.ValidateAsync(new UpdateCategoryCommand { Name = new string('x', 51) }, Ct);

        Assert.Equal("Name must not exceed 50 characters.", Assert.Single(result.Errors).ErrorMessage);
    }
}
