using CleanArch.Api.Category.v1;
using CleanArch.Application.CQRS;
using CleanArch.Application.Features.Categories.Commands.CreateCategory;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesList;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesListWithEvents;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CleanArch.Api.UnitTests.Controllers;

public class CategoryControllerTests
{
    private Mock<IDispatcher> _dispatcher;
    private CategoryController _controller;

    [SetUp]
    public void SetUp()
    {
        _dispatcher = new Mock<IDispatcher>();
        _controller = new CategoryController().WithDispatcher(_dispatcher);
    }

    [Test]
    public async Task GetAllCategories_ReturnsOkWithQueryResult()
    {
        var categories = new List<CategoryListVm> { new() { CategoryId = Guid.NewGuid(), Name = "Concert" } };
        _dispatcher.Setup(d => d.Query(It.IsAny<GetCategoriesListQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(categories);

        var result = await _controller.GetAllCategories();

        Assert.That(((OkObjectResult)result.Result!).Value, Is.SameAs(categories));
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task GetCategoriesWithEvents_ForwardsIncludeHistory(bool includeHistory)
    {
        var categories = new List<CategoryEventListVm>();
        _dispatcher.Setup(d => d.Query(It.IsAny<GetCategoriesListWithEventsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(categories);

        var result = await _controller.GetCategoriesWithEvents(includeHistory);

        Assert.That(((OkObjectResult)result.Result!).Value, Is.SameAs(categories));
        _dispatcher.Verify(d => d.Query(
            It.Is<GetCategoriesListWithEventsQuery>(q => q.IncludeHistory == includeHistory),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Create_SendsCommand_AndReturnsOkWithResponse()
    {
        var command = new CreateCategoryCommand { Name = "Workshops" };
        var response = new CreateCategoryCommandResponse();
        _dispatcher.Setup(d => d.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.Create(command);

        Assert.That(((OkObjectResult)result.Result!).Value, Is.SameAs(response));
    }
}
