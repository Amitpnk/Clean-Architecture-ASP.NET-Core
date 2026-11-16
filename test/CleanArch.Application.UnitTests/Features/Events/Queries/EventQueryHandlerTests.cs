using CleanArch.Application.Contracts.Infrastructure;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Application.Features.Events.Queries.GetEventDetail;
using CleanArch.Application.Features.Events.Queries.GetEventsExport;
using CleanArch.Application.Features.Events.Queries.GetEventsList;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Domain.Entities;
using Moq;

namespace CleanArch.Application.UnitTests.Features.Events.Queries;

public class GetEventsListQueryHandlerTests
{
    [Test]
    public async Task Handle_ReturnsEventsSortedByDate_WithIdMappedToEventId()
    {
        var late = new Event { Id = Guid.NewGuid(), Name = "Late", Date = new DateTime(2027, 12, 1) };
        var early = new Event { Id = Guid.NewGuid(), Name = "Early", Date = new DateTime(2027, 1, 1) };
        var repository = new Mock<IGenericRepositoryAsync<Event>>();
        repository.Setup(r => r.ListAllAsync()).ReturnsAsync([late, early]);
        var handler = new GetEventsListQueryHandler(MapperFactory.Create(), repository.Object);

        var result = await handler.Handle(new GetEventsListQuery(), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Select(e => e.Name), Is.EqualTo(new[] { "Early", "Late" }));
            Assert.That(result.Select(e => e.EventId), Is.EqualTo(new[] { early.Id, late.Id }));
        }
    }

    [Test]
    public async Task Handle_NoEvents_ReturnsEmptyList()
    {
        var repository = new Mock<IGenericRepositoryAsync<Event>>();
        repository.Setup(r => r.ListAllAsync()).ReturnsAsync([]);
        var handler = new GetEventsListQueryHandler(MapperFactory.Create(), repository.Object);

        var result = await handler.Handle(new GetEventsListQuery(), CancellationToken.None);

        Assert.That(result, Is.Empty);
    }
}

public class GetEventDetailQueryHandlerTests
{
    private Mock<IGenericRepositoryAsync<Event>> _events;
    private Mock<IGenericRepositoryAsync<Category>> _categories;
    private GetEventDetailQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _events = new Mock<IGenericRepositoryAsync<Event>>();
        _categories = new Mock<IGenericRepositoryAsync<Category>>();
        _handler = new GetEventDetailQueryHandler(MapperFactory.Create(), _events.Object, _categories.Object);
    }

    [Test]
    public async Task Handle_ExistingEvent_ReturnsDetailWithCategory()
    {
        var category = new Category { Id = Guid.NewGuid(), Name = "Concert" };
        var @event = new Event { Id = Guid.NewGuid(), Name = "Live", Description = "desc", Date = new DateTime(2027, 3, 3), CategoryId = category.Id };
        _events.Setup(r => r.GetByIdAsync(@event.Id)).ReturnsAsync(@event);
        _categories.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

        var result = await _handler.Handle(new GetEventDetailQuery { Id = @event.Id }, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EventId, Is.EqualTo(@event.Id));
            Assert.That(result.Name, Is.EqualTo("Live"));
            Assert.That(result.Description, Is.EqualTo("desc"));
            Assert.That(result.Date, Is.EqualTo(@event.Date));
            Assert.That(result.CategoryId, Is.EqualTo(category.Id));
            Assert.That(result.Category.Id, Is.EqualTo(category.Id));
            Assert.That(result.Category.Name, Is.EqualTo("Concert"));
        }
    }

    [Test]
    public void Handle_UnknownEvent_ThrowsNotFound()
    {
        _events.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Event)null);

        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new GetEventDetailQuery { Id = Guid.NewGuid() }, CancellationToken.None));
    }

    [Test]
    public void Handle_EventWithMissingCategory_ThrowsNotFound()
    {
        var @event = new Event { Id = Guid.NewGuid(), CategoryId = Guid.NewGuid() };
        _events.Setup(r => r.GetByIdAsync(@event.Id)).ReturnsAsync(@event);
        _categories.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Category)null);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new GetEventDetailQuery { Id = @event.Id }, CancellationToken.None));

        Assert.That(ex.Message, Does.StartWith("Category ("));
    }
}

public class GetEventsExportQueryHandlerTests
{
    [Test]
    public async Task Handle_ExportsEventsSortedByDate_AsCsvFile()
    {
        var late = new Event { Id = Guid.NewGuid(), Name = "Late", Date = new DateTime(2027, 12, 1) };
        var early = new Event { Id = Guid.NewGuid(), Name = "Early", Date = new DateTime(2027, 1, 1) };
        var repository = new Mock<IGenericRepositoryAsync<Event>>();
        repository.Setup(r => r.ListAllAsync()).ReturnsAsync([late, early]);

        List<EventExportDto> exported = null;
        var csvBytes = new byte[] { 1, 2, 3 };
        var exporter = new Mock<ICsvExporter>();
        exporter.Setup(e => e.ExportEventsToCsv(It.IsAny<List<EventExportDto>>()))
            .Callback<List<EventExportDto>>(dtos => exported = dtos)
            .Returns(csvBytes);

        var handler = new GetEventsExportQueryHandler(MapperFactory.Create(), repository.Object, exporter.Object);

        var result = await handler.Handle(new GetEventsExportQuery(), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ContentType, Is.EqualTo("text/csv"));
            Assert.That(result.Data, Is.SameAs(csvBytes));
            Assert.That(result.EventExportFileName, Does.EndWith(".csv"));
            Assert.That(exported.Select(e => e.EventId), Is.EqualTo(new[] { early.Id, late.Id }));
        }
    }
}
