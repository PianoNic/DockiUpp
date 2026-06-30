using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Commands
{
    public sealed class RestartContainerCommand : IRequest
    {
        public RestartContainerCommand(string containerId, Guid? nodeId = null)
        {
            ContainerId = containerId;
            NodeId = nodeId;
        }
        public string ContainerId { get; }
        public Guid? NodeId { get; }
    }

    public sealed class RestartContainerCommandHandler : IRequestHandler<RestartContainerCommand>
    {
        private readonly IDockerServiceResolver _dockerResolver;

        public RestartContainerCommandHandler(IDockerServiceResolver dockerResolver) => _dockerResolver = dockerResolver;

        public async ValueTask<Unit> Handle(RestartContainerCommand request, CancellationToken cancellationToken)
        {
            await _dockerResolver.Resolve(request.NodeId).RestartContainerAsync(request.ContainerId);
            return default;
        }
    }
}
