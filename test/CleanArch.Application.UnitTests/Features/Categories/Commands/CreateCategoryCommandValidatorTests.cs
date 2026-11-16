using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Features.Categories.Commands.CreateCategory;
using CleanArch.Application.Features.Categories.Commands.UpdateCategory;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Categories.Commands;

public class CreateCategoryCommandValidatorTests
{
    private Mock<ICategoryRepository> _repository;
    private CreateCategoryCommandValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ICategoryRepository>();
        _repository.Setup(r => r.IsCategoryName(It.IsAny<string>())).ReturnsAsync(false);
        _validator = new CreateCategoryCommandValidator(_repository.Object);
    }

    [Test]
    public async Task ValidName_IsValid()
    {
        var result = await _validator.ValidateAsync(new CreateCategoryCommand { Name = "Workshops" });

        Assert.That(result.IsValid, Is.True);
    }

    [TestCase(null)]
    [TestCase("")]
    public async Task MissingName_IsInvalid(string name)
    {
        var result = await _validator.ValidateAsync(new CreateCategoryCommand { Name = name });

        Assert.That(result.Errors.Select(e => e.PropertyName), Does.Contain(nameof(CreateCategoryCommand.Name)));
    }

    [Test]
    public async Task NameLongerThan50_IsInvalid()
    {
        var result = await _validator.ValidateAsync(new CreateCategoryCommand { Name = new string('x', 51) });

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task DuplicateName_IsInvalid()
    {
        _repository.Setup(r => r.IsCategoryName("Concert")).ReturnsAsync(true);

        var result = await _validator.ValidateAsync(new CreateCategoryCommand { Name = "Concert" });

        Assert.That(result.Errors.Select(e => e.ErrorMessage), Does.Contain("An category with the same name already exists."));
    }
}

public class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator = new();

    [Test]
    public async Task ValidName_IsValid()
    {
        var result = await _validator.ValidateAsync(new UpdateCategoryCommand { Name = "Plays" });

        Assert.That(result.IsValid, Is.True);
    }

    [TestCase(null)]
    [TestCase("")]
    public async Task MissingName_IsInvalid(string name)
    {
        var result = await _validator.ValidateAsync(new UpdateCategoryCommand { Name = name });

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task NameLongerThan50_IsInvalid()
    {
        var result = await _validator.ValidateAsync(new UpdateCategoryCommand { Name = new string('x', 51) });

        Assert.That(result.Errors.Single().ErrorMessage, Is.EqualTo("Name must not exceed 50 characters."));
    }
}
