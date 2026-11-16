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
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Handle_ReturnsEventsSortedByDate_WithIdMappedToEventId()
    {
        var late = new Event { Id = Guid.NewGuid(), Name = "Late", Date = new DateTime(2027, 12, 1) };
        var early = new Event { Id = Guid.NewGuid(), Name = "Early", Date = new DateTime(2027, 1, 1) };
        var repository = new Mock<IGenericRepositoryAsync<Event>>();
        repository.Setup(r => r.ListAllAsync()).ReturnsAsync([late, early]);
        var handler = new GetEventsListQueryHandler(MapperFactory.Create(), repository.Object);

        var result = await handler.Handle(new GetEventsListQuery(), Ct);

        Assert.Equal(["Early", "Late"], result.Select(e => e.Name));
        Assert.Equal([early.Id, late.Id], result.Select(e => e.EventId));
    }

    [Fact]
    public async Task Handle_NoEvents_ReturnsEmptyList()
    {
        var repository = new Mock<IGenericRepositoryAsync<Event>>();
        repository.Setup(r => r.ListAllAsync()).ReturnsAsync([]);
        var handler = new GetEventsListQueryHandler(MapperFactory.Create(), repository.Object);

        var result = await handler.Handle(new GetEventsListQuery(), Ct);

        Assert.Empty(result);
    }
}

public class GetEventDetailQueryHandlerTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private readonly Mock<IGenericRepositoryAsync<Event>> _events = new();
    private readonly Mock<IGenericRepositoryAsync<Category>> _categories = new();
    private readonly GetEventDetailQueryHandler _handler;

    public GetEventDetailQueryHandlerTests()
    {
        _handler = new GetEventDetailQueryHandler(MapperFactory.Create(), _events.Object, _categories.Object);
    }

    [Fact]
    public async Task Handle_ExistingEvent_ReturnsDetailWithCategory()
    {
        var category = new Category { Id = Guid.NewGuid(), Name = "Concert" };
        var @event = new Event { Id = Guid.NewGuid(), Name = "Live", Description = "desc", Date = new DateTime(2027, 3, 3), CategoryId = category.Id };
        _events.Setup(r => r.GetByIdAsync(@event.Id)).ReturnsAsync(@event);
        _categories.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

        var result = await _handler.Handle(new GetEventDetailQuery { Id = @event.Id }, Ct);

        Assert.Equal(@event.Id, result.EventId);
        Assert.Equal("Live", result.Name);
        Assert.Equal("desc", result.Description);
        Assert.Equal(@event.Date, result.Date);
        Assert.Equal(category.Id, result.CategoryId);
        Assert.Equal(category.Id, result.Category.Id);
        Assert.Equal("Concert", result.Category.Name);
    }

    [Fact]
    public async Task Handle_UnknownEvent_ThrowsNotFound()
    {
        _events.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Event)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new GetEventDetailQuery { Id = Guid.NewGuid() }, Ct));
    }

    [Fact]
    public async Task Handle_EventWithMissingCategory_ThrowsNotFound()
    {
        var @event = new Event { Id = Guid.NewGuid(), CategoryId = Guid.NewGuid() };
        _events.Setup(r => r.GetByIdAsync(@event.Id)).ReturnsAsync(@event);
        _categories.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Category)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new GetEventDetailQuery { Id = @event.Id }, Ct));

        Assert.StartsWith("Category (", ex.Message);
    }
}

public class GetEventsExportQueryHandlerTests
{
    [Fact]
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

        var result = await handler.Handle(new GetEventsExportQuery(), TestContext.Current.CancellationToken);

        Assert.Equal("text/csv", result.ContentType);
        Assert.Same(csvBytes, result.Data);
        Assert.EndsWith(".csv", result.EventExportFileName);
        Assert.Equal([early.Id, late.Id], exported.Select(e => e.EventId));
    }
}
