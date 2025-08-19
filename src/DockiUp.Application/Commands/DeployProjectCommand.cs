using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using DockiUp.Application.Models;
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
        private readonly SystemPaths _systemPaths;
        private readonly IDockiUpProjectConfigurationService _projectConfigurationService;

        public DeployProjectCommandHandler(IDockerService dockerService, IOptions<SystemPaths> systemPaths, IDockiUpProjectConfigurationService projectConfigurationService)
        {
            _dockerService = dockerService;
            _systemPaths = systemPaths.Value;
            _projectConfigurationService = projectConfigurationService;
        }

        public async Task Handle(DeployProjectCommand request, CancellationToken cancellationToken)
        {
            string containerFolderPath = Path.Combine(_systemPaths.ProjectsPath, request.SetupContainerDto.ProjectName);
            Directory.CreateDirectory(containerFolderPath);

            if (request.SetupContainerDto.ProjectOrigin == ProjectOriginType.Compose && request.SetupContainerDto.Compose != null)
            {
                await _projectConfigurationService.GenerateDockiUpConfigFileAsync(containerFolderPath, request.SetupContainerDto);
                await _projectConfigurationService.WriteComposeFileAsync(containerFolderPath, request.SetupContainerDto.Compose);
                await _dockerService.StartProjectAsync(containerFolderPath);
            }
        }
    }
}
