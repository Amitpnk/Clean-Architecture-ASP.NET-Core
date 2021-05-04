using CleanArch.Api.Controllers;
using CleanArch.Application.Features.Events.Commands.CreateEvent;
using CleanArch.Application.Features.Events.Commands.DeleteEvent;
using CleanArch.Application.Features.Events.Commands.UpdateEvent;
using CleanArch.Application.Features.Events.Queries.GetEventDetail;
using CleanArch.Application.Features.Events.Queries.GetEventsExport;
using CleanArch.Application.Features.Events.Queries.GetEventsList;
using CleanArch.CrossCuttingConcerns.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CleanArch.Api.Event.v1
{
    public class EventsController : BaseController
    {
        [HttpGet(Name = "GetAllEvents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetAll()
        {
            var vm = await Mediator.Send(new GetEventsListQuery());
            return Ok(vm);
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetEventById")]
        public async Task<IActionResult> GetEventById(Guid id)
        {
            var getEventDetailQuery = new GetEventDetailQuery() { Id = id };
            return Ok(await Mediator.Send(getEventDetailQuery));
        }

        [Authorize]
        [HttpPost(Name = "AddEvent")]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateEventCommand createEventCommand)
        {
            var id = await Mediator.Send(createEventCommand);
            return Ok(id);
        }

        [Authorize]
        [HttpPut(Name = "UpdateEvent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Update([FromBody] UpdateEventCommand updateEventCommand)
        {
            await Mediator.Send(updateEventCommand);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}", Name = "DeleteEvent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleteEventCommand = new DeleteEventCommand() { EventId = id };
            await Mediator.Send(deleteEventCommand);
            return NoContent();
        }

        [Authorize]
        [HttpGet("export", Name = "ExportEvents")]
        [FileResultContentType("text/csv")]
        public async Task<FileResult> ExportEvents()
        {
            var fileDto = await Mediator.Send(new GetEventsExportQuery());

            return File(fileDto.Data, fileDto.ContentType, fileDto.EventExportFileName);
        }

    }
}