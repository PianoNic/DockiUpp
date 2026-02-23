using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Queries
{
    public sealed class GetProjectsQuery : IRequest<ProjectDto[]>
    {
        public GetProjectsQuery()
        {
        }
    }

    public sealed class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, ProjectDto[]>
    {
        private readonly IDockerService _dockerService;

        public GetProjectsQueryHandler(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public async ValueTask<ProjectDto[]> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            return await _dockerService.GetProjectsAsync();
        }
    }
}
