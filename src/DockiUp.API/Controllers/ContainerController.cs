using DockiUp.Application.Commands;
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
        public async Task<ActionResult<ContainerDto>> GetContainer(Guid containerId)
        {
            var container = await _mediator.Send(new GetContainerQuery(containerId));
            return Ok(container);
        }

        [HttpGet("GetContainers", Name = "GetContainers")]
        [ProducesResponseType(typeof(ContainerDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ContainerDto>> GetContainers()
        {
            var container = await _mediator.Send(new GetContainersQuery());
            return Ok(container);
        }

        [HttpPost("CreateContainer", Name = "CreateContainer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ContainerDto>> CreateContainer([FromBody] SetupContainerDto setupContainerDto)
        {
            await _mediator.Send(new CreateContainerCommand(setupContainerDto));
            return Ok();
        }
    }
}
