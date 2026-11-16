using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesListWithEvents;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Domain.Entities;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Categories.Queries;

public class GetCategoriesListWithEventsQueryHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_PassesIncludeHistory_AndMapsCategoriesWithEvents(bool includeHistory)
    {
        var categoryId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var date = new DateTime(2027, 5, 1);
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(r => r.GetCategoriesWithEvents(includeHistory)).ReturnsAsync(
        [
            new Category
            {
                Id = categoryId,
                Name = "Concert",
                Events = [new Event { Id = eventId, Name = "Live", Date = date, CategoryId = categoryId }]
            }
        ]);
        var handler = new GetCategoriesListWithEventsQueryHandler(MapperFactory.Create(), repository.Object);

        var result = await handler.Handle(new GetCategoriesListWithEventsQuery { IncludeHistory = includeHistory },
            TestContext.Current.CancellationToken);

        var category = Assert.Single(result);
        var @event = Assert.Single(category.Events);
        Assert.Equal(categoryId, category.CategoryId);
        Assert.Equal("Concert", category.Name);
        Assert.Equal(eventId, @event.EventId);
        Assert.Equal("Live", @event.Name);
        Assert.Equal(date, @event.Date);
        Assert.Equal(categoryId, @event.CategoryId);
        repository.Verify(r => r.GetCategoriesWithEvents(includeHistory), Times.Once);
    }
}
