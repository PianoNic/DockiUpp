using DockiUp.Application.Commands;
using DockiUp.Application.Dtos;
using DockiUp.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DockiUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("DeployProject", Name = "DeployProject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContainerDto>> DeployProject([FromBody] SetupProjectDto containerId)
        {
            await _mediator.Send(new DeployProjectCommand(containerId));
            return NoContent();
        }

        [HttpGet("GetProjects", Name = "GetProjects")]
        [ProducesResponseType(typeof(ComposeProjectDto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ComposeProjectDto[]>> GetContainers()
        {
            var container = await _mediator.Send(new GetProjectsQuery());
            return Ok(container);
        }

        [HttpGet("StopProject", Name = "StopProject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> StopProject()
        {
            await _mediator.Send(new StopProjectCommand());
            return NoContent();
        }
    }
}
