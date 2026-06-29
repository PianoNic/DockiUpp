using DockiUp.Application.Dtos;
using DockiUp.Application.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace DockiUp.API.Controllers
{
    /// <summary>At-a-glance dashboard stats.</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController(IMediator mediator) : ControllerBase
    {
        [HttpGet("stats")]
        [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Stats(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetDashboardStatsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
