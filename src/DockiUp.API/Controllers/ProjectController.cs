using DockiUp.Application.Commands;
using DockiUp.Application.Dtos;
using DockiUp.Application.Queries;
using Mediator;
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
        public async Task<ActionResult<ContainerDto>> DeployProject([FromBody] SetupProjectDto setupDto)
        {
            await _mediator.Send(new DeployProjectCommand(setupDto), HttpContext.RequestAborted);
            return NoContent();
        }

        [HttpGet("GetProjects", Name = "GetProjects")]
        [ProducesResponseType(typeof(ProjectDto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectDto[]>> GetContainers()
        {
            var container = await _mediator.Send(new GetProjectsQuery(), HttpContext.RequestAborted);
            return Ok(container);
        }

        [HttpGet("StopProject", Name = "StopProject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> StopProject([FromQuery] int? projectId, [FromQuery] string? dockerProjectName)
        {
            await _mediator.Send(new StopProjectCommand(projectId, dockerProjectName), HttpContext.RequestAborted);
            return NoContent();
        }

        [HttpPost("RestartProject", Name = "RestartProject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RestartProject([FromQuery] int? projectId, [FromQuery] string? dockerProjectName)
        {
            await _mediator.Send(new RestartProjectCommand(projectId, dockerProjectName), HttpContext.RequestAborted);
            return NoContent();
        }

        [HttpGet("GetProject", Name = "GetProject")]
        [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectDto>> GetProject([FromQuery] int? projectId, [FromQuery] string? dockerProjectName)
        {
            var project = await _mediator.Send(new GetProjectQuery(projectId, dockerProjectName), HttpContext.RequestAborted);
            if (project == null)
                return NotFound();
            return Ok(project);
        }

        [HttpPost("UpdateProject", Name = "UpdateProject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProject([FromQuery] int projectId)
        {
            await _mediator.Send(new UpdateProjectCommand(projectId), HttpContext.RequestAborted);
            return NoContent();
        }
    }
}
