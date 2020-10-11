using CA.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CA.Customers.v1
{
    [Route("api/v{version:apiVersion}/customers")]
    [ApiVersion("1.0")]
    public class CustomersController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<CustomersListViewModel>> GetAll()
        {
            var vm = await Mediator.Send(new GetCustomersListQuery());

            return Ok(vm);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDetailViewModel>> Get(int id)
        {
            var vm = await Mediator.Send(new GetCustomerDetailQuery { Id = id });

            return Ok(vm);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CreateCustomerViewModel>> Create([FromBody] CreateCustomerCommand command)
        {
            var vm = await Mediator.Send(command);

            return Ok(vm);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerCommand command)
        {
            var vm = await Mediator.Send(command);

            return Ok(vm);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteCustomerCommand { Id = id });

            return NoContent();
        }
    }
}