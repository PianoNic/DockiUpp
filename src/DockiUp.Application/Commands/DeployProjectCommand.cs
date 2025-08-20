using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using DockiUp.Application.Models;
using DockiUp.Domain;
using DockiUp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace DockiUp.Application.Commands
{
    public class DeployProjectCommand : IRequest
    {
        public DeployProjectCommand(SetupProjectDto setupContainerDto)
        {
            SetupContainerDto = setupContainerDto;
        }

        public SetupProjectDto SetupContainerDto { get; set; }
    }

    public class DeployProjectCommandHandler : IRequestHandler<DeployProjectCommand>
    {
        private readonly IDockerService _dockerService;
        private readonly IDockiUpDbContext _dbContext;
        private readonly SystemPaths _systemPaths;
        private readonly IDockiUpProjectConfigurationService _projectConfigurationService;

        public DeployProjectCommandHandler(IDockerService dockerService, IOptions<SystemPaths> systemPaths, IDockiUpProjectConfigurationService projectConfigurationService, IDockiUpDbContext dbContext)
        {
            _dockerService = dockerService;
            _systemPaths = systemPaths.Value;
            _projectConfigurationService = projectConfigurationService;
            _dbContext = dbContext;
        }

        public async Task Handle(DeployProjectCommand request, CancellationToken cancellationToken)
        {
            string containerFolderPath = Path.Combine(_systemPaths.ProjectsPath, request.SetupContainerDto.ProjectName);
            Directory.CreateDirectory(containerFolderPath);

            if (request.SetupContainerDto.ProjectOrigin == ProjectOriginType.Compose)
            {
                await HandleComposeProjectDeploymentAsync(request, containerFolderPath, cancellationToken);
            }
            else if (request.SetupContainerDto.ProjectOrigin == ProjectOriginType.Git)
            {
                await HandleGitProjectDeploymentAsync(request, containerFolderPath, cancellationToken);
            }
            else if (request.SetupContainerDto.ProjectOrigin == ProjectOriginType.Import)
            {

            }
        }

        private async Task HandleGitProjectDeploymentAsync(DeployProjectCommand request, string containerFolderPath, CancellationToken cancellationToken)
        {
            await _projectConfigurationService.CloneRepositoryAsync(containerFolderPath, request.SetupContainerDto.GitUrl);
            var composeFilePath = await _projectConfigurationService.WriteComposeFileAsync(containerFolderPath, request.SetupContainerDto.Compose);
            await _dockerService.StartProjectAsync(containerFolderPath);

            var projectInfo = new ProjectInfo
            {
                ProjectName = request.SetupContainerDto.ProjectName,
                DockerProjectName = new string(request.SetupContainerDto.ProjectName.ToLower().Where(c => !char.IsWhiteSpace(c)).ToArray()),
                Description = request.SetupContainerDto.Description,

                ProjectOrigin = request.SetupContainerDto.ProjectOrigin,
                ProjectPath = containerFolderPath,
                ComposePath = composeFilePath,

                ProjectUpdateMethod = request.SetupContainerDto.ProjectUpdateMethod,
                WebhookUrl = request.SetupContainerDto.WebhookUrl,
                PeriodicIntervalInMinutes = request.SetupContainerDto.PeriodicIntervalInMinutes
            };

            await _dbContext.ProjectInfo.AddAsync(projectInfo, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task HandleComposeProjectDeploymentAsync(DeployProjectCommand request, string containerFolderPath, CancellationToken cancellationToken)
        {
            var composeFilePath = await _projectConfigurationService.WriteComposeFileAsync(containerFolderPath, request.SetupContainerDto.Compose);
            await _dockerService.StartProjectAsync(containerFolderPath);

            var projectInfo = new ProjectInfo
            {
                ProjectName = request.SetupContainerDto.ProjectName,
                DockerProjectName = new string(request.SetupContainerDto.ProjectName.ToLower().Where(c => !char.IsWhiteSpace(c)).ToArray()),
                Description = request.SetupContainerDto.Description,

                ProjectOrigin = request.SetupContainerDto.ProjectOrigin,
                ProjectPath = containerFolderPath,
                ComposePath = composeFilePath,

                ProjectUpdateMethod = request.SetupContainerDto.ProjectUpdateMethod,
                WebhookUrl = request.SetupContainerDto.WebhookUrl,
                PeriodicIntervalInMinutes = request.SetupContainerDto.PeriodicIntervalInMinutes
            };

            await _dbContext.ProjectInfo.AddAsync(projectInfo, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
