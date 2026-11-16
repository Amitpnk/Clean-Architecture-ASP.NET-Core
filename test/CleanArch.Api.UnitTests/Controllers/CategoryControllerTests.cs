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
    private readonly Mock<IDispatcher> _dispatcher = new();
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _controller = new CategoryController().WithDispatcher(_dispatcher);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsOkWithQueryResult()
    {
        var categories = new List<CategoryListVm> { new() { CategoryId = Guid.NewGuid(), Name = "Concert" } };
        _dispatcher.Setup(d => d.Query(It.IsAny<GetCategoriesListQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(categories);

        var result = await _controller.GetAllCategories();

        Assert.Same(categories, Assert.IsType<OkObjectResult>(result.Result).Value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetCategoriesWithEvents_ForwardsIncludeHistory(bool includeHistory)
    {
        var categories = new List<CategoryEventListVm>();
        _dispatcher.Setup(d => d.Query(It.IsAny<GetCategoriesListWithEventsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(categories);

        var result = await _controller.GetCategoriesWithEvents(includeHistory);

        Assert.Same(categories, Assert.IsType<OkObjectResult>(result.Result).Value);
        _dispatcher.Verify(d => d.Query(
            It.Is<GetCategoriesListWithEventsQuery>(q => q.IncludeHistory == includeHistory),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_SendsCommand_AndReturnsOkWithResponse()
    {
        var command = new CreateCategoryCommand { Name = "Workshops" };
        var response = new CreateCategoryCommandResponse();
        _dispatcher.Setup(d => d.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.Create(command);

        Assert.Same(response, Assert.IsType<OkObjectResult>(result.Result).Value);
    }
}
