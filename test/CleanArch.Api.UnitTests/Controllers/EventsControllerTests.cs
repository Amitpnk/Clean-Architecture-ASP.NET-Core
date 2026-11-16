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
    private Mock<IDispatcher> _dispatcher;
    private EventsController _controller;

    [SetUp]
    public void SetUp()
    {
        _dispatcher = new Mock<IDispatcher>();
        _controller = new EventsController().WithDispatcher(_dispatcher);
    }

    [Test]
    public async Task GetAll_ReturnsOkWithEvents()
    {
        var events = new List<EventListVm> { new() { EventId = Guid.NewGuid(), Name = "Expo" } };
        _dispatcher.Setup(d => d.Query(It.IsAny<GetEventsListQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

        var result = await _controller.GetAll();

        Assert.That(((OkObjectResult)result).Value, Is.SameAs(events));
    }

    [Test]
    public async Task GetEventById_QueriesById()
    {
        var id = Guid.NewGuid();
        var detail = new EventDetailVm { EventId = id };
        _dispatcher.Setup(d => d.Query(It.Is<GetEventDetailQuery>(q => q.Id == id), It.IsAny<CancellationToken>())).ReturnsAsync(detail);

        var result = await _controller.GetEventById(id);

        Assert.That(((OkObjectResult)result).Value, Is.SameAs(detail));
    }

    [Test]
    public async Task Create_ReturnsOkWithNewId()
    {
        var id = Guid.NewGuid();
        var command = new CreateEventCommand { Name = "Expo" };
        _dispatcher.Setup(d => d.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(id);

        var result = await _controller.Create(command);

        Assert.That(((OkObjectResult)result.Result!).Value, Is.EqualTo(id));
    }

    [Test]
    public async Task Update_SendsCommand_AndReturnsNoContent()
    {
        var command = new UpdateEventCommand { EventId = Guid.NewGuid(), Name = "Expo" };
        _dispatcher.Setup(d => d.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

        var result = await _controller.Update(command);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _dispatcher.Verify(d => d.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Delete_SendsCommandWithId_AndReturnsNoContent()
    {
        var id = Guid.NewGuid();
        _dispatcher.Setup(d => d.Send(It.IsAny<DeleteEventCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

        var result = await _controller.Delete(id);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _dispatcher.Verify(d => d.Send(It.Is<DeleteEventCommand>(c => c.EventId == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ExportEvents_ReturnsFileWithContentTypeAndName()
    {
        var file = new EventExportFileVm { Data = [1, 2, 3], ContentType = "text/csv", EventExportFileName = "events.csv" };
        _dispatcher.Setup(d => d.Query(It.IsAny<GetEventsExportQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(file);

        var result = (FileContentResult)await _controller.ExportEvents();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.FileContents, Is.EqualTo(file.Data));
            Assert.That(result.ContentType, Is.EqualTo("text/csv"));
            Assert.That(result.FileDownloadName, Is.EqualTo("events.csv"));
        }
    }
}
