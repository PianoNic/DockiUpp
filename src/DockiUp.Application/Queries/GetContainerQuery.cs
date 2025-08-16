using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using MediatR;

namespace DockiUp.Application.Queries
{
    public class GetContainerQuery : IRequest<ContainerDto>
    {
        public string ContainerId { get; }
        public GetContainerQuery(string containerId)
        {
            ContainerId = containerId;
        }
    }

    public class GetContainerQueryHandler : IRequestHandler<GetContainerQuery, ContainerDto>
    {
        private readonly IDockerService _dockerService;

        public GetContainerQueryHandler(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public async Task<ContainerDto> Handle(GetContainerQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
