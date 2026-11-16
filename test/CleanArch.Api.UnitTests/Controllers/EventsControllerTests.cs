using CleanArch.Api.Event.v1;
using CleanArch.Application.CQRS;
using CleanArch.Application.Features.Events.Commands.CreateEvent;
using CleanArch.Application.Features.Events.Commands.DeleteEvent;
using CleanArch.Application.Features.Events.Commands.UpdateEvent;
using CleanArch.Application.Features.Events.Queries.GetEventDetail;
using CleanArch.Application.Features.Events.Queries.GetEventsExport;
using CleanArch.Application.Features.Events.Queries.GetEventsList;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CleanArch.Api.UnitTests.Controllers;

public class EventsControllerTests
{
    private readonly Mock<IDispatcher> _dispatcher = new();
    private readonly EventsController _controller;

    public EventsControllerTests()
    {
        _controller = new EventsController().WithDispatcher(_dispatcher);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithEvents()
    {
        var events = new List<EventListVm> { new() { EventId = Guid.NewGuid(), Name = "Expo" } };
        _dispatcher.Setup(d => d.Query(It.IsAny<GetEventsListQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

        var result = await _controller.GetAll();

        Assert.Same(events, Assert.IsType<OkObjectResult>(result).Value);
    }

    [Fact]
    public async Task GetEventById_QueriesById()
    {
        var id = Guid.NewGuid();
        var detail = new EventDetailVm { EventId = id };
        _dispatcher.Setup(d => d.Query(It.Is<GetEventDetailQuery>(q => q.Id == id), It.IsAny<CancellationToken>())).ReturnsAsync(detail);

        var result = await _controller.GetEventById(id);

        Assert.Same(detail, Assert.IsType<OkObjectResult>(result).Value);
    }

    [Fact]
    public async Task Create_ReturnsOkWithNewId()
    {
        var id = Guid.NewGuid();
        var command = new CreateEventCommand { Name = "Expo" };
        _dispatcher.Setup(d => d.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(id);

        var result = await _controller.Create(command);

        Assert.Equal(id, Assert.IsType<OkObjectResult>(result.Result).Value);
    }

    [Fact]
    public async Task Update_SendsCommand_AndReturnsNoContent()
    {
        var command = new UpdateEventCommand { EventId = Guid.NewGuid(), Name = "Expo" };
        _dispatcher.Setup(d => d.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

        var result = await _controller.Update(command);

        Assert.IsType<NoContentResult>(result);
        _dispatcher.Verify(d => d.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_SendsCommandWithId_AndReturnsNoContent()
    {
        var id = Guid.NewGuid();
        _dispatcher.Setup(d => d.Send(It.IsAny<DeleteEventCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

        var result = await _controller.Delete(id);

        Assert.IsType<NoContentResult>(result);
        _dispatcher.Verify(d => d.Send(It.Is<DeleteEventCommand>(c => c.EventId == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExportEvents_ReturnsFileWithContentTypeAndName()
    {
        var file = new EventExportFileVm { Data = [1, 2, 3], ContentType = "text/csv", EventExportFileName = "events.csv" };
        _dispatcher.Setup(d => d.Query(It.IsAny<GetEventsExportQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(file);

        var result = Assert.IsType<FileContentResult>(await _controller.ExportEvents());

        Assert.Equal(file.Data, result.FileContents);
        Assert.Equal("text/csv", result.ContentType);
        Assert.Equal("events.csv", result.FileDownloadName);
    }
}
