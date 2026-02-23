using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Queries
{
    public sealed class GetContainerLogsQuery : IRequest<string>
    {
        public string ContainerId { get; }
        public int? Tail { get; }

        public GetContainerLogsQuery(string containerId, int? tail = null)
        {
            ContainerId = containerId;
            Tail = tail;
        }
    }

    public sealed class GetContainerLogsQueryHandler : IRequestHandler<GetContainerLogsQuery, string>
    {
        private readonly IDockerService _dockerService;

        public GetContainerLogsQueryHandler(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public async ValueTask<string> Handle(GetContainerLogsQuery request, CancellationToken cancellationToken)
        {
            return await _dockerService.GetContainerLogsAsync(request.ContainerId, request.Tail, cancellationToken);
        }
    }
}
