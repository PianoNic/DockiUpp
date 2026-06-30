using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Queries
{
    public sealed class GetContainerQuery : IRequest<ContainerDto>
    {
        public string ContainerId { get; }
        public Guid? NodeId { get; }
        public GetContainerQuery(string containerId, Guid? nodeId = null)
        {
            ContainerId = containerId;
            NodeId = nodeId;
        }
    }

    public sealed class GetContainerQueryHandler : IRequestHandler<GetContainerQuery, ContainerDto>
    {
        private readonly IDockerServiceResolver _dockerResolver;

        public GetContainerQueryHandler(IDockerServiceResolver dockerResolver)
        {
            _dockerResolver = dockerResolver;
        }

        public async ValueTask<ContainerDto> Handle(GetContainerQuery request, CancellationToken cancellationToken)
        {
            var inspect = await _dockerResolver.Resolve(request.NodeId).InspectContainerAsync(request.ContainerId, cancellationToken);
            if (inspect == null)
                throw new KeyNotFoundException($"Container {request.ContainerId} not found.");
            return inspect;
        }
    }
}
