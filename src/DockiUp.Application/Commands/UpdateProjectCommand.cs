using DockiUp.Application.Interfaces;
using DockiUp.Domain.Enums;
using Mediator;

namespace DockiUp.Application.Commands
{
    /// <summary>Update a project: pull git (if origin is Git) and restart compose (Komodo-style pull + redeploy).</summary>
    public sealed class UpdateProjectCommand : IRequest
    {
        public UpdateProjectCommand(int projectId)
        {
            ProjectId = projectId;
        }

        public int ProjectId { get; }
    }

    public sealed class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
    {
        private readonly IDockerService _dockerService;
        private readonly IDockiUpDbContext _dbContext;
        private readonly IDockiUpProjectConfigurationService _projectConfigurationService;

        public UpdateProjectCommandHandler(
            IDockerService dockerService,
            IDockiUpDbContext dbContext,
            IDockiUpProjectConfigurationService projectConfigurationService)
        {
            _dockerService = dockerService;
            _dbContext = dbContext;
            _projectConfigurationService = projectConfigurationService;
        }

        public async ValueTask<Unit> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _dbContext.ProjectInfo.FindAsync([request.ProjectId], cancellationToken);
            if (project == null)
                throw new KeyNotFoundException($"Project with id {request.ProjectId} not found.");

            if (project.ProjectOrigin == ProjectOriginType.Git)
                await _projectConfigurationService.UpdateRepositoy(project.Id);
            await _dockerService.RestartProjectAsync(project.ProjectPath);
            return default;
        }
    }
}
