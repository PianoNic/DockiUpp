using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Commands
{
    public sealed class StopContainerCommand : IRequest
    {
        public StopContainerCommand(string containerId, Guid? nodeId = null)
        {
            ContainerId = containerId;
            NodeId = nodeId;
        }
        public string ContainerId { get; }
        public Guid? NodeId { get; }
    }

    public sealed class StopContainerCommandHandler : IRequestHandler<StopContainerCommand>
    {
        private readonly IDockerServiceResolver _dockerResolver;

        public StopContainerCommandHandler(IDockerServiceResolver dockerResolver) => _dockerResolver = dockerResolver;

        public async ValueTask<Unit> Handle(StopContainerCommand request, CancellationToken cancellationToken)
        {
            await _dockerResolver.Resolve(request.NodeId).StopContainerAsync(request.ContainerId);
            return default;
        }
    }
}
