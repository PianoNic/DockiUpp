using DockiUp.Application.Dtos;
using DockiUp.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DockiUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContainerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContainerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetContainer", Name = "GetContainer")]
        [ProducesResponseType(typeof(ContainerDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ContainerDto>> GetContainer(string containerId)
        {
            var container = await _mediator.Send(new GetContainerQuery(containerId), HttpContext.RequestAborted);
            return Ok(container);
        }
    }
}
