using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesListWithEvents;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Domain.Entities;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Categories.Queries;

public class GetCategoriesListWithEventsQueryHandlerTests
{
    [TestCase(true)]
    [TestCase(false)]
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

        var result = await handler.Handle(new GetCategoriesListWithEventsQuery { IncludeHistory = includeHistory }, CancellationToken.None);

        var category = result.Single();
        var @event = category.Events.Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(category.CategoryId, Is.EqualTo(categoryId));
            Assert.That(category.Name, Is.EqualTo("Concert"));
            Assert.That(@event.EventId, Is.EqualTo(eventId));
            Assert.That(@event.Name, Is.EqualTo("Live"));
            Assert.That(@event.Date, Is.EqualTo(date));
            Assert.That(@event.CategoryId, Is.EqualTo(categoryId));
        }
        repository.Verify(r => r.GetCategoriesWithEvents(includeHistory), Times.Once);
    }
}
