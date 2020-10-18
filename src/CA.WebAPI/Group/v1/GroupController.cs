using CA.Application.GroupFeature.Commands;
using CA.Application.GroupFeature.Queries;
using CA.Application.GroupFeature.ViewModel;
using CA.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CA.WebAPI.Group.v1
{

    [Route("api/v{version:apiVersion}/group")]
    [ApiVersion("1.0")]
    public class GroupController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vm = await Mediator.Send(new GetAllGroupsQuery());
            return Ok(vm);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GroupViewModel>> Get(Guid id)
        {
            var vm = await Mediator.Send(new GetGroupByIdQuery(id));
            return Ok(vm);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<GroupViewModel>> Create([FromBody] CreateGroupCommand command)
        {
            var vm = await Mediator.Send(command);
            return Ok(vm);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateGroupCommand command)
        {
            var vm = await Mediator.Send(command);
            return Ok(vm);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Mediator.Send(new DeleteGroupCommand(id));
            return NoContent();
        }
    }
}