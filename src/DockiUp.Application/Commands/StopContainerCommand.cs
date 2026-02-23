using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Commands
{
    public sealed class StopContainerCommand : IRequest
    {
        public StopContainerCommand(string containerId) => ContainerId = containerId;
        public string ContainerId { get; }
    }

    public sealed class StopContainerCommandHandler : IRequestHandler<StopContainerCommand>
    {
        private readonly IDockerService _dockerService;

        public StopContainerCommandHandler(IDockerService dockerService) => _dockerService = dockerService;

        public async ValueTask<Unit> Handle(StopContainerCommand request, CancellationToken cancellationToken)
        {
            await _dockerService.StopContainerAsync(request.ContainerId);
            return default;
        }
    }
}
