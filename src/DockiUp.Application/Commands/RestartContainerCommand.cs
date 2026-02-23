using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Commands
{
    public sealed class RestartContainerCommand : IRequest
    {
        public RestartContainerCommand(string containerId) => ContainerId = containerId;
        public string ContainerId { get; }
    }

    public sealed class RestartContainerCommandHandler : IRequestHandler<RestartContainerCommand>
    {
        private readonly IDockerService _dockerService;

        public RestartContainerCommandHandler(IDockerService dockerService) => _dockerService = dockerService;

        public async ValueTask<Unit> Handle(RestartContainerCommand request, CancellationToken cancellationToken)
        {
            await _dockerService.RestartContainerAsync(request.ContainerId);
            return default;
        }
    }
}
