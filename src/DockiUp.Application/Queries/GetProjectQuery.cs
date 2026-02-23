using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Queries
{
    /// <summary>Get a single project by id or docker project name (Komodo-style).</summary>
    public sealed class GetProjectQuery : IRequest<ProjectDto?>
    {
        public GetProjectQuery(int? projectId, string? dockerProjectName = null)
        {
            ProjectId = projectId;
            DockerProjectName = dockerProjectName;
        }

        public int? ProjectId { get; }
        public string? DockerProjectName { get; }
    }

    public sealed class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto?>
    {
        private readonly IDockerService _dockerService;
        private readonly IDockiUpDbContext _dbContext;

        public GetProjectQueryHandler(IDockerService dockerService, IDockiUpDbContext dbContext)
        {
            _dockerService = dockerService;
            _dbContext = dbContext;
        }

        public async ValueTask<ProjectDto?> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            if (request.ProjectId.HasValue)
            {
                var dbProject = await _dbContext.ProjectInfo.FindAsync([request.ProjectId.Value], cancellationToken);
                if (dbProject == null)
                    return null;
                var byName = await _dockerService.GetProjectByDockerNameAsync(dbProject.DockerProjectName);
                if (byName == null)
                {
                    return new ProjectDto
                    {
                        Id = dbProject.Id,
                        ProjectName = dbProject.ProjectName,
                        DockerProjectName = dbProject.DockerProjectName,
                        ProjectDescription = dbProject.Description ?? "",
                        ManagedByDockiUp = true,
                        Containers = [],
                        ProjectPath = dbProject.ProjectPath,
                        UpdateMethod = dbProject.ProjectUpdateMethod.ToString()
                    };
                }
                byName.Id = dbProject.Id;
                byName.ProjectPath = dbProject.ProjectPath;
                byName.UpdateMethod = dbProject.ProjectUpdateMethod.ToString();
                return byName;
            }
            if (!string.IsNullOrWhiteSpace(request.DockerProjectName))
                return await _dockerService.GetProjectByDockerNameAsync(request.DockerProjectName);
            return null;
        }
    }
}
