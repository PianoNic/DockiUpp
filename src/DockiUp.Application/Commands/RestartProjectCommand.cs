using DockiUp.Application.Interfaces;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace DockiUp.Application.Commands
{
    /// <summary>Restart a project/stack by id or docker project name (Komodo-style).</summary>
    public sealed class RestartProjectCommand : IRequest
    {
        public RestartProjectCommand(Guid? projectId, string? dockerProjectName = null)
        {
            ProjectId = projectId;
            DockerProjectName = dockerProjectName;
        }

        public Guid? ProjectId { get; }
        public string? DockerProjectName { get; }
    }

    public sealed class RestartProjectCommandHandler : IRequestHandler<RestartProjectCommand>
    {
        private readonly IDockerServiceResolver _dockerResolver;
        private readonly IDockiUpDbContext _dbContext;

        public RestartProjectCommandHandler(IDockerServiceResolver dockerResolver, IDockiUpDbContext dbContext)
        {
            _dockerResolver = dockerResolver;
            _dbContext = dbContext;
        }

        public async ValueTask<Unit> Handle(RestartProjectCommand request, CancellationToken cancellationToken)
        {
            var (projectPath, nodeId) = await ResolveProjectAsync(request, cancellationToken);
            await _dockerResolver.Resolve(nodeId).RestartProjectAsync(projectPath);
            return default;
        }

        private async Task<(string ProjectPath, Guid? NodeId)> ResolveProjectAsync(RestartProjectCommand request, CancellationToken cancellationToken)
        {
            if (request.ProjectId.HasValue)
            {
                var project = await _dbContext.ProjectInfo.FindAsync([request.ProjectId.Value], cancellationToken);
                if (project == null)
                    throw new KeyNotFoundException($"Project with id {request.ProjectId} not found.");
                return (project.ProjectPath, project.NodeId);
            }
            if (!string.IsNullOrWhiteSpace(request.DockerProjectName))
            {
                var dbProject = await _dbContext.ProjectInfo
                    .SingleOrDefaultAsync(p => p.DockerProjectName == request.DockerProjectName, cancellationToken);
                if (dbProject != null)
                    return (dbProject.ProjectPath, dbProject.NodeId);
                throw new KeyNotFoundException($"Project '{request.DockerProjectName}' not found or not managed by DockiUp.");
            }
            throw new ArgumentException("Provide either ProjectId or DockerProjectName.");
        }
    }
}
