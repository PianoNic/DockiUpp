using DockiUp.Application.Interfaces;
using DockiUp.Domain.Enums;
using Mediator;

namespace DockiUp.Application.Commands
{
    /// <summary>Update a project: pull git (if origin is Git) and restart compose (Komodo-style pull + redeploy).</summary>
    public sealed class UpdateProjectCommand : IRequest
    {
        public UpdateProjectCommand(Guid projectId)
        {
            ProjectId = projectId;
        }

        public Guid ProjectId { get; }
    }

    public sealed class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
    {
        private readonly IDockerServiceResolver _dockerResolver;
        private readonly IDockiUpDbContext _dbContext;
        private readonly IDockiUpProjectConfigurationService _projectConfigurationService;
        private readonly INodeRpc _nodeRpc;

        public UpdateProjectCommandHandler(
            IDockerServiceResolver dockerResolver,
            IDockiUpDbContext dbContext,
            IDockiUpProjectConfigurationService projectConfigurationService,
            INodeRpc nodeRpc)
        {
            _dockerResolver = dockerResolver;
            _dbContext = dbContext;
            _projectConfigurationService = projectConfigurationService;
            _nodeRpc = nodeRpc;
        }

        public async ValueTask<Unit> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _dbContext.ProjectInfo.FindAsync([request.ProjectId], cancellationToken);
            if (project == null)
                throw new KeyNotFoundException($"Project with id {request.ProjectId} not found.");

            // The git pull must happen where the checkout lives - on the node for node-hosted projects.
            if (project.ProjectOrigin == ProjectOriginType.Git)
            {
                if (project.NodeId is Guid nodeId)
                    await _nodeRpc.InvokeAsync<bool>(nodeId, "PullRepository", new object?[] { project.ProjectPath }, cancellationToken);
                else
                    await _projectConfigurationService.UpdateRepositoryAsync(project.ProjectPath);
            }

            await _dockerResolver.Resolve(project.NodeId).RestartProjectAsync(project.ProjectPath);
            return default;
        }
    }
}
