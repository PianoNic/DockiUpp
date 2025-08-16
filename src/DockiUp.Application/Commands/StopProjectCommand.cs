using DockiUp.Application.Interfaces;
using MediatR;

namespace DockiUp.Application.Commands
{
    public class StopProjectCommand : IRequest
    {
        public StopProjectCommand()
        {
        }
    }

    public class StopProjectCommandHandler : IRequestHandler<StopProjectCommand>
    {
        private IDockerService _dockerService;

        public StopProjectCommandHandler(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public async Task Handle(StopProjectCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //await _dockerService.StopProjectAsync();
        }
    }
}
