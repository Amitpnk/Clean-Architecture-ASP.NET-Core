using CA.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CA.Sales
{
    [Route("api/v1/sales")]
    public class SalesController : BaseController
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<SalesListVm>> GetAll()
        {
            var vm = await Mediator.Send(new GetSalesListQuery());

            return Ok(vm);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SaleDetailVm>> Get(int id)
        {
            var vm = await Mediator.Send(new GetSaleDetailQuery { Id = id });

            return Ok(vm);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromBody] CreateSaleCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateSaleCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteSaleCommand { Id = id });

            return NoContent();
        }
    }
}