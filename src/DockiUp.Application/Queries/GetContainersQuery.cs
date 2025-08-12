using DockiUp.Application.Dtos;
using DockiUp.Application.Enums;
using DockiUp.Application.Services;
using MediatR;

namespace DockiUp.Application.Queries
{
    public class GetContainersQuery : IRequest<ContainerDto[]>
    {
        public GetContainersQuery()
        {
        }
    }
    public class GetContainersQueryHandler : IRequestHandler<GetContainersQuery, ContainerDto[]>
    {
        private readonly IDockiUpConfigService _dockiUpConfigService;

        public GetContainersQueryHandler(IDockiUpConfigService dockiUpConfigService)
        {
            _dockiUpConfigService = dockiUpConfigService;
        }

        public async Task<ContainerDto[]> Handle(GetContainersQuery request, CancellationToken cancellationToken)
        {
            var info = await _dockiUpConfigService.ScanForDockiUpFiles();

            List<ContainerDto> containers = new List<ContainerDto>();
            foreach (var element in info)
            {
                containers.Add(new ContainerDto
                {
                    Id = element.Id,
                    Name = element.Name,
                    LastUpdated = element.LastUpdated,
                    ContainerName = "idk",
                    ContainerState = ContainerState.Created,
                    OpenPorts = []
                });
            }
            return await Task.FromResult(containers.ToArray());
        }
    }
}
