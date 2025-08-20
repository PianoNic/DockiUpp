using DockiUp.Application.Dtos;
using DockiUp.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DockiUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAppInfo", Name = "GetAppInfo")]
        [ProducesResponseType(typeof(AppInfoDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AppInfoDto>> GetAppInfo()
        {
            var appInfo = await _mediator.Send(new GetAppInfoQuery(), HttpContext.RequestAborted);
            return Ok(appInfo);
        }
    }
}
