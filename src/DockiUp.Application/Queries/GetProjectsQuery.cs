using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using MediatR;

namespace DockiUp.Application.Queries
{
    public class GetProjectsQuery : IRequest<ComposeProjectDto[]>
    {
        public GetProjectsQuery()
        {
        }
    }
    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, ComposeProjectDto[]>
    {
        private readonly IDockerService _dockerService;

        public GetProjectsQueryHandler(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public async Task<ComposeProjectDto[]> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            return await _dockerService.GetProjectsAsync();
        }
    }
}
