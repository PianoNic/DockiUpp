using DockiUp.Application.Dtos;
using DockiUp.Application.Services;
using MediatR;

namespace DockiUp.Application.Commands
{
    public class CreateContainerCommand : IRequest
    {
        public CreateContainerCommand(SetupContainerDto setupContainerDto)
        {
            SetupContainerDto = setupContainerDto;
        }

        public SetupContainerDto SetupContainerDto { get; set; }
    }
    public class CreateContainerCommandHandler : IRequestHandler<CreateContainerCommand>
    {
        private readonly IDockerService _dockerService;

        public CreateContainerCommandHandler(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public async Task Handle(CreateContainerCommand request, CancellationToken cancellationToken)
        {
            Guid containerId = await _dockerService.SetupDirectory(request.SetupContainerDto);
            await _dockerService.StartAsync(containerId);
        }
    }
}
