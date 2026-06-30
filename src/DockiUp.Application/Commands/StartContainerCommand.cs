using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Commands
{
    public sealed class StartContainerCommand : IRequest
    {
        public StartContainerCommand(string containerId, Guid? nodeId = null)
        {
            ContainerId = containerId;
            NodeId = nodeId;
        }
        public string ContainerId { get; }
        public Guid? NodeId { get; }
    }

    public sealed class StartContainerCommandHandler : IRequestHandler<StartContainerCommand>
    {
        private readonly IDockerServiceResolver _dockerResolver;

        public StartContainerCommandHandler(IDockerServiceResolver dockerResolver) => _dockerResolver = dockerResolver;

        public async ValueTask<Unit> Handle(StartContainerCommand request, CancellationToken cancellationToken)
        {
            await _dockerResolver.Resolve(request.NodeId).StartContainerAsync(request.ContainerId);
            return default;
        }
    }
}
