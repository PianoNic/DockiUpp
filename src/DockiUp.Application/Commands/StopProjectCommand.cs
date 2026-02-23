using DockiUp.Application.Interfaces;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace DockiUp.Application.Commands
{
    /// <summary>Stop a project/stack by id (DockiUp-managed) or by docker project name.</summary>
    public sealed class StopProjectCommand : IRequest
    {
        public StopProjectCommand(int? projectId, string? dockerProjectName = null)
        {
            ProjectId = projectId;
            DockerProjectName = dockerProjectName;
        }

        public int? ProjectId { get; }
        public string? DockerProjectName { get; }
    }

    public sealed class StopProjectCommandHandler : IRequestHandler<StopProjectCommand>
    {
        private readonly IDockerService _dockerService;
        private readonly IDockiUpDbContext _dbContext;

        public StopProjectCommandHandler(IDockerService dockerService, IDockiUpDbContext dbContext)
        {
            _dockerService = dockerService;
            _dbContext = dbContext;
        }

        public async ValueTask<Unit> Handle(StopProjectCommand request, CancellationToken cancellationToken)
        {
            string projectPath = await ResolveProjectPathAsync(request, cancellationToken);
            await _dockerService.StopProjectAsync(projectPath);
            return default;
        }

        private async Task<string> ResolveProjectPathAsync(StopProjectCommand request, CancellationToken cancellationToken)
        {
            if (request.ProjectId.HasValue)
            {
                var project = await _dbContext.ProjectInfo.FindAsync([request.ProjectId.Value], cancellationToken);
                if (project == null)
                    throw new KeyNotFoundException($"Project with id {request.ProjectId} not found.");
                return project.ProjectPath;
            }
            if (!string.IsNullOrWhiteSpace(request.DockerProjectName))
            {
                var dbProject = await _dbContext.ProjectInfo
                    .SingleOrDefaultAsync(p => p.DockerProjectName == request.DockerProjectName, cancellationToken);
                if (dbProject != null)
                    return dbProject.ProjectPath;
                throw new KeyNotFoundException($"Project '{request.DockerProjectName}' not found or not managed by DockiUp.");
            }
            throw new ArgumentException("Provide either ProjectId or DockerProjectName.");
        }
    }
}
