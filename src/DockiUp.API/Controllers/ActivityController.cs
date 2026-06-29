using DockiUp.Application.Dtos;
using DockiUp.Application.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace DockiUp.API.Controllers
{
    /// <summary>Audit log of actions taken in DockiUp.</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ActivityEntryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int limit = 200, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new ListActivityQuery(limit), cancellationToken);
            return Ok(result);
        }
    }
}
