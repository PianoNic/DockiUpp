using DockiUp.Application.Dtos;
using DockiUp.Application.Enums;
using MediatR;

namespace DockiUp.Application.Queries
{
    public class GetContainerQuery : IRequest<ContainerDto>
    {
        public Guid ContainerId { get; }
        public GetContainerQuery(Guid containerId)
        {
            ContainerId = containerId;
        }
    }
    public class GetContainerQueryHandler : IRequestHandler<GetContainerQuery, ContainerDto>
    {
        public async Task<ContainerDto> Handle(GetContainerQuery request, CancellationToken cancellationToken)
        {
            var container = new ContainerDto()
            {
                Id = request.ContainerId,
                Name = "Test Container",
                ContainerName = "funny-snake",
                ContainerState = ContainerState.Running,
                LastUpdated = DateTime.UtcNow,
                OpenPorts = [1223, 224, 1112]
            };
            return await Task.FromResult(container);
        }
    }
}
