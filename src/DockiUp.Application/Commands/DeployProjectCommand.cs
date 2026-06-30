using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using DockiUp.Application.Models;
using DockiUp.Domain;
using DockiUp.Domain.Enums;
using Mediator;
using Microsoft.Extensions.Options;

namespace DockiUp.Application.Commands
{
    public sealed class DeployProjectCommand : IRequest
    {
        public DeployProjectCommand(SetupProjectDto setupContainerDto)
        {
            SetupContainerDto = setupContainerDto;
        }

        public SetupProjectDto SetupContainerDto { get; set; }
    }

    public sealed class DeployProjectCommandHandler : IRequestHandler<DeployProjectCommand>
    {
        private readonly IDockerService _dockerService;
        private readonly IDockiUpDbContext _dbContext;
        private readonly SystemPaths _systemPaths;
        private readonly IDockiUpProjectConfigurationService _projectConfigurationService;
        private readonly IActivityLogger _activityLogger;
        private readonly INodeRpc _nodeRpc;

        public DeployProjectCommandHandler(IDockerService dockerService, IOptions<SystemPaths> systemPaths, IDockiUpProjectConfigurationService projectConfigurationService, IDockiUpDbContext dbContext, IActivityLogger activityLogger, INodeRpc nodeRpc)
        {
            _dockerService = dockerService;
            _systemPaths = systemPaths.Value;
            _projectConfigurationService = projectConfigurationService;
            _dbContext = dbContext;
            _activityLogger = activityLogger;
            _nodeRpc = nodeRpc;
        }

        public async ValueTask<Unit> Handle(DeployProjectCommand request, CancellationToken cancellationToken)
        {
            var dto = request.SetupContainerDto;

            // Import has nothing to write or start yet (parity with the previous handler).
            if (dto.ProjectOrigin == ProjectOriginType.Import)
            {
                await _activityLogger.LogAsync("deploy", dto.ProjectName,
                    details: dto.ProjectOrigin.ToString(), cancellationToken: cancellationToken);
                return default;
            }

            string projectPath;
            string composePath;

            if (dto.NodeId is Guid nodeId)
            {
                // Ship the whole definition to the node; it clones/writes/composes on its own filesystem
                // (it has no app database) and reports back the paths it used.
                var result = await _nodeRpc.InvokeAsync<NodeDeployResultDto>(
                    nodeId, "DeployProject", new object?[] { dto }, cancellationToken);
                projectPath = result.ProjectPath;
                composePath = result.ComposePath;
            }
            else
            {
                projectPath = Path.Combine(_systemPaths.ProjectsPath, dto.ProjectName);
                Directory.CreateDirectory(projectPath);
                if (dto.ProjectOrigin == ProjectOriginType.Git)
                    await _projectConfigurationService.CloneRepositoryAsync(projectPath, dto.GitUrl!);
                composePath = await _projectConfigurationService.WriteComposeFileAsync(projectPath, dto.Compose!);
                await _dockerService.StartProjectAsync(projectPath);
            }

            // The control plane always owns the project row, even for node-hosted projects.
            var projectInfo = new ProjectInfo
            {
                ProjectName = dto.ProjectName,
                DockerProjectName = new string(dto.ProjectName.ToLower().Where(c => !char.IsWhiteSpace(c)).ToArray()),
                Description = dto.Description,

                ProjectOrigin = dto.ProjectOrigin,
                GitUrl = dto.GitUrl,
                NodeId = dto.NodeId,
                ProjectPath = projectPath,
                ComposePath = composePath,

                ProjectUpdateMethod = dto.ProjectUpdateMethod,
                WebhookUrl = dto.WebhookUrl,
                PeriodicIntervalInMinutes = dto.PeriodicIntervalInMinutes
            };

            await _dbContext.ProjectInfo.AddAsync(projectInfo, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _activityLogger.LogAsync("deploy", dto.ProjectName,
                details: dto.ProjectOrigin.ToString(), cancellationToken: cancellationToken);

            return default;
        }
    }
}
