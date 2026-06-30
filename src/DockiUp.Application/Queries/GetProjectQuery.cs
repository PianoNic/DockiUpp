using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Queries
{
    /// <summary>Get a single project by id or docker project name (Komodo-style).</summary>
    public sealed class GetProjectQuery : IRequest<ProjectDto?>
    {
        public GetProjectQuery(Guid? projectId, string? dockerProjectName = null)
        {
            ProjectId = projectId;
            DockerProjectName = dockerProjectName;
        }

        public Guid? ProjectId { get; }
        public string? DockerProjectName { get; }
    }

    public sealed class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto?>
    {
        private readonly IDockerService _localDocker;
        private readonly IDockerServiceResolver _dockerResolver;
        private readonly IDockiUpDbContext _dbContext;

        public GetProjectQueryHandler(IDockerService localDocker, IDockerServiceResolver dockerResolver, IDockiUpDbContext dbContext)
        {
            _localDocker = localDocker;
            _dockerResolver = dockerResolver;
            _dbContext = dbContext;
        }

        public async ValueTask<ProjectDto?> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            if (request.ProjectId.HasValue)
            {
                var dbProject = await _dbContext.ProjectInfo.FindAsync([request.ProjectId.Value], cancellationToken);
                if (dbProject == null)
                    return null;

                // Read container status from wherever the project runs (local host or its node).
                var byName = await _dockerResolver.Resolve(dbProject.NodeId).GetProjectByDockerNameAsync(dbProject.DockerProjectName);
                if (byName == null)
                {
                    return new ProjectDto
                    {
                        Id = dbProject.Id,
                        ProjectName = dbProject.ProjectName,
                        DockerProjectName = dbProject.DockerProjectName,
                        ProjectDescription = dbProject.Description ?? "",
                        ManagedByDockiUp = true,
                        NodeId = dbProject.NodeId,
                        Containers = [],
                        ProjectPath = dbProject.ProjectPath,
                        UpdateMethod = dbProject.ProjectUpdateMethod.ToString()
                    };
                }
                byName.Id = dbProject.Id;
                byName.ProjectName = dbProject.ProjectName;
                byName.ProjectDescription = dbProject.Description ?? byName.ProjectDescription;
                byName.ManagedByDockiUp = true;
                byName.NodeId = dbProject.NodeId;
                byName.ProjectPath = dbProject.ProjectPath;
                byName.UpdateMethod = dbProject.ProjectUpdateMethod.ToString();
                return byName;
            }
            if (!string.IsNullOrWhiteSpace(request.DockerProjectName))
                return await _localDocker.GetProjectByDockerNameAsync(request.DockerProjectName);
            return null;
        }
    }
}
