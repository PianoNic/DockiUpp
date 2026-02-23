using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Commands
{
    public sealed class StartContainerCommand : IRequest
    {
        public StartContainerCommand(string containerId) => ContainerId = containerId;
        public string ContainerId { get; }
    }

    public sealed class StartContainerCommandHandler : IRequestHandler<StartContainerCommand>
    {
        private readonly IDockerService _dockerService;

        public StartContainerCommandHandler(IDockerService dockerService) => _dockerService = dockerService;

        public async ValueTask<Unit> Handle(StartContainerCommand request, CancellationToken cancellationToken)
        {
            await _dockerService.StartContainerAsync(request.ContainerId);
            return default;
        }
    }
}
