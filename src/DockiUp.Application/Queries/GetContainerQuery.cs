using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Queries
{
    public sealed class GetContainerQuery : IRequest<ContainerDto>
    {
        public string ContainerId { get; }
        public GetContainerQuery(string containerId)
        {
            ContainerId = containerId;
        }
    }

    public sealed class GetContainerQueryHandler : IRequestHandler<GetContainerQuery, ContainerDto>
    {
        private readonly IDockerService _dockerService;

        public GetContainerQueryHandler(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public async ValueTask<ContainerDto> Handle(GetContainerQuery request, CancellationToken cancellationToken)
        {
            var inspect = await _dockerService.InspectContainerAsync(request.ContainerId, cancellationToken);
            if (inspect == null)
                throw new KeyNotFoundException($"Container {request.ContainerId} not found.");
            return inspect;
        }
    }
}
